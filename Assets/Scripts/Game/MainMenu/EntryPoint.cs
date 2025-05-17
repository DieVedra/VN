
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
    private LoadScreenUIHandler _loadScreenUIHandler;

    [Inject]
    private void Construct(DiContainer container, SaveServiceProvider saveServiceProvider, PrefabsProvider prefabsProvider,
        GlobalSound globalSound, LoadScreenUIHandler loadScreenUIHandler)
    {
        _saveServiceProvider = saveServiceProvider;
        _globalSound = globalSound;
        _prefabsProvider = prefabsProvider;
        _onSceneTransition = new ReactiveCommand();
        LoadSaveData();
        _wallet = new Wallet(_saveData);
        container.Bind<Wallet>().FromInstance(_wallet).AsSingle();
        _loadScreenUIHandler = loadScreenUIHandler;
        _appStarter = new AppStarter();

    }

    private async void Awake()
    { 
        (StoriesProvider, MainMenuUIProvider, LevelLoader) result =
            await _appStarter.StartApp(_prefabsProvider, _wallet, _loadScreenUIHandler, _onSceneTransition, _saveServiceProvider);

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
        _mainMenuUIProvider.Dispose();
    }
    private void OnApplicationQuit()
    {
        Dispose();
        _loadScreenUIHandler.Dispose();
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