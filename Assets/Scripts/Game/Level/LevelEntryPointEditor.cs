using UniRx;
using UnityEngine;
using Zenject;

public class LevelEntryPointEditor : LevelEntryPoint
{
    [SerializeField] private LevelSoundEditMode levelSoundEditMode;
    [SerializeField] private WardrobeSeriaDataProviderEditMode _wardrobeSeriaDataProviderEditMode;
    [SerializeField] private BackgroundEditMode _backgroundEditMode;
    [Space]
    [SerializeField] private SpriteViewer _spriteViewerPrefab;

    [SerializeField] private BlackFrameView _blackFrameView;
    [SerializeField] private CharacterProviderEditMode _characterProviderEditMode;
    [SerializeField] private GameSeriesHandlerEditorMode _gameSeriesHandlerEditorMode;
    [SerializeField] private SeriaGameStatsProviderEditor _seriaGameStatsProviderEditor;
    [SerializeField] private GameStatsViewer _gameStatsViewer;
    [Space]
    [SerializeField] private bool _initializeInEditMode;

    private GlobalUIHandler _globalUIHandler;
    public bool InitializeInEditMode => _initializeInEditMode;

    [Inject]
    private void Construct(PrefabsProvider prefabsProvider, GlobalUIHandler globalUIHandler)
    {
        PrefabsProvider = prefabsProvider;
        _globalUIHandler = globalUIHandler;
    }

    private async void Awake()
    {
        if (PrefabsProvider.IsInitialized == false)
        {
            await PrefabsProvider.Init();
        }
        Init();
        OnSceneTransition.Subscribe(_ =>
        {
            Dispose();
        });
    }

    public void Init()
    {
        _seriaGameStatsProviderEditor.Init();
        _gameStatsViewer.Construct(_seriaGameStatsProviderEditor.GameStatsHandler.Stats);
        if (LoadSaveData == true)
        {
            SaveData = SaveServiceProvider.SaveData;
            StoryData = SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            TestMonets = SaveData.Monets;
            TestHearts = SaveData.Hearts;
            Wallet = new Wallet(SaveData);
            _seriaGameStatsProviderEditor.UpdateAllStatsFromSave(StoryData.Stats);
            _characterProviderEditMode.Construct(StoryData.WardrobeSaveDatas);//?
        }
        else
        {
            _characterProviderEditMode.Construct();
            Wallet = new Wallet(TestMonets, TestHearts);
        }

        InitGlobalSound();
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();
        OnSceneTransition = new ReactiveCommand();
        SwitchToNextNodeEvent = new SwitchToNextNodeEvent();
        SwitchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        DisableNodesContentEvent = new DisableNodesContentEvent();
        InitLevelUIProvider();

        ViewerCreatorEditMode viewerCreatorEditMode = new ViewerCreatorEditMode(_spriteViewerPrefab);
        CharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorEditMode);
        InitWardrobeCharacterViewer(viewerCreatorEditMode);
        
        InitBackground();
        NodeGraphInitializer = new NodeGraphInitializer(_characterProviderEditMode, _backgroundEditMode.GetBackgroundContent, _backgroundEditMode, LevelUIProvider,
            CharacterViewer, WardrobeCharacterViewer, _wardrobeSeriaDataProviderEditMode, levelSoundEditMode, Wallet, _seriaGameStatsProviderEditor,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent);

        if (SaveData == null)
        {
            _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent);
        }
        else
        {
            _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent, 
                StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeIndex);
        }
    }

    private void OnApplicationQuit()
    {
        Dispose();
    }
    protected override void Dispose()
    {
        _gameSeriesHandlerEditorMode.Dispose();
        base.Dispose();
        Save();
    }
    private void Save()
    {
        if (LoadSaveData == true)
        {
            StoryData.CurrentNodeGraphIndex = _gameSeriesHandlerEditorMode.CurrentNodeGraphIndex;
            StoryData.CurrentNodeIndex = _gameSeriesHandlerEditorMode.CurrentNodeIndex;
            StoryData.Stats = _seriaGameStatsProviderEditor.GetAllStatsToSave();
            StoryData.CurrentAudioClipIndex = levelSoundEditMode.CurrentMusicClipIndex;
            StoryData.LowPassEffectIsOn = levelSoundEditMode.AudioEffectsCustodian.LowPassEffectIsOn;
            StoryData.CustomizableCharacterIndex = WardrobeCharacterViewer.CustomizableCharacterIndex;
            StoryData.BackgroundSaveData = _backgroundEditMode.GetBackgroundSaveData();

            StoryData.WardrobeSaveDatas = SaveService.CreateWardrobeSaveDatas(_characterProviderEditMode.CustomizableCharacters);

            SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = StoryData;
            SaveServiceProvider.SaveService.Save(SaveData);
        }
    }
    private void InitLevelUIProvider()
    {
        var customizationCharacterPanelUI = LevelUIView.CustomizationCharacterPanelUI;
        BlackFrameUIHandler blackFrameUIHandler = new BlackFrameUIHandler(_blackFrameView);
        LevelUIProvider = new LevelUIProvider(LevelUIView, blackFrameUIHandler, Wallet, OnSceneTransition, DisableNodesContentEvent,
            SwitchToNextNodeEvent, customizationCharacterPanelUI);
    }
    protected override void InitWardrobeCharacterViewer(ViewerCreator viewerCreatorEditMode)
    {
        WardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorEditMode);
    }

    protected override void InitGlobalSound()
    {
        if (LoadSaveData == true)
        {
            levelSoundEditMode.Construct(SaveData.SoundStatus);
        }
        else
        {
            levelSoundEditMode.Construct(true);
        }
    }
    protected override void InitBackground()
    {
        if (LoadSaveData == true)
        {
            _backgroundEditMode.InitSaveData(StoryData.BackgroundSaveData);
        }
        _backgroundEditMode.Construct(DisableNodesContentEvent, CharacterViewer);
    }
}