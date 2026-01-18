using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

public class LevelEntryPointEditor : LevelEntryPoint
{
    [SerializeField] private LevelSoundEditMode _levelSoundEditMode;
    [SerializeField] private BackgroundEditMode _backgroundEditMode;
    [Space]
    [SerializeField] private SpriteViewer _spriteViewerPrefab;
    [SerializeField] private ChoiceCaseView[] _choiceCases;
    [SerializeField] private WardrobeCharacterViewer _wardrobeCharacterViewer;
    [SerializeField] private BlackFrameView _blackFrameView;
    [SerializeField] private CharacterProviderEditMode _characterProviderEditMode;
    [SerializeField] private GameSeriesHandlerEditorMode _gameSeriesHandlerEditorMode;
    [SerializeField] private SeriaGameStatsProviderEditor _seriaGameStatsProviderEditor;
    [SerializeField] private GameStatsViewer _gameStatsViewer;
    [SerializeField] private PhoneProviderInEditMode _phoneProviderInEditMode;
    [SerializeField] private Wallet _wallet;
    [SerializeField, Expandable] private StartConfig _startConfig;
    [SerializeField] private string _storyKey;
    [Space]
    [SerializeField] private bool _initializeInEditMode;

    [SerializeField, HideInInspector] private TestModeEditor _testModeEditor;

    private LevelUIProviderEditMode _levelUIProviderEditMode;
    private SaveData _saveData;
    private PhoneSaveHandler _phoneSaveHandler;
    private ICharacterProvider _characterProvider => _characterProviderEditMode.CharacterProvider;
    public bool IsInitializing { get; private set; }
    public bool InitializeInEditMode => _initializeInEditMode;

    private async void Awake()
    {
        PrefabsProvider = new PrefabsProvider();
        await PrefabsProvider.Init();
        Init();
    }
    
    public void Init()
    {
        IsInitializing = true;
        DisableNodesContentEvent?.Execute();
        _seriaGameStatsProviderEditor.Init();
        _gameStatsViewer.Construct(_seriaGameStatsProviderEditor.GameStatsHandler.Stats);
        SaveServiceProvider = new SaveServiceProvider();
        var phoneMessagesCustodian = new PhoneMessagesCustodian();
        ReactiveProperty<bool> phoneNodeIsActive = new ReactiveProperty<bool>();
        _phoneSaveHandler = new PhoneSaveHandler(phoneMessagesCustodian, phoneNodeIsActive);
        Load();
        ConstructSound();
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();
        SwitchToNextNodeEvent = new SwitchToNextNodeEvent();
        SwitchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        
        DisableNodesContentEvent = new DisableNodesContentEvent();
        ViewerCreatorEditMode viewerCreatorEditMode = new ViewerCreatorEditMode(_spriteViewerPrefab);
        CharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorEditMode);
        ConstructWardrobeCharacterViewer(viewerCreatorEditMode);
        ConstructBackground();
        _phoneProviderInEditMode.Construct(phoneMessagesCustodian, _phoneSaveHandler);
        InitLevelUIProvider(_phoneProviderInEditMode.PhoneContentProvider, phoneMessagesCustodian, _phoneSaveHandler);
        NodeGraphInitializer = new NodeGraphInitializer(_characterProviderEditMode.CustomizableCharacterIndexesCustodians, _characterProvider, _backgroundEditMode, _levelUIProviderEditMode,
            CharacterViewer, _wardrobeCharacterViewer, _levelSoundEditMode, _wallet, _seriaGameStatsProviderEditor, _phoneProviderInEditMode,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent, new SetLocalizationChangeEvent(), phoneNodeIsActive);
        DisableNodesContentEvent.Execute();
        if (Application.isPlaying)
        {
            if (StoryData != null)
            {
                _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent, new ReactiveProperty<int>(StoryData.CurrentSeriaIndex),
                   StoryData. CurrentNodeGraphIndex, StoryData.CurrentNodeIndex, StoryData.PutOnSwimsuitKey);
                _levelUIProviderEditMode.CurtainUIHandler.CurtainOpens(new CancellationToken()).Forget();
                _levelUIProviderEditMode.PhoneUIHandler.TryRestartPhoneTime(StoryData.CurrentPhoneMinute);
            }
            else if (_testModeEditor.IsTestMode == true)
            {
                _seriaGameStatsProviderEditor.GameStatsHandler.UpdateStats(_testModeEditor.Stats.ToList());
                _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent, new ReactiveProperty<int>(_testModeEditor.SeriaIndex),
                    _testModeEditor.GraphIndex, _testModeEditor.NodeIndex);

                _levelUIProviderEditMode.CurtainUIHandler.CurtainOpens(new CancellationToken()).Forget();
            }
            else
            {
                _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent, new ReactiveProperty<int>(DefaultSeriaIndex));
            }
        }
        else
        {
            _gameSeriesHandlerEditorMode.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent, new ReactiveProperty<int>(DefaultSeriaIndex));
        }
        IsInitializing = false;
    }

    private void OnApplicationQuit()
    {
        Shutdown();
    }
    protected override void Shutdown()
    {
        Save();
        _gameSeriesHandlerEditorMode.Shutdown();
        _levelUIProviderEditMode.Shutdown();
        _wardrobeCharacterViewer.Shutdown();
        _levelSoundEditMode.ShutdownFromLevel();
        base.Shutdown();
    }

    private void Load()
    {
        if (Application.isPlaying && LoadSaveData == true)
        {
            if (SaveServiceProvider.LoadSaveData(_startConfig) == true)
            {
                _saveData = SaveServiceProvider.SaveData;
                _wallet = new Wallet(_saveData);
                if (_saveData.StoryDatas.TryGetValue(_storyKey, out StoryData))
                {
                    if (StoryData.PhoneSaveDatas.Count > 0)
                    {
                        _phoneSaveHandler.SetPhoneInfoFromSaveData(StoryData);
                    }

                    if (StoryData.Stats.Count > 0)
                    {
                        _seriaGameStatsProviderEditor.UpdateAllStatsFromSave(StoryData.Stats);
                    }

                    if (StoryData.WardrobeSaveDatas.Count > 0)
                    {
                        _characterProviderEditMode.Construct(StoryData.WardrobeSaveDatas); //?
                    }
                    else
                    {
                        _characterProviderEditMode.Construct();

                    }
                }
            }
        }
        else
        {
            _saveData = new SaveData()
            {
                Monets = _startConfig.Monets,
                Hearts = _startConfig.Hearts,
                SoundStatus = _startConfig.SoundStatus,
                NameStartStory = _storyKey,
                LanguageLocalizationKey = _startConfig.DefaultLanguageLocalizationKey,
                StoryDatas = new Dictionary<string, StoryData>()
            };
            _characterProviderEditMode.Construct();
            _wallet = new Wallet(_saveData.Monets, _saveData.Hearts);
        }
    }
    private void Save()
    {
        if (LoadSaveData == true)
        {
            if (StoryData == null)
            {
                StoryData = new StoryData
                {
                    StoryName = _storyKey
                };
            }
            if (_saveData.StoryDatas == null)
            {
                _saveData.StoryDatas = new Dictionary<string, StoryData>();
            }
            else
            {
                _saveData.StoryDatas.Clear();
            }
            StoryData.PutOnSwimsuitKey = _gameSeriesHandlerEditorMode.PutOnSwimsuitKeyProperty;
            _gameSeriesHandlerEditorMode.GetInfoToSave(StoryData);
            
            // StoryData.Stats.Clear();
            // StoryData.Stats.AddRange(_seriaGameStatsProviderEditor.GetAllStatsToSave());
            _seriaGameStatsProviderEditor.GameStatsHandler.FillSaveStats(StoryData);
            StoryData.CurrentAudioMusicKey = _levelSoundEditMode.CurrentMusicClipKey;
            StoryData.CurrentAudioAmbientKey = _levelSoundEditMode.CurrentAdditionalClipKey;

            StoryData.AudioEffectsIsOn.Clear();
            var effects = _levelSoundEditMode.AudioEffectsCustodian.GetEnableEffectsToSave();
            if (effects != null)
            {
                StoryData.AudioEffectsIsOn.AddRange(effects);
            }
            
            StoryData.CustomizableCharacterIndex = _wardrobeCharacterViewer.CustomizableCharacterIndex;
            _backgroundEditMode.FillSaveData(StoryData);

            _phoneProviderInEditMode.FillPhoneSaveInfo(StoryData);
            
            StoryData.WardrobeSaveDatas.Clear();
            StoryData.WardrobeSaveDatas.AddRange(SaveService.CreateWardrobeSaveDatas(_characterProviderEditMode.CustomizableCharacterIndexesCustodians));


            _saveData.StoryDatas.Add(_storyKey, StoryData);
            SaveServiceProvider.SaveService.Save(_saveData);
        }
    }
    private void InitLevelUIProvider(PhoneContentProvider phoneContentProvider, PhoneMessagesCustodian phoneMessagesCustodian, PhoneSaveHandler phoneSaveHandler)
    {
        var customizationCharacterPanelUI = LevelUIView.CustomizationCharacterPanelUI;
        BlackFrameUIHandler blackFrameUIHandler = new BlackFrameUIHandler(_blackFrameView);
        _levelUIProviderEditMode = new LevelUIProviderEditMode(LevelUIView, blackFrameUIHandler, 
            new ChoicePanelInitializerEditMode(_choiceCases),
            _wallet, DisableNodesContentEvent, SwitchToNextNodeEvent, customizationCharacterPanelUI, phoneContentProvider,
            ()=>{_levelUIProviderEditMode.PhoneUIHandler.Init(LevelUIView.PhoneUIView, phoneMessagesCustodian, phoneSaveHandler, _gameSeriesHandlerEditorMode.GetNodePort);});
    }
    protected override void ConstructWardrobeCharacterViewer(ViewerCreator viewerCreatorEditMode)
    {
        _wardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorEditMode);
        if (Application.isPlaying)
        {
            var wardrobePS = PrefabsProvider.WardrobePSProvider.CreateWardrobePS(_wardrobeCharacterViewer.transform);
            _wardrobeCharacterViewer.InitParticleSystem(wardrobePS);
        }
    }

    protected override void ConstructSound()
    {
        if (_saveData != null && LoadSaveData == true)
        {
            _levelSoundEditMode.Construct(_saveData.SoundStatus);
            _levelSoundEditMode.Init(StoryData.AudioEffectsIsOn, StoryData.CurrentAudioMusicKey, StoryData.CurrentAudioAmbientKey);
        }
        else
        {
            _levelSoundEditMode.Construct(soundOn: true);
            _levelSoundEditMode.Init();
        }
    }
    protected override void ConstructBackground()
    {
        if (StoryData != null && LoadSaveData == true)
        {
            _backgroundEditMode.InitSaveData(StoryData.BackgroundSaveData);
        }
        _backgroundEditMode.Construct(DisableNodesContentEvent, CharacterViewer);
    }
    
}