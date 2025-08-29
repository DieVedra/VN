using System;
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
    private MainMenuLocalizationHandler _mainMenuLocalizationHandler;

    [Inject]
    private void Construct(SaveServiceProvider saveServiceProvider, PrefabsProvider prefabsProvider,
        GlobalSound globalSound, GlobalUIHandler globalUIHandler, MainMenuLocalizationHandler mainMenuLocalizationHandler)
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
        _saveServiceProvider.TrySetLanguageLocalizationKey(_mainMenuLocalizationHandler.GetKey);

        _storiesProvider.Init(_saveServiceProvider.SaveData);
        _saveServiceProvider.TrySetStoryDatas(_storiesProvider);
        _onSceneTransition.Subscribe(_ =>
        {
            _saveServiceProvider.TrySetStartStory(_mainMenuUIProvider.PlayStoryPanelHandler.GetCurrentStoryName);
            Dispose();
        });
    }

    private void Dispose()
    {
        _saveServiceProvider.SaveProgress(_wallet, _globalSound, _storiesProvider,
            _mainMenuLocalizationHandler, _mainMenuUIProvider);
        _wallet.Dispose();
        _storiesProvider?.Dispose();
        _mainMenuUIProvider?.Dispose();
    }
    private void OnApplicationQuit()
    {
        Dispose();
        _globalUIHandler.Dispose();
    }

    // private void LoadSaveData()
    // {
    //     // _saveService = new SaveService(new BinarySave());
    //     // _saveService = new SaveService(new JSonSave());
    //     // _saveServiceProvider.SaveService = _saveService;
    //
    //     var result = _saveService.LoadData();
    //     var aa = _storiesProvider.GetStoryDatas();
    //     if (result.Item1 == false)
    //     {
    //         Debug.Log($"_saveData == null");
    //
    //         _saveData = new SaveData();
    //         _saveData.SoundStatus = true;
    //         _saveData.Monets = 50;
    //         _saveData.Hearts = 5;
    //         _saveData.NameStartStory = aa[0].StoryName;
    //         _saveData.StoryDatas = aa;
    //         // _saveData.LanguageLocalizationKey = _mainMenuLocalizationHandler.GetKey;
    //     }
    //     _saveServiceProvider._saveData = _saveData;
    // }
}