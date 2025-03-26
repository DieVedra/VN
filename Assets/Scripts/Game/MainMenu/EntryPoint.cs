
using System;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class EntryPoint: MonoBehaviour
{
    [SerializeField] private MainMenuUIView _mainMenuUIView;
    [SerializeField, Expandable] private StoriesProvider _storiesProvider;
    private MainMenuUIProvider _mainMenuUIProvider;
    private LevelLoader _levelLoader;
    private AppStarter _appStarter;
    private Wallet _wallet;
    private AdvertisingHandler _advertisingHandler;
    private ReactiveCommand _onSceneTransition;
    private SaveData _saveData;
    private SaveService _saveService;
    private void Awake()
    {
        LoadSaveData();
        _storiesProvider.Init(_saveData);
        _onSceneTransition = new ReactiveCommand();
        _wallet = new Wallet(_saveData);
        _advertisingHandler = new AdvertisingHandler(_wallet);
        _mainMenuUIProvider = new MainMenuUIProvider(_mainMenuUIView, _wallet, _advertisingHandler);
        _appStarter = new AppStarter(_mainMenuUIProvider);
        _levelLoader = new LevelLoader(_storiesProvider, _mainMenuUIProvider.GetBlackFrameUIHandler(), _mainMenuUIProvider.GetLoadScreenHandler(),
            _mainMenuUIProvider.GetLoadIndicatorUIHandler(), _mainMenuUIView.transform, _onSceneTransition);
        

        _onSceneTransition.Subscribe(_ =>
        {
            SaveProgress();
            _wallet.Dispose();
            _storiesProvider.Dispose();
        });
    }

    private async void Start()
    {
        await _appStarter.StartApp(_storiesProvider, _levelLoader, _wallet);
        
        
    }

    private void OnApplicationQuit()
    {
        SaveProgress();
    }

    private void LoadSaveData()
    {
        _saveService = new SaveService();
        SaveServiceProvider.SaveService = _saveService;

        _saveData = _saveService.LoadData();
        if (_saveData == null)
        {
            _saveData = new SaveData {StoryDatas = _storiesProvider.GetStoryDatas()};
        }

        SaveServiceProvider.SaveData = _saveData;
    }

    private void SaveProgress()
    {
        _saveService.Save(_saveData);
    }
}