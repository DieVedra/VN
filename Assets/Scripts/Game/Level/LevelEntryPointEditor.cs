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
    private SaveData _saveData;
    private ICharacterProvider _characterProvider => _characterProviderEditMode.CharacterProvider;
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
        SaveServiceProvider = new SaveServiceProvider();
        if (Application.isPlaying &&  LoadSaveData == true)
        {
            if (SaveServiceProvider.LoadSaveData() == true)
            {
                _saveData = SaveServiceProvider.SaveData;
                _testMonets = _saveData.Monets;
                _testHearts = _saveData.Hearts;
                _wallet = new Wallet(_saveData);
                StoryData = _saveData.StoryDatas[_currentStoryIndex];
                if (StoryData.Contacts.Count > 0)
                {
                    _phoneProviderInEditMode.TrySetSaveData(StoryData.Contacts);
                }

                if (StoryData.Stats.Count > 0)
                {
                    _seriaGameStatsProviderEditor.UpdateAllStatsFromSave(StoryData.Stats);
                }

                if (StoryData.WardrobeSaveDatas.Count > 0)
                {
                    _characterProviderEditMode.Construct(StoryData.WardrobeSaveDatas);//?
                }
                else
                {
                    _characterProviderEditMode.Construct();

                }
            }
            else
            {
                ConstructSave();
            }
        }
        else
        {
            ConstructSave();
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
            if (StoryData != null)
            {
                _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, _characterProvider, SwitchToNextSeriaEvent, new ReactiveProperty<int>(StoryData.CurrentSeriaIndex),
                   StoryData. CurrentNodeGraphIndex, StoryData.CurrentNodeIndex, StoryData.PutOnSwimsuitKey);
                _levelUIProviderEditMode.CurtainUIHandler.CurtainOpens(new CancellationToken()).Forget();
                _levelUIProviderEditMode.PhoneUIHandler.TryRestartPhoneTime(StoryData.CurrentPhoneMinute);
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

        void ConstructSave()
        {
            _saveData = SaveServiceProvider.SaveData;
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
            StoryData.PutOnSwimsuitKey = _gameSeriesHandlerEditorMode.PutOnSwimsuitKeyProperty;
            StoryData.CurrentNodeGraphIndex = _gameSeriesHandlerEditorMode.CurrentNodeGraphIndex;
            StoryData.CurrentNodeIndex = _gameSeriesHandlerEditorMode.CurrentNodeIndex;
            StoryData.CurrentSeriaIndex = _gameSeriesHandlerEditorMode.CurrentSeriaIndex;
            StoryData.Stats.Clear();
            StoryData.Stats.AddRange(_seriaGameStatsProviderEditor.GetAllStatsToSave());
            StoryData.CurrentAudioClipIndex = levelSoundEditMode.CurrentMusicClipIndex;
            StoryData.LowPassEffectIsOn = levelSoundEditMode.AudioEffectsCustodian.LowPassEffectIsOn;
            StoryData.CustomizableCharacterIndex = _wardrobeCharacterViewer.CustomizableCharacterIndex;
            StoryData.BackgroundSaveData = _backgroundEditMode.GetBackgroundSaveData();
            StoryData.Contacts.Clear();
            StoryData.Contacts.AddRange(_phoneProviderInEditMode.GetSaveData());
            StoryData.WardrobeSaveDatas.Clear();
            StoryData.WardrobeSaveDatas.AddRange(SaveService.CreateWardrobeSaveDatas(_characterProviderEditMode.CustomizableCharacterIndexesCustodians));
            if (_levelUIProviderEditMode.PhoneUIHandler.PhoneTime?.IsStarted == true)
            {
                StoryData.CurrentPhoneMinute = _levelUIProviderEditMode.PhoneUIHandler.PhoneTime.CurrentMinute;
            }
            _saveData.StoryDatas.Clear();
            _saveData.StoryDatas.Add(StoryData);
            SaveServiceProvider.SaveService.Save(_saveData);
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
        if (_saveData != null && LoadSaveData == true)
        {
            levelSoundEditMode.Construct(_saveData.SoundStatus);
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