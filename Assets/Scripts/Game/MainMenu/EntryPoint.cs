using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Zenject;

public class EntryPoint: MonoBehaviour
{
    [SerializeField, Expandable] private StoriesProvider _storiesProvider;
    [SerializeField] private int _targetFrameRate = 60;
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
    private BackgroundData _iconsData;

    [Inject]
    private void Construct(SaveServiceProvider saveServiceProvider, PrefabsProvider prefabsProvider,
        GlobalSound globalSound, GlobalUIHandler globalUIHandler, PanelsLocalizationHandler panelsLocalizationHandler)
    {
        _saveServiceProvider = saveServiceProvider;
        _globalSound = globalSound;
        _prefabsProvider = prefabsProvider;
        _onSceneTransition = new ReactiveCommand();
        if (ProjectContext.Instance.Container.HasBinding<Wallet>())
        {
            _wallet = ProjectContext.Instance.Container.Resolve<Wallet>();
        }

        _globalUIHandler = globalUIHandler;
        _panelsLocalizationHandler = panelsLocalizationHandler;
    }

    private async void Awake()
    {
        Application.targetFrameRate = _targetFrameRate;
        StartConfigAssetProvider startConfigAssetProvider = new StartConfigAssetProvider();
        StartConfig sc = await startConfigAssetProvider.Load();
        _saveServiceProvider.LoadSaveData(sc);
        if (ProjectContext.Instance.Container.HasBinding<Wallet>() == false)
        {
            _wallet = new Wallet(_saveServiceProvider.SaveData);
            ProjectContext.Instance.Container.Bind<Wallet>().FromInstance(_wallet).AsSingle();
        }

        IconsDataAssetProvider iconsDataAssetProvider = new IconsDataAssetProvider();
        _iconsData = await iconsDataAssetProvider.LoadIconsDataAsset();
        _appStarter = new AppStarter();
        (StoriesProvider, MainMenuUIProvider, LevelLoader) result =
            await _appStarter.StartApp(_prefabsProvider, _wallet, _globalUIHandler, _onSceneTransition,
                _saveServiceProvider, _globalSound, _panelsLocalizationHandler, sc, _iconsData);

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
        _saveServiceProvider.SaveFromMainMenu(_wallet, _globalSound, _storiesProvider,
            _panelsLocalizationHandler, _mainMenuUIProvider);
        _wallet.Shutdown();
        _mainMenuUIProvider?.Shutdown();
        if (_iconsData != null)
        {
            Addressables.Release(_iconsData);
        }
    }

    private void OnApplicationQuit()
    {
        Shutdown();
        _globalSound.ShutdownFromMenu();
        _globalUIHandler.Shutdown();
    }
}