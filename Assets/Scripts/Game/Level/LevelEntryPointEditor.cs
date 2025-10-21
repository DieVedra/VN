using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class LevelEntryPointEditor : LevelEntryPoint
{
    [SerializeField] private LevelSoundEditMode levelSoundEditMode;
    [SerializeField] private BackgroundEditMode _backgroundEditMode;
    [Space]
    [SerializeField] private SpriteViewer _spriteViewerPrefab;
    [SerializeField] private WardrobeCharacterViewer _wardrobeCharacterViewer;
    [SerializeField] private BlackFrameView _blackFrameView;
    [SerializeField] private CharacterProviderEditMode _characterProviderEditMode;
    [SerializeField] private GameSeriesHandlerEditorMode _gameSeriesHandlerEditorMode;
    [SerializeField] private SeriaGameStatsProviderEditor _seriaGameStatsProviderEditor;
    [SerializeField] private GameStatsViewer _gameStatsViewer;
    [SerializeField] private PhoneProviderInEditMode _phoneProviderInEditMode;
    [SerializeField] private Wallet _wallet;

    [SerializeField, Space(10f)] private int _testMonets;
    [SerializeField, Space(10f)] private int _testHearts;
    [Space]
    [SerializeField] private bool _initializeInEditMode;

    [SerializeField, HideInInspector] private TestModeEditor _testModeEditor;

    private const int _currentStoryIndex = 0;
    private LevelUIProviderEditMode _levelUIProviderEditMode;
    private ICharacterProvider _characterProvider => _characterProviderEditMode.CharacterProvider;

    // private SaveServiceProvider _getSaveServiceProvider =>
    //     SaveServiceProvider == null ? new SaveServiceProvider() : SaveServiceProvider;
    public bool IsInitializing { get; private set; }
    public bool InitializeInEditMode => _initializeInEditMode;

    private async void Awake()
    {
        PrefabsProvider = new PrefabsProvider();
        await PrefabsProvider.Init();
        Init();
        OnSceneTransitionEvent.Subscribe(Dispose);
    }
    
    public void Init()
    {
        IsInitializing = true;
        DisableNodesContentEvent?.Execute();
        _seriaGameStatsProviderEditor.Init();
        _gameStatsViewer.Construct(_seriaGameStatsProviderEditor.GameStatsHandler.Stats);
        StoryData storyData = null;
        SaveServiceProvider = new SaveServiceProvider();
        if (Application.isPlaying &&  LoadSaveData == true)
        {
            if (SaveServiceProvider.LoadSaveData() == true)
            {
                SaveData = SaveServiceProvider.SaveData;
                _testMonets = SaveData.Monets;
                _testHearts = SaveData.Hearts;
                _wallet = new Wallet(SaveData);
                Debug.Log($"SaveData.Monets {SaveData.Monets}     {SaveData.StoryDatas.Count}");
                
                storyData = SaveData.StoryDatas[_currentStoryIndex];
                Debug.Log($"2");

                
                if (storyData.Contacts.Count > 0)
                {
                    _phoneProviderInEditMode.TrySetSaveData(storyData.Contacts.ToArray());
                }

                if (storyData.Stats.Count > 0)
                {
                    _seriaGameStatsProviderEditor.UpdateAllStatsFromSave(storyData.Stats.ToArray());

                }

                if (storyData.WardrobeSaveDatas.Count > 0)
                {
                    _characterProviderEditMode.Construct(storyData.WardrobeSaveDatas.ToArray());//?
                }
                else
                {
                    _characterProviderEditMode.Construct();

                }
                // if (SaveData.StoryDatas != null)
                // {
                //     Debug.Log($"SaveData.StoryDatas != null");
                //     // _phoneProviderInEditMode.TrySetSaveData(SaveData.Contacts);
                //
                //     // storyData = SaveData.StoryDatas[_currentStoryIndex];
                //     _seriaGameStatsProviderEditor.UpdateAllStatsFromSave(storyData.Stats);
                //     _characterProviderEditMode.Construct(storyData.WardrobeSaveDatas);//?
                // }
                // else
                // {
                //     Debug.Log($"SaveData.StoryDatas == null");
                //
                //     _characterProviderEditMode.Construct();
                // }
            }
            else
            {
                Construct();
            }
        }
        else
        {
            Construct();
        }

        InitGlobalSound();
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();
        OnSceneTransitionEvent = new OnSceneTransitionEvent();
        SwitchToNextNodeEvent = new SwitchToNextNodeEvent();
        SwitchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        
        DisableNodesContentEvent = new DisableNodesContentEvent();
        ViewerCreatorEditMode viewerCreatorEditMode = new ViewerCreatorEditMode(_spriteViewerPrefab);
        CharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorEditMode);
        InitWardrobeCharacterViewer(viewerCreatorEditMode);
        InitBackground();
        _phoneProviderInEditMode.Construct(_characterProviderEditMode.CustomizableCharacterIndexesCustodians);
        InitLevelUIProvider(_phoneProviderInEditMode.PhoneContentProvider);
        NodeGraphInitializer = new NodeGraphInitializer(_backgroundEditMode.GetBackgroundContent, _characterProvider,_backgroundEditMode, _levelUIProviderEditMode,
            CharacterViewer, _wardrobeCharacterViewer, levelSoundEditMode, _wallet, _seriaGameStatsProviderEditor, _phoneProviderInEditMode,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent, new SetLocalizationChangeEvent());

        DisableNodesContentEvent.Execute();


        if (Application.isPlaying)
        {
            if (storyData != null)
            {
                Debug.Log($"StoryData.CurrentSeriaIndex{storyData.CurrentSeriaIndex} StoryData. CurrentNodeGraphIndex{storyData. CurrentNodeGraphIndex} StoryData.CurrentNodeIndex{storyData.CurrentNodeIndex}");
                _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, _characterProvider, SwitchToNextSeriaEvent, new ReactiveProperty<int>(storyData.CurrentSeriaIndex),
                   storyData. CurrentNodeGraphIndex, storyData.CurrentNodeIndex);
                _levelUIProviderEditMode.CurtainUIHandler.CurtainOpens(new CancellationToken()).Forget();
            }
            else if (_testModeEditor.IsTestMode == true)
            {
                _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, _characterProvider, SwitchToNextSeriaEvent, new ReactiveProperty<int>(_testModeEditor.SeriaIndex),
                    _testModeEditor.GraphIndex, _testModeEditor.NodeIndex);
                
                _seriaGameStatsProviderEditor.GameStatsHandler.UpdateStats(_testModeEditor.Stats.ToList());
                _levelUIProviderEditMode.CurtainUIHandler.CurtainOpens(new CancellationToken()).Forget();
            }
            else
            {
                _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, _characterProvider, SwitchToNextSeriaEvent, new ReactiveProperty<int>(DefaultSeriaIndex));
            }
        }
        else
        {
            _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, _characterProvider, SwitchToNextSeriaEvent, new ReactiveProperty<int>(DefaultSeriaIndex));
        }

        void Construct()
        {
            SaveData = SaveServiceProvider.SaveData;
            _characterProviderEditMode.Construct();
            _wallet = new Wallet(_testMonets, _testHearts);
        }
        IsInitializing = false;
    }

    private void OnApplicationQuit()
    {
        Dispose();
    }
    protected override void Dispose()
    {
        _gameSeriesHandlerEditorMode.Dispose();
        _levelUIProviderEditMode.Dispose();
        _wardrobeCharacterViewer.Dispose();
        base.Dispose();
        Save();
    }
    private void Save()
    {
        if (LoadSaveData == true)
        {
            StoryData storyData = new StoryData();
            storyData.CurrentNodeGraphIndex = _gameSeriesHandlerEditorMode.CurrentNodeGraphIndex;
            storyData.CurrentNodeIndex = _gameSeriesHandlerEditorMode.CurrentNodeIndex;
            storyData.CurrentSeriaIndex = _gameSeriesHandlerEditorMode.CurrentSeriaIndex;
            Debug.Log($"SaveData CurrentNodeGraphIndex:{_gameSeriesHandlerEditorMode.CurrentNodeGraphIndex} CurrentNodeIndex:{_gameSeriesHandlerEditorMode.CurrentNodeIndex} CurrentSeriaIndex:{_gameSeriesHandlerEditorMode.CurrentSeriaIndex}");

            storyData.Stats.AddRange(_seriaGameStatsProviderEditor.GetAllStatsToSave());
            storyData.CurrentAudioClipIndex = levelSoundEditMode.CurrentMusicClipIndex;
            storyData.LowPassEffectIsOn = levelSoundEditMode.AudioEffectsCustodian.LowPassEffectIsOn;
            storyData.CustomizableCharacterIndex = _wardrobeCharacterViewer.CustomizableCharacterIndex;
            storyData.BackgroundSaveData = _backgroundEditMode.GetBackgroundSaveData();
            storyData.Contacts.AddRange(_phoneProviderInEditMode.GetSaveData());
            storyData.WardrobeSaveDatas.AddRange(SaveService.CreateWardrobeSaveDatas(_characterProviderEditMode.CustomizableCharacterIndexesCustodians));
            
            // SaveData.StoryDatas[_currentStoryIndex] = storyData;
            SaveData.StoryDatas.Add(storyData);
            SaveServiceProvider.SaveService.Save(SaveData);
        }
    }
    private void InitLevelUIProvider(PhoneContentProvider phoneContentProvider)
    {
        var customizationCharacterPanelUI = LevelUIView.CustomizationCharacterPanelUI;
        BlackFrameUIHandler blackFrameUIHandler = new BlackFrameUIHandler(_blackFrameView);
        _levelUIProviderEditMode = new LevelUIProviderEditMode(LevelUIView, blackFrameUIHandler,
            _wallet, DisableNodesContentEvent, SwitchToNextNodeEvent, customizationCharacterPanelUI, phoneContentProvider,
            ()=>{_levelUIProviderEditMode.PhoneUIHandler.Init(LevelUIView.PhoneUIView);});
    }
    protected override void InitWardrobeCharacterViewer(ViewerCreator viewerCreatorEditMode)
    {
        _wardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorEditMode);
        if (Application.isPlaying)
        {
            var wardrobePS = PrefabsProvider.WardrobePSProvider.CreateWardrobePS(_wardrobeCharacterViewer.transform);
            _wardrobeCharacterViewer.InitParticleSystem(wardrobePS);
        }
    }

    protected override void InitGlobalSound()
    {
        if (SaveData != null && LoadSaveData == true)
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
        if (StoryData != null && LoadSaveData == true)
        {
            _backgroundEditMode.InitSaveData(StoryData.BackgroundSaveData);
        }
        _backgroundEditMode.Construct(DisableNodesContentEvent, CharacterViewer);
    }
    
}