using NaughtyAttributes;
using UniRx;
using Unity.Services.Analytics;
using UnityEngine;
using Zenject;

public class EntryPoint: MonoBehaviour
{
    [SerializeField, Expandable] private StoriesProvider _storiesProvider;
    private MainMenuUIProvider _mainMenuUIProvider;
    private LevelLoader _levelLoader;
    private AppStarter _appStarter;
    private Wallet _wallet;
    private ReactiveCommand _onSceneTransition;
    private SaveServiceProvider _saveServiceProvider;
    private GlobalSound _globalSound;
    private PrefabsProvider _prefabsProvider;
    private GlobalUIHandler _globalUIHandler;
    private PanelsLocalizationHandler _panelsLocalizationHandler;
    private IconsUISpriteAtlasAssetProvider _iconsUISpriteAtlasAssetProvider;


    [Inject]
    private void Construct(PrefabsProvider prefabsProvider,
        GlobalSound globalSound, GlobalUIHandler globalUIHandler, PanelsLocalizationHandler panelsLocalizationHandler)
    {
        _globalSound = globalSound;
        _prefabsProvider = prefabsProvider;
        _onSceneTransition = new ReactiveCommand();
        if (ProjectContext.Instance.Container.HasBinding<Wallet>())
        {
            _wallet = ProjectContext.Instance.Container.Resolve<Wallet>();
        }
        if (ProjectContext.Instance.Container.HasBinding<SaveServiceProvider>())
        {
            _saveServiceProvider = ProjectContext.Instance.Container.Resolve<SaveServiceProvider>();
        }
        
        _globalUIHandler = globalUIHandler;
        _panelsLocalizationHandler = panelsLocalizationHandler;
    }

    private async void Awake()
    {
        StartConfigAssetProvider startConfigAssetProvider = new StartConfigAssetProvider();
        StartConfig sc = await startConfigAssetProvider.Load();

        if (ProjectContext.Instance.Container.HasBinding<SaveServiceProvider>() == false)
        {
            _saveServiceProvider = new SaveServiceProvider(sc);
            ProjectContext.Instance.Container.Bind<SaveServiceProvider>().FromInstance(_saveServiceProvider).AsSingle();
        }
        await _saveServiceProvider.SaveService.Construct();
        await _saveServiceProvider.LoadSaveData();
        if (ProjectContext.Instance.Container.HasBinding<Wallet>() == false)
        {
            _wallet = new Wallet(_saveServiceProvider.SaveData);
            ProjectContext.Instance.Container.Bind<Wallet>().FromInstance(_wallet).AsSingle();
        }
        _iconsUISpriteAtlasAssetProvider = new IconsUISpriteAtlasAssetProvider();
        _appStarter = new AppStarter();
        (StoriesProvider, MainMenuUIProvider, LevelLoader) result =
            await _appStarter.StartApp(_prefabsProvider, _wallet, _globalUIHandler, _onSceneTransition,
                _saveServiceProvider, _globalSound, _panelsLocalizationHandler, sc, _iconsUISpriteAtlasAssetProvider);

        _storiesProvider = result.Item1;
        _mainMenuUIProvider = result.Item2;
        _levelLoader = result.Item3;
        _saveServiceProvider.TrySetLanguageLocalizationKey(_panelsLocalizationHandler.GetKey);

        _storiesProvider.Init(_saveServiceProvider.SaveData);
        _saveServiceProvider.TrySetStoryDatas(_storiesProvider);
        _onSceneTransition.Subscribe(_ =>
        {
            var storyName = _mainMenuUIProvider.PlayStoryPanelHandler.GetCurrentStoryName;
            _saveServiceProvider.TrySetStartStory(storyName);
            // _saveServiceProvider.CurrentStoryIndex = _storiesProvider.GetIndexByName(storyName);
            _saveServiceProvider.CurrentStoryKey = storyName;
            Shutdown();
        });
    }

    private void Shutdown()
    {
        _panelsLocalizationHandler.UnsubscribeChangeLanguage();
        _wallet.Shutdown();
        _iconsUISpriteAtlasAssetProvider?.Release();
        _mainMenuUIProvider?.Shutdown();
    }

    private async void OnApplicationQuit()
    {
        await _saveServiceProvider.SaveFromMainMenu();
        _globalSound.ShutdownFromMenu();
        _globalUIHandler.Shutdown();
        AnalyticsService.Instance.Flush();
        Shutdown();
    }
    // [Button()]
    // public void TestCrash()
    // {
    //     Utils.ForceCrash(ForcedCrashCategory.FatalError);
    // }
}