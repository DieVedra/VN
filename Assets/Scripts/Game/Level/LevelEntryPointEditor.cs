using UniRx;
using UnityEngine;

public class LevelEntryPointEditor : LevelEntryPoint
{
    [SerializeField] private WardrobeCharacterViewer _wardrobeCharacterViewer;
    [SerializeField] private BackgroundContent _wardrobeBackground;
    
    [SerializeField] private LevelSoundEditMode levelSoundEditMode;
    [SerializeField] private WardrobeSeriaDataProviderEditMode _wardrobeSeriaDataProviderEditMode;

    [Space]
    [SerializeField] private SpriteViewer _spriteViewerPrefab;

    [SerializeField] private SpriteRenderer _spriteRendererPrefab;
    [SerializeField] private CharacterProviderEditMode characterProviderEditMode;
    [SerializeField] private GameSeriesHandlerEditorMode _gameSeriesHandlerEditorMode;
    [Space]
    [SerializeField] private bool _initializeInEditMode;

    public bool InitializeInEditMode => _initializeInEditMode;

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
        if (LoadSaveData == true)
        {
            SaveData = SaveServiceProvider.SaveData;
            StoryData = SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            TestMonets = SaveData.Monets;
            TestHearts = SaveData.Hearts;
            Wallet = new Wallet(SaveData);
            GameStatsCustodian.Init(StoryData.Stats);
            characterProviderEditMode.CustomizableCharacter.Construct(StoryData.WardrobeSaveData);//?
        }
        else
        {
            Wallet = new Wallet(TestMonets, TestHearts);
            GameStatsCustodian.Init();
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
        
        SpriteRendererCreator spriteRendererCreator = new SpriteRendererCreatorEditor(_spriteRendererPrefab);
        InitBackground(spriteRendererCreator, _wardrobeBackground);
        
        NodeGraphInitializer = new NodeGraphInitializer(characterProviderEditMode.GetCharacters(), Background.GetBackgroundContent, Background, LevelUIProvider,
            CharacterViewer, _wardrobeCharacterViewer, characterProviderEditMode.CustomizableCharacter, _wardrobeSeriaDataProviderEditMode, levelSoundEditMode, GameStatsCustodian, Wallet,
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
            StoryData.Stats = GameStatsCustodian.GetSaveStatsToSave();
            StoryData.CurrentAudioClipIndex = levelSoundEditMode.CurrentMusicClipIndex;
            StoryData.LowPassEffectIsOn = levelSoundEditMode.AudioEffectsCustodian.LowPassEffectIsOn;
            
            StoryData.BackgroundSaveData = Background.GetBackgroundSaveData();

            StoryData.WardrobeSaveData = characterProviderEditMode.CustomizableCharacter.GetWardrobeSaveData();


            SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = StoryData;
            SaveServiceProvider.SaveService.Save(SaveData);
        }
    }
    private void InitLevelUIProvider()
    {
        CustomizationCharacterPanelUI customizationCharacterPanelUI;
        customizationCharacterPanelUI = LevelUIView.CustomizationCharacterPanelUI;
        LevelUIProvider = new LevelUIProvider(LevelUIView, Wallet, OnSceneTransition, DisableNodesContentEvent, SwitchToNextNodeEvent, customizationCharacterPanelUI);
    }
    protected override void InitWardrobeCharacterViewer(ViewerCreator viewerCreatorEditMode)
    {

        if (Application.isPlaying)
        {
            Destroy(_wardrobeCharacterViewer.gameObject);

            _wardrobeCharacterViewer =
                PrefabsProvider.WardrobeCharacterViewerAssetProvider.CreateWardrobeCharacterViewer(transform);
            _wardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorEditMode);
            PrefabsProvider.SpriteViewerAssetProvider.UnloadAsset();
        }
        else
        {
            _wardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorEditMode);
        }
    }

    protected override void InitGlobalSound()
    {
        if (LoadSaveData == true)
        {
            levelSoundEditMode.Construct(SaveData.SoundIsOn);
        }
        else
        {
            levelSoundEditMode.Construct(true);
        }
    }
}