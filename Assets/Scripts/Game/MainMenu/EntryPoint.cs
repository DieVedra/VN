
using NaughtyAttributes;
using UniRx;
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

    [Inject]
    private void Construct(SaveServiceProvider saveServiceProvider, PrefabsProvider prefabsProvider,
        GlobalSound globalSound, GlobalUIHandler globalUIHandler, PanelsLocalizationHandler panelsLocalizationHandler)
    {
        _saveServiceProvider = saveServiceProvider;
        _globalSound = globalSound;
        _prefabsProvider = prefabsProvider;
        _onSceneTransition = new ReactiveCommand();
        _saveServiceProvider.LoadSaveData();
        if (ProjectContext.Instance.Container.HasBinding<Wallet>() == false)
        {
            _wallet = new Wallet(_saveServiceProvider.SaveData);
            ProjectContext.Instance.Container.Bind<Wallet>().FromInstance(_wallet).AsSingle();
        }
        else
        {
            _wallet = ProjectContext.Instance.Container.Resolve<Wallet>();
        }

        _globalUIHandler = globalUIHandler;
        _appStarter = new AppStarter();
        _panelsLocalizationHandler = panelsLocalizationHandler;
    }

    private async void Awake()
    {
        (StoriesProvider, MainMenuUIProvider, LevelLoader) result =
            await _appStarter.StartApp(_prefabsProvider, _wallet, _globalUIHandler, _onSceneTransition,
                _saveServiceProvider, _globalSound, _panelsLocalizationHandler);

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
            Dispose();
        });
    }

    private void Dispose()
    {
        _panelsLocalizationHandler.UnsubscribeChangeLanguage();
        _saveServiceProvider.SaveProgress(_wallet, _globalSound, _storiesProvider,
            _panelsLocalizationHandler, _mainMenuUIProvider);
        _wallet.Dispose();
        _storiesProvider?.Shutdown();
        _mainMenuUIProvider?.Dispose();
    }

    private void OnApplicationQuit()
    {
        Dispose();
        _globalUIHandler.Dispose();
    }
}