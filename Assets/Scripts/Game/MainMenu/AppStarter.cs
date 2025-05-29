
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AppStarter
{
    public async UniTask<(StoriesProvider, MainMenuUIProvider, LevelLoader)> StartApp(PrefabsProvider prefabsProvider, 
        Wallet wallet, GlobalUIHandler globalUIHandler, ReactiveCommand onSceneTransition, SaveServiceProvider saveServiceProvider,
        GlobalSound globalSound, MainMenuLocalizationHandler mainMenuLocalizationHandler)
    {
        await Addressables.InitializeAsync();

        var swipeDetectorOff = new ReactiveCommand<bool>();
        var loadIndicatorUIHandler = new LoadIndicatorUIHandler();
        var blackFrameUIHandler = new BlackFrameUIHandler();
        var darkeningBackgroundFrameUIHandler = new BlackFrameUIHandler();
        var loadScreenUIHandler = new LoadScreenUIHandler();
        var settingsPanelUIHandler = new SettingsPanelUIHandler(mainMenuLocalizationHandler.LanguageChanged, swipeDetectorOff);
        var shopMoneyPanelUIHandler = new ShopMoneyPanelUIHandler(darkeningBackgroundFrameUIHandler, wallet,
            swipeDetectorOff);
        
        await globalUIHandler.Init(loadScreenUIHandler, settingsPanelUIHandler, shopMoneyPanelUIHandler, loadIndicatorUIHandler, blackFrameUIHandler);

        (MainMenuUIProvider, MainMenuUIView, Transform) result =
            await CreateMainMenuUIProvider(wallet, globalUIHandler, darkeningBackgroundFrameUIHandler, mainMenuLocalizationHandler.LanguageChanged, swipeDetectorOff);

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
        GlobalUIHandler globalUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        ReactiveCommand languageChanged, ReactiveCommand<bool> swipeDetectorOff)
    {
        MainMenuCanvasAssetProvider menuCanvasAssetProvider = new MainMenuCanvasAssetProvider();
        MainMenuUIView mainMenuUIView = await menuCanvasAssetProvider.CreateAsset();
        mainMenuUIView.GetComponent<Canvas>().worldCamera = Camera.main;
        var mainMenuUIViewTransform = mainMenuUIView.transform;
        var playStoryPanelHandler = new PlayStoryPanelHandler(darkeningBackgroundFrameUIHandler, languageChanged);
        var settingsPanelButtonUIHandler = new SettingsPanelButtonUIHandler(globalUIHandler.GlobalUITransforn, globalUIHandler.SettingsPanelUIHandler,
            darkeningBackgroundFrameUIHandler, globalUIHandler.LoadIndicatorUIHandler);
        var shopMoneyButtonsUIHandler = new ShopMoneyButtonsUIHandler(globalUIHandler.LoadIndicatorUIHandler, wallet, globalUIHandler.ShopMoneyPanelUIHandler, globalUIHandler.GlobalUITransforn);

        var myScrollHandler = new MyScrollHandler(mainMenuUIView.MyScrollUIView, languageChanged, swipeDetectorOff);
        var confirmedPanelUIHandler = new ConfirmedPanelUIHandler(globalUIHandler.LoadIndicatorUIHandler, darkeningBackgroundFrameUIHandler, mainMenuUIViewTransform);
        var bottomPanelUIHandler = new BottomPanelUIHandler(confirmedPanelUIHandler,
            new AdvertisingButtonUIHandler(globalUIHandler.LoadIndicatorUIHandler, darkeningBackgroundFrameUIHandler, wallet, mainMenuUIViewTransform),
            mainMenuUIViewTransform, languageChanged);
        
        MainMenuUIProvider mainMenuUIProvider = new MainMenuUIProvider(darkeningBackgroundFrameUIHandler,
            playStoryPanelHandler, settingsPanelButtonUIHandler, globalUIHandler.SettingsPanelUIHandler, globalUIHandler.ShopMoneyPanelUIHandler,
            shopMoneyButtonsUIHandler, confirmedPanelUIHandler, globalUIHandler,bottomPanelUIHandler, myScrollHandler);
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
            mainMenuUIViewTransform, onSceneTransition, saveServiceProvider);
    }

    private async UniTask<StoriesProvider> CreateStoriesProvider()
    {
        var storiesProviderAssetProvider = new StoriesProviderAssetProvider();
        return await storiesProviderAssetProvider.Load();
    }
}