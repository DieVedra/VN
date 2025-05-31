
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
    private SaveData _saveData;
    private SaveService _saveService;
    private SaveServiceProvider _saveServiceProvider;
    private GlobalSound _globalSound;
    private PrefabsProvider _prefabsProvider;
    private GlobalUIHandler _globalUIHandler;
    private MainMenuLocalizationHandler _mainMenuLocalizationHandler;

    [Inject]
    private void Construct(SaveServiceProvider saveServiceProvider, PrefabsProvider prefabsProvider,
        GlobalSound globalSound, GlobalUIHandler globalUIHandler, MainMenuLocalizationHandler mainMenuLocalizationHandler)
    {
        _saveServiceProvider = saveServiceProvider;
        _globalSound = globalSound;
        _prefabsProvider = prefabsProvider;
        _onSceneTransition = new ReactiveCommand();
        LoadSaveData();
        if (ProjectContext.Instance.Container.HasBinding<Wallet>() == false)
        {
            _wallet = new Wallet(_saveData);
            ProjectContext.Instance.Container.Bind<Wallet>().FromInstance(_wallet).AsSingle();
        }
        else
        {
            _wallet = ProjectContext.Instance.Container.Resolve<Wallet>();
        }

        _globalUIHandler = globalUIHandler;
        _appStarter = new AppStarter();
        _mainMenuLocalizationHandler = mainMenuLocalizationHandler;
    }

    private async void Awake()
    {
        (StoriesProvider, MainMenuUIProvider, LevelLoader) result =
            await _appStarter.StartApp(_prefabsProvider, _wallet, _globalUIHandler, _onSceneTransition,
                _saveServiceProvider, _globalSound, _mainMenuLocalizationHandler);

        _storiesProvider = result.Item1;
        _mainMenuUIProvider = result.Item2;
        _levelLoader = result.Item3;
        
        _storiesProvider.Init(_saveData);
        _onSceneTransition.Subscribe(_ =>
        {
            Dispose();
        });
    }

    private void Dispose()
    {
        SaveProgress();
        _wallet.Dispose();
        _storiesProvider?.Dispose();
        _mainMenuUIProvider?.Dispose();
    }
    private void OnApplicationQuit()
    {
        Dispose();
        _globalUIHandler.Dispose();
    }

    private void LoadSaveData()
    {
        _saveService = new SaveService();
        _saveServiceProvider.SaveService = _saveService;

        _saveData = _saveService.LoadData();
        if (_saveData == null)
        {
            _saveData = new SaveData {StoryDatas = _storiesProvider.GetStoryDatas()};
        }

        _saveServiceProvider.SaveData = _saveData;
    }

    private void SaveProgress()
    {
        _saveService.Save(_saveData);
    }
}