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
    [SerializeField] private GlobalAudioData _globalAudioData;
    [Space]
    [SerializeField] private bool _initializeInEditMode;
    // private SwitchToNextNodeEvent SwitchToNextNodeEvent;
    // private SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> SwitchToAnotherNodeGraphEvent;
    // private DisableNodesContentEvent DisableNodesContentEvent;

    // private NodeGraphInitializer NodeGraphInitializer;
    // private LevelUIProvider LevelUIProvider;
    // private Wallet Wallet;
    // private SaveData SaveData;
    // private StoryData StoryData;
    // private ReactiveCommand OnSceneTransition;

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
            CharacterViewer.Dispose();
            GameSeriesHandler.Dispose();
            LevelUIProvider.Dispose();
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
            SaveData = SaveServiceProvider.SaveData;
            StoryData = SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            TestMonets = SaveData.Monets;
            TestHearts = SaveData.Hearts;
            Wallet = new Wallet(SaveData);
            GameStatsCustodian.Init(StoryData.Stats);
            _levelSound.Init(SaveData.SoundIsOn);
            _levelSound.SetGlobalSoundData(_globalAudioData);
        }
        else
        {
            Wallet = new Wallet(TestMonets, TestHearts);
            GameStatsCustodian.Init();
            _levelSound.Init(true);
            _levelSound.SetGlobalSoundData(_globalAudioData);
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
        
        NodeGraphInitializer = new NodeGraphInitializer(Characters, Background.GetBackgroundContent, Background, LevelUIProvider,
            CharacterViewer, _wardrobeCharacterViewer, CustomizableCharacter, _levelSound, GameStatsCustodian, Wallet,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent);

        if (SaveData == null)
        {
            GameSeriesHandler.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent);

        }
        else
        {
            GameSeriesHandler.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent, 
                StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeIndex);
        }

        
    }

    private void OnApplicationQuit()
    {
        CharacterViewer.Dispose();
        GameSeriesHandler.Dispose();
        LevelUIProvider.Dispose();
        EventSystem.gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        if (LoadSaveData == true)
        {
            StoryData.CurrentNodeGraphIndex = GameSeriesHandler.CurrentNodeGraphIndex;
            StoryData.CurrentNodeIndex = GameSeriesHandler.CurrentNodeIndex;
            StoryData.Stats = GameStatsCustodian.GetSaveStatsToSave();
            StoryData.BackgroundSaveData = Background.GetBackgroundSaveData();
            StoryData.CurrentAudioClipIndex = _levelSound.CurrentMusicClipIndex;
            StoryData.LowPassEffectIsOn = _levelSound.AudioEffectsCustodian.LowPassEffectIsOn;
            
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
        // if (_wardrobeCharacterViewer != null)
        // {
        //     Destroy(_wardrobeCharacterViewer.gameObject);
        // }

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
            _levelSound.Init(SaveData.SoundIsOn);
        }
        else
        {
            _levelSound.Init(true);
        }
        _levelSound.SetGlobalSoundData(_globalAudioData);
    }
}