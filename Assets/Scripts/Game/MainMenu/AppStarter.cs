
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AppStarter
{
    public async UniTask<(StoriesProvider, MainMenuUIProvider, LevelLoader)> StartApp(PrefabsProvider prefabsProvider, 
        Wallet wallet, LoadScreenUIHandler loadScreenUIHandler, ReactiveCommand onSceneTransition, SaveServiceProvider saveServiceProvider,
        GlobalSound globalSound, MainMenuLocalizationHandler mainMenuLocalizationHandler)
    {
        await Addressables.InitializeAsync();

        var loadIndicatorUIHandler = new LoadIndicatorUIHandler();
        var blackFrameUIHandler = new BlackFrameUIHandler();

        await loadScreenUIHandler.Init(loadIndicatorUIHandler, blackFrameUIHandler);
        (MainMenuUIProvider, MainMenuUIView, Transform) result =
            await CreateMainMenuUIProvider(wallet, loadIndicatorUIHandler, blackFrameUIHandler, loadScreenUIHandler, mainMenuLocalizationHandler.LanguageChanged);

        var storiesProvider = await CreateStoriesProvider();
        await mainMenuLocalizationHandler.Init(saveServiceProvider.SaveData, storiesProvider);
        loadScreenUIHandler.ShowOnStart();


        globalSound.SetGlobalSoundData(await new GlobalAudioAssetProvider().LoadGlobalAudioAsset());
        globalSound.Construct(saveServiceProvider.SaveData.SoundStatus);

        var levelLoader = LevelLoaderCreate(result.Item1, onSceneTransition, saveServiceProvider, result.Item3, storiesProvider);


        await InitMainMenuUI(globalSound.SoundStatus, mainMenuLocalizationHandler, levelLoader, result.Item1, result.Item2, result.Item3, storiesProvider, saveServiceProvider.SaveData.StartIndexStory);


        await prefabsProvider.Init();
        result.Item2.gameObject.SetActive(true);


        loadScreenUIHandler.HideOnMainMenuMove().Forget();
        return (storiesProvider, result.Item1, levelLoader);
    }

    private async UniTask<(MainMenuUIProvider, MainMenuUIView, Transform)> CreateMainMenuUIProvider(Wallet wallet,
        LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler, LoadScreenUIHandler loadScreenUIHandler,
        ReactiveCommand languageChanged)
    {
        MainMenuCanvasAssetProvider menuCanvasAssetProvider = new MainMenuCanvasAssetProvider();
        MainMenuUIView mainMenuUIView = await menuCanvasAssetProvider.CreateAsset();
        mainMenuUIView.GetComponent<Canvas>().worldCamera = Camera.main;
        var mainMenuUIViewTransform = mainMenuUIView.transform;
        var swipeDetectorOff = new ReactiveCommand<bool>();
        var darkeningBackgroundFrameUIHandler = new BlackFrameUIHandler();
        var playStoryPanelHandler = new PlayStoryPanelHandler(darkeningBackgroundFrameUIHandler, languageChanged);
        var settingsPanelUIHandler = new SettingsPanelUIHandler(languageChanged, swipeDetectorOff);
        var settingsPanelButtonUIHandler = new SettingsPanelButtonUIHandler(loadScreenUIHandler.CanvasTransfornLoadScreen, settingsPanelUIHandler,
            darkeningBackgroundFrameUIHandler,loadIndicatorUIHandler);
        
        var shopMoneyPanelUIHandler = new ShopMoneyPanelUIHandler(loadIndicatorUIHandler, darkeningBackgroundFrameUIHandler, wallet,
            loadScreenUIHandler.CanvasTransfornLoadScreen, swipeDetectorOff);
        var shopMoneyButtonsUIHandler = new ShopMoneyButtonsUIHandler(wallet, shopMoneyPanelUIHandler, mainMenuUIViewTransform);

        var myScrollHandler = new MyScrollHandler(mainMenuUIView.MyScrollUIView, languageChanged, swipeDetectorOff);
        var confirmedPanelUIHandler = new ConfirmedPanelUIHandler(loadIndicatorUIHandler, darkeningBackgroundFrameUIHandler, mainMenuUIViewTransform);
        var bottomPanelUIHandler = new BottomPanelUIHandler(confirmedPanelUIHandler,
            new AdvertisingButtonUIHandler(loadIndicatorUIHandler, darkeningBackgroundFrameUIHandler, wallet, mainMenuUIViewTransform),
            mainMenuUIViewTransform, languageChanged);
        
        MainMenuUIProvider mainMenuUIProvider = new MainMenuUIProvider(blackFrameUIHandler, darkeningBackgroundFrameUIHandler,
            loadIndicatorUIHandler, playStoryPanelHandler, settingsPanelButtonUIHandler, settingsPanelUIHandler, shopMoneyPanelUIHandler,
            shopMoneyButtonsUIHandler, confirmedPanelUIHandler,loadScreenUIHandler,bottomPanelUIHandler, myScrollHandler);
        return (mainMenuUIProvider, mainMenuUIView, mainMenuUIViewTransform);
    }

    private async UniTask InitMainMenuUI(IReactiveProperty<bool> soundStatus, ILocalizationChanger localizationChanger, LevelLoader levelLoader, MainMenuUIProvider mainMenuUIProvider,
        MainMenuUIView mainMenuUIView, Transform mainMenuUIViewTransform, StoriesProvider storiesProvider, int startIndexStory)
    {
        await mainMenuUIProvider.DarkeningBackgroundFrameUIHandler.Init(mainMenuUIViewTransform);
        await mainMenuUIProvider.PlayStoryPanelHandler.Init(levelLoader, mainMenuUIViewTransform);
        await mainMenuUIProvider.MyScrollHandler.Construct(storiesProvider.Stories, mainMenuUIProvider.PlayStoryPanelHandler, levelLoader, startIndexStory);
        mainMenuUIProvider.SettingsButtonUIHandler.Init(mainMenuUIView.SettingsButtonView, soundStatus, localizationChanger);
        
        
        mainMenuUIProvider.ShopButtonsUIHandler.Init(mainMenuUIView.MonetPanelView, mainMenuUIView.HeartsPanelView);
        mainMenuUIProvider.BottomPanelUIHandler.Init(mainMenuUIView.BottomPanelView, mainMenuUIProvider.DarkeningBackgroundFrameUIHandler);
    }
    private LevelLoader LevelLoaderCreate(MainMenuUIProvider mainMenuUIProvider, ReactiveCommand onSceneTransition,
        SaveServiceProvider saveServiceProvider, Transform mainMenuUIViewTransform, StoriesProvider storiesProvider)
    {
        return new LevelLoader(storiesProvider, mainMenuUIProvider.LoadScreenUIHandler,
            mainMenuUIProvider.LoadIndicatorUIHandler, mainMenuUIViewTransform, onSceneTransition, saveServiceProvider);
    }

    private async UniTask<StoriesProvider> CreateStoriesProvider()
    {
        var storiesProviderAssetProvider = new StoriesProviderAssetProvider();
        return await storiesProviderAssetProvider.Load();
    }
}