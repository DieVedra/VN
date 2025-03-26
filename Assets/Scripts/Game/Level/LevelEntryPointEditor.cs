

using UniRx;
using UnityEngine;

public class LevelEntryPointEditor : LevelEntryPoint
{
    [SerializeField] private WardrobeCharacterViewer _wardrobeCharacterViewer;
    [SerializeField] private BackgroundContent _wardrobeBackground;
    [SerializeField] private LevelSound _levelSound;

    [Space]
    [SerializeField] private SpriteViewer _spriteViewerPrefab;
    [SerializeField] private SpriteRenderer _spriteRendererPrefab;

    [Space]
    [SerializeField] private bool _initializeInEditMode;
    private SwitchToNextNodeEvent _switchToNextNodeEvent;
    private SwitchToAnotherNodeGraphEvent _switchToAnotherNodeGraphEvent;
    private DisableNodesContentEvent _disableNodesContentEvent;

    private NodeGraphInitializer _nodeGraphInitializer;
    private LevelUIProvider _levelUIProvider;
    private Wallet _wallet;
    private SaveData _saveData;
    private StoryData _storyData;
    private ReactiveCommand _onSceneTransition;

    public bool InitializeInEditMode => _initializeInEditMode;

    private async void Awake()
    {
        if (PrefabsProvider.IsInitialized == false)
        {
            await PrefabsProvider.Init();
        }
        Init();
        _onSceneTransition.Subscribe(_ =>
        {
            _characterViewer.Dispose();
            _nodeGraphsHandler.Dispose();
            _levelUIProvider.Dispose();
            _eventSystem.gameObject.SetActive(false);
            Save();
        });
        
        // var handle = Addressables.LoadAssetAsync<LevelPartNodeGraph>("8 LevelPartNodeGraph 2.1");
        // _test1 = await handle.Task;
    }

    public void Init()
    {
        if (_loadSaveData == true)
        {
            _saveData = SaveServiceProvider.SaveData;
            _storyData = _saveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            _testMonets = _saveData.Monets;
            _testHearts = _saveData.Hearts;
            _wallet = new Wallet(_saveData);
            _gameStatsCustodian.Init(_storyData.Stats);
            _levelSound.Init(_saveData.SoundIsOn);
        }
        else
        {
            _wallet = new Wallet(_testMonets, _testHearts);
            _gameStatsCustodian.Init();
            _levelSound.Init();
        }
        _onSceneTransition = new ReactiveCommand();
        _switchToNextNodeEvent = new SwitchToNextNodeEvent();
        _switchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent();
        _disableNodesContentEvent = new DisableNodesContentEvent();
        InitLevelUIProvider();

        ViewerCreatorEditMode viewerCreatorEditMode = new ViewerCreatorEditMode(_spriteViewerPrefab);
        _characterViewer.Construct(_disableNodesContentEvent, viewerCreatorEditMode);
        InitWardrobeCharacterViewer(viewerCreatorEditMode);
        
        SpriteRendererCreator spriteRendererCreator = new SpriteRendererCreatorEditor(_spriteRendererPrefab);
        InitBackground(spriteRendererCreator);
        
        _nodeGraphInitializer = new NodeGraphInitializer(_characters, _background.GetBackgroundContent, _background, _levelUIProvider,
            _characterViewer, _wardrobeCharacterViewer, _customizableCharacter, _levelSound, _gameStatsCustodian, _wallet,
            _switchToNextNodeEvent, _switchToAnotherNodeGraphEvent, _disableNodesContentEvent);

        if (_saveData == null)
        {
            _nodeGraphsHandler.Init(_nodeGraphInitializer, _switchToNextNodeEvent, _switchToAnotherNodeGraphEvent);

        }
        else
        {
            _nodeGraphsHandler.Init(_nodeGraphInitializer, _switchToNextNodeEvent, _switchToAnotherNodeGraphEvent,
                _storyData.CurrentNodeGraphIndex, _storyData.CurrentNodeIndex);
        }

        
    }

    private void OnApplicationQuit()
    {
        _characterViewer.Dispose();
        _nodeGraphsHandler.Dispose();
        _levelUIProvider.Dispose();
        _eventSystem.gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        if (_loadSaveData == true)
        {
            _storyData.CurrentNodeGraphIndex = _nodeGraphsHandler.CurrentNodeGraphIndex;
            _storyData.CurrentNodeIndex = _nodeGraphsHandler.CurrentNodeIndex;
            _storyData.Stats = _gameStatsCustodian.GetSaveStatsToSave();
            _saveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = _storyData;
            SaveServiceProvider.SaveService.Save(_saveData);
        }
    }
    private void InitLevelUIProvider()
    {
        CustomizationCharacterPanelUI customizationCharacterPanelUI;
        customizationCharacterPanelUI = _levelUIView.CustomizationCharacterPanelUI;
        _levelUIProvider = new LevelUIProvider(_levelUIView, _wallet, _onSceneTransition, _disableNodesContentEvent, _switchToNextNodeEvent, customizationCharacterPanelUI);
    }
    protected override void InitWardrobeCharacterViewer(ViewerCreator viewerCreatorEditMode)
    {
        // if (_wardrobeCharacterViewer != null)
        // {
        //     Destroy(_wardrobeCharacterViewer.gameObject);
        // }

        if (Application.isPlaying)
        {
            Destroy(_wardrobeCharacterViewer.gameObject);

            _wardrobeCharacterViewer =
                PrefabsProvider.WardrobeCharacterViewerAssetProvider.CreateWardrobeCharacterViewer(transform);
            _wardrobeCharacterViewer.Construct(_disableNodesContentEvent, viewerCreatorEditMode);
            PrefabsProvider.SpriteViewerAssetProvider.UnloadAsset();
        }
        else
        {
            _wardrobeCharacterViewer.Construct(_disableNodesContentEvent, viewerCreatorEditMode);
        }
    }

    protected override void InitBackground(SpriteRendererCreator spriteRendererCreator)
    {
        _background.Construct(_disableNodesContentEvent, _characterViewer, spriteRendererCreator, _wardrobeBackground);

    }
}