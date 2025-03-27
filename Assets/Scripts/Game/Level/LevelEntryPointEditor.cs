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
    private SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> _switchToAnotherNodeGraphEvent;
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
            CharacterViewer.Dispose();
            GameSeriesHandler.Dispose();
            _levelUIProvider.Dispose();
            EventSystem.gameObject.SetActive(false);
            Save();
        });
        
        // var handle = Addressables.LoadAssetAsync<SeriaPartNodeGraph>("8 SeriaPartNodeGraph 2.1");
        // _test1 = await handle.Task;
    }

    public void Init()
    {
        if (LoadSaveData == true)
        {
            _saveData = SaveServiceProvider.SaveData;
            _storyData = _saveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            TestMonets = _saveData.Monets;
            TestHearts = _saveData.Hearts;
            _wallet = new Wallet(_saveData);
            GameStatsCustodian.Init(_storyData.Stats);
            _levelSound.Init(_saveData.SoundIsOn);
        }
        else
        {
            _wallet = new Wallet(TestMonets, TestHearts);
            GameStatsCustodian.Init();
            _levelSound.Init();
        }
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();
        _onSceneTransition = new ReactiveCommand();
        _switchToNextNodeEvent = new SwitchToNextNodeEvent();
        _switchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        _disableNodesContentEvent = new DisableNodesContentEvent();
        InitLevelUIProvider();

        ViewerCreatorEditMode viewerCreatorEditMode = new ViewerCreatorEditMode(_spriteViewerPrefab);
        CharacterViewer.Construct(_disableNodesContentEvent, viewerCreatorEditMode);
        InitWardrobeCharacterViewer(viewerCreatorEditMode);
        
        SpriteRendererCreator spriteRendererCreator = new SpriteRendererCreatorEditor(_spriteRendererPrefab);
        InitBackground(spriteRendererCreator);
        
        _nodeGraphInitializer = new NodeGraphInitializer(Characters, Background.GetBackgroundContent, Background, _levelUIProvider,
            CharacterViewer, _wardrobeCharacterViewer, CustomizableCharacter, _levelSound, GameStatsCustodian, _wallet,
            _switchToNextNodeEvent, _switchToAnotherNodeGraphEvent, _disableNodesContentEvent, SwitchToNextSeriaEvent);

        if (_saveData == null)
        {
            GameSeriesHandler.Construct(_nodeGraphInitializer, SwitchToNextSeriaEvent);

        }
        else
        {
            GameSeriesHandler.Construct(_nodeGraphInitializer, SwitchToNextSeriaEvent,
                _storyData.CurrentNodeGraphIndex, _storyData.CurrentNodeIndex);
        }

        
    }

    private void OnApplicationQuit()
    {
        CharacterViewer.Dispose();
        GameSeriesHandler.Dispose();
        _levelUIProvider.Dispose();
        EventSystem.gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        if (LoadSaveData == true)
        {
            _storyData.CurrentNodeGraphIndex = GameSeriesHandler.CurrentNodeGraphIndex;
            _storyData.CurrentNodeIndex = GameSeriesHandler.CurrentNodeIndex;
            _storyData.Stats = GameStatsCustodian.GetSaveStatsToSave();
            _saveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = _storyData;
            SaveServiceProvider.SaveService.Save(_saveData);
        }
    }
    private void InitLevelUIProvider()
    {
        CustomizationCharacterPanelUI customizationCharacterPanelUI;
        customizationCharacterPanelUI = LevelUIView.CustomizationCharacterPanelUI;
        _levelUIProvider = new LevelUIProvider(LevelUIView, _wallet, _onSceneTransition, _disableNodesContentEvent, _switchToNextNodeEvent, customizationCharacterPanelUI);
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
        Background.Construct(_disableNodesContentEvent, CharacterViewer, spriteRendererCreator, _wardrobeBackground);

    }
}