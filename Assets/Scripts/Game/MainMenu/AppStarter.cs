using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AppStarter
{
    public async UniTask<(StoriesProvider, MainMenuUIProvider, LevelLoader)> StartApp(PrefabsProvider prefabsProvider, 
        Wallet wallet, GlobalUIHandler globalUIHandler, ReactiveCommand onSceneTransition, SaveServiceProvider saveServiceProvider,
        GlobalSound globalSound, PanelsLocalizationHandler panelsLocalizationHandler)
    {
        await Addressables.InitializeAsync();

        LoadIndicatorUIHandler loadIndicatorUIHandler;
        if (globalUIHandler.LoadIndicatorUIHandler == null)
        {
            loadIndicatorUIHandler = new LoadIndicatorUIHandler();
        }
        else
        {
            loadIndicatorUIHandler = globalUIHandler.LoadIndicatorUIHandler;
        }

        BlackFrameUIHandler blackFrameUIHandler;
        if (globalUIHandler.BlackFrameUIHandler == null)
        {
            blackFrameUIHandler = new BlackFrameUIHandler();
        }
        else
        {
            blackFrameUIHandler = globalUIHandler.BlackFrameUIHandler;
        }

        var darkeningBackgroundFrameUIHandler = new BlackFrameUIHandler();
        
        LoadScreenUIHandler loadScreenUIHandler;
        if (globalUIHandler.LoadScreenUIHandler == null)
        {
            loadScreenUIHandler = new LoadScreenUIHandler();
        }
        else
        {
            loadScreenUIHandler = globalUIHandler.LoadScreenUIHandler;
        }

        ReactiveCommand<bool> swipeDetectorOff = null;
        var storiesProvider = await CreateStoriesProvider();

        SettingsPanelUIHandler settingsPanelUIHandler;
        if (globalUIHandler.SettingsPanelUIHandler == null)
        {
            swipeDetectorOff = new ReactiveCommand<bool>();
            settingsPanelUIHandler = new SettingsPanelUIHandler(panelsLocalizationHandler.LanguageChanged, swipeDetectorOff);
        }
        else
        {
            settingsPanelUIHandler = globalUIHandler.SettingsPanelUIHandler;
            swipeDetectorOff = globalUIHandler.SettingsPanelUIHandler.SwipeDetectorOffReactiveCommand;
        }

        ShopMoneyPanelUIHandler shopMoneyPanelUIHandler;
        if (globalUIHandler.ShopMoneyPanelUIHandler == null)
        {
            shopMoneyPanelUIHandler = new ShopMoneyPanelUIHandler(loadIndicatorUIHandler, wallet,
                swipeDetectorOff);
        }
        else
        {
            shopMoneyPanelUIHandler = globalUIHandler.ShopMoneyPanelUIHandler;
            swipeDetectorOff = globalUIHandler.ShopMoneyPanelUIHandler.SwipeDetectorOff;
        }

        await globalUIHandler.Init(loadScreenUIHandler, settingsPanelUIHandler, shopMoneyPanelUIHandler, loadIndicatorUIHandler, blackFrameUIHandler);

        (MainMenuUIProvider, MainMenuUIView, Transform) result =
            await CreateMainMenuUIProvider(wallet, globalUIHandler, darkeningBackgroundFrameUIHandler, panelsLocalizationHandler.LanguageChanged,
                swipeDetectorOff);
        MainMenuUIProvider mainMenuUIProvider = result.Item1;
        MainMenuUIView mainMenuUIView = result.Item2;
        Transform tr = result.Item3;
        
        panelsLocalizationHandler.SetPanelsLocalizableContentFromMainMenu(ListExtensions.MergeIReadOnlyLists(
            mainMenuUIProvider.GetLocalizableContent(),
                storiesProvider.GetLocalizableContent()));
        if (loadScreenUIHandler.IsStarted == false)
        {
            await panelsLocalizationHandler.Init(saveServiceProvider.SaveData);
        }
        panelsLocalizationHandler.SetLanguagePanelsAndMenuStory();
        panelsLocalizationHandler.SubscribeChangeLanguage();
        if (loadScreenUIHandler.IsStarted == false)
        {
            loadScreenUIHandler.ShowOnStart();
        }

        globalSound.SetGlobalSoundData(await new GlobalAudioAssetProvider().LoadGlobalAudioAsset());
        int startIndexStory = 0;
        if (saveServiceProvider.SaveHasBeenLoaded)
        {
            globalSound.Construct(saveServiceProvider.SaveData.SoundStatus);
            startIndexStory = storiesProvider.GetIndexByName(saveServiceProvider.SaveData.NameStartStory);
        }
        else
        {
            globalSound.Construct(true);
        }

        var levelLoader = LevelLoaderCreate(mainMenuUIProvider, onSceneTransition, saveServiceProvider, tr, storiesProvider);




        await prefabsProvider.Init();
        mainMenuUIView.gameObject.SetActive(true);
        await InitMainMenuUI(globalSound.SoundStatus, panelsLocalizationHandler, levelLoader, mainMenuUIProvider, mainMenuUIView, tr,
            storiesProvider, startIndexStory);
        await mainMenuUIProvider.MyScrollHandler.Construct(storiesProvider.Stories, mainMenuUIProvider.PlayStoryPanelHandler,
            levelLoader, startIndexStory);


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
        var playStoryPanelHandler = new PlayStoryPanelHandler(darkeningBackgroundFrameUIHandler);
        var settingsPanelButtonUIHandler = new SettingsPanelButtonUIHandler(globalUIHandler.GlobalUITransforn, globalUIHandler.SettingsPanelUIHandler,
            globalUIHandler.LoadIndicatorUIHandler);
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

    private async UniTask InitMainMenuUI(IReactiveProperty<bool> soundStatus, ILocalizationChanger localizationChanger, LevelLoader levelLoader, 
        MainMenuUIProvider mainMenuUIProvider,
        MainMenuUIView mainMenuUIView, Transform mainMenuUIViewTransform, StoriesProvider storiesProvider, int startIndexStory)
    {
        await mainMenuUIProvider.DarkeningBackgroundFrameUIHandler.Init(mainMenuUIViewTransform);
        await mainMenuUIProvider.PlayStoryPanelHandler.Init(levelLoader, mainMenuUIViewTransform);
        await mainMenuUIProvider.MyScrollHandler.Construct(storiesProvider.Stories, mainMenuUIProvider.PlayStoryPanelHandler,
            levelLoader, startIndexStory);
        mainMenuUIProvider.SettingsButtonUIHandler.BaseInit(mainMenuUIView.SettingsButtonView, mainMenuUIProvider.DarkeningBackgroundFrameUIHandler,
            soundStatus, localizationChanger);
        mainMenuUIProvider.SettingsButtonUIHandler.InitInMenu();
        
        mainMenuUIProvider.ShopButtonsUIHandler.Init(mainMenuUIProvider.DarkeningBackgroundFrameUIHandler, mainMenuUIView.MonetPanelView,
            mainMenuUIView.HeartsPanelView);
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