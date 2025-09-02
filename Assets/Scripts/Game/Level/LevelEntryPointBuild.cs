using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class LevelEntryPointBuild : LevelEntryPoint
{
    [SerializeField] private GameSeriesHandlerBuildMode _gameSeriesHandlerBuildMode;
    [SerializeField] private BackgroundBuildMode _backgroundBuildMode;

    private Wallet _wallet;
    private WardrobeCharacterViewer _wardrobeCharacterViewer;
    private LevelUIProviderBuildMode _levelUIProviderBuildMode;
    private GlobalSound _globalSound;
    private PanelsLocalizationHandler _panelsLocalizationHandler;
    private LevelLoadDataHandler _levelLoadDataHandler;
    private BackgroundContentCreator _backgroundContentCreator;
    private GlobalUIHandler _globalUIHandler;
    private SpriteRendererCreatorBuild _spriteRendererCreator;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private LevelLocalizationProvider _levelLocalizationProvider;
    private LevelLocalizationHandler _levelLocalizationHandler;
    private BlockGameControlPanelUIEvent<bool> _blockGameControlPanelUIEvent;
    private ReactiveProperty<int> _currentSeriaIndexReactiveProperty;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private OnAwaitLoadContentEvent<AwaitLoadContentPanel> _onAwaitLoadContentEvent;
    private OnContentIsLoadProperty<bool> _onContentIsLoadProperty;
    private CurrentSeriaLoadedNumberProperty<int> _currentSeriaLoadedNumberProperty;
    private OnEndGameEvent _onEndGameEvent;
    private GameStatsHandler _gameStatsHandler => _levelLoadDataHandler.SeriaGameStatsProviderBuild.GameStatsHandler;
    
    [Inject]
    private void Construct(GlobalSound globalSound, PrefabsProvider prefabsProvider, GlobalUIHandler globalUIHandler,
        Wallet wallet, PanelsLocalizationHandler panelsLocalizationHandler, SaveServiceProvider saveServiceProvider)
    {
        _globalSound = globalSound;
        PrefabsProvider = prefabsProvider;
        _globalUIHandler = globalUIHandler;
        _wallet = wallet;
        _panelsLocalizationHandler = panelsLocalizationHandler;
        SaveServiceProvider = saveServiceProvider;
    }
    private async void Awake()
    {
        _currentSeriaLoadedNumberProperty = new CurrentSeriaLoadedNumberProperty<int>();
        _onAwaitLoadContentEvent = new OnAwaitLoadContentEvent<AwaitLoadContentPanel>();
        _setLocalizationChangeEvent = new SetLocalizationChangeEvent();
        _blockGameControlPanelUIEvent = new BlockGameControlPanelUIEvent<bool>();
        _currentSeriaIndexReactiveProperty = new ReactiveProperty<int>(DefaultSeriaIndex);
        _levelLocalizationProvider = new LevelLocalizationProvider(_panelsLocalizationHandler, _currentSeriaIndexReactiveProperty);
        _backgroundContentCreator = new BackgroundContentCreator(_backgroundBuildMode.transform, PrefabsProvider.SpriteRendererAssetProvider);
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();
        OnSceneTransitionEvent = new OnSceneTransitionEvent();
        SwitchToNextNodeEvent = new SwitchToNextNodeEvent();
        SwitchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        DisableNodesContentEvent = new DisableNodesContentEvent();
        _onEndGameEvent = new OnEndGameEvent();
        _onContentIsLoadProperty = new OnContentIsLoadProperty<bool>();
        
        if (LoadSaveData == true)
        {
            SaveData = SaveServiceProvider.SaveData;
            StoryData = SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            _currentSeriaIndexReactiveProperty.Value = StoryData.CurrentSeriaIndex;
            _levelLoadDataHandler = new LevelLoadDataHandler(_panelsLocalizationHandler, _backgroundContentCreator,
                _levelLocalizationProvider, SwitchToNextSeriaEvent, _currentSeriaLoadedNumberProperty,
                _onContentIsLoadProperty, CurrentSeriaNumberProvider.GetCurrentSeriaNumber(_currentSeriaIndexReactiveProperty.Value));
        }
        else
        {
            _levelLoadDataHandler = new LevelLoadDataHandler(_panelsLocalizationHandler, _backgroundContentCreator,
                _levelLocalizationProvider, SwitchToNextSeriaEvent, _currentSeriaLoadedNumberProperty, _onContentIsLoadProperty);
        }
        
        _levelLocalizationHandler = new LevelLocalizationHandler(_levelLocalizationProvider, _levelLoadDataHandler.CharacterProviderBuildMode, _setLocalizationChangeEvent);

        await _levelLoadDataHandler.LoadStartSeriaContent();
        LevelCanvasAssetProvider levelCanvasAssetProvider = new LevelCanvasAssetProvider();
        LevelUIView = await levelCanvasAssetProvider.CreateAsset();
        if (LevelUIView.TryGetComponent(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }
        await TryCreateBlackFrameUIHandler();

        Init();
        OnSceneTransitionEvent.Subscribe(() =>
        {
            Dispose();
            Save();
        }); 
        _panelsLocalizationHandler.AddLocalizableContentFromLevel(_levelUIProviderBuildMode.GetLocalizableContent());
        await _globalUIHandler.LoadScreenUIHandler.HideOnLevelMove();
        await UniTask.RunOnThreadPool(() =>
        {
            _levelLoadDataHandler.LoadNextSeriesContent().Forget();
        });

    }

    private void Init()
    {
        if (LoadSaveData == true)
        {
            _gameStatsHandler.UpdateStatFromSave(StoryData.Stats);
            StoryData.StoryStarted = true;
        }
        InitGlobalSound();
        InitLevelUIProvider();
        ViewerCreatorBuildMode viewerCreatorBuildMode = new ViewerCreatorBuildMode(PrefabsProvider.SpriteViewerAssetProvider);
        CharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorBuildMode);
        InitWardrobeCharacterViewer(viewerCreatorBuildMode);

        _spriteRendererCreator = new SpriteRendererCreatorBuild(PrefabsProvider.SpriteRendererAssetProvider);
        InitBackground();
        
        NodeGraphInitializer = new NodeGraphInitializer(_levelLoadDataHandler.CharacterProviderBuildMode,
            _backgroundBuildMode.GetBackgroundContent, _backgroundBuildMode,
            _levelUIProviderBuildMode, CharacterViewer, _wardrobeCharacterViewer,
            _levelLoadDataHandler.WardrobeSeriaDataProviderBuildMode, _globalSound, _wallet, _levelLoadDataHandler.SeriaGameStatsProviderBuild,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent, _setLocalizationChangeEvent);

        if (SaveData == null)
        {
            _gameSeriesHandlerBuildMode.Construct(_gameStatsHandler, _levelLocalizationHandler, _levelLoadDataHandler.GameSeriesProvider,
                NodeGraphInitializer, SwitchToNextSeriaEvent, _currentSeriaIndexReactiveProperty,
                _onContentIsLoadProperty, _onAwaitLoadContentEvent, _currentSeriaLoadedNumberProperty, _onEndGameEvent);
        }
        else
        {
            _gameSeriesHandlerBuildMode.Construct(_gameStatsHandler, _levelLocalizationHandler,_levelLoadDataHandler.GameSeriesProvider,
                NodeGraphInitializer, SwitchToNextSeriaEvent, _currentSeriaIndexReactiveProperty, _onContentIsLoadProperty,
                _onAwaitLoadContentEvent, _currentSeriaLoadedNumberProperty, _onEndGameEvent, StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeIndex);
        }
    }
    private void OnApplicationQuit()
    {
        Dispose();
        Save();
    }

    protected override void InitBackground()
    {
        if (LoadSaveData == true)
        {
            _backgroundBuildMode.InitSaveData(StoryData.BackgroundSaveData);
        }

        _backgroundBuildMode.Construct(_levelLoadDataHandler.BackgroundDataProvider, _backgroundContentCreator, CharacterViewer, _spriteRendererCreator);
    }
    protected override void Dispose()
    {
        _gameSeriesHandlerBuildMode.Dispose();
        _levelLoadDataHandler.Dispose();
        _levelUIProviderBuildMode.Dispose();
        base.Dispose();
    }
    private void Save()
    {
        if (LoadSaveData == true)
        {
            StoryData.CurrentNodeGraphIndex = _gameSeriesHandlerBuildMode.CurrentNodeGraphIndex;
            StoryData.CurrentNodeIndex = _gameSeriesHandlerBuildMode.CurrentNodeIndex;
            StoryData.CurrentSeriaIndex = _gameSeriesHandlerBuildMode.CurrentSeriaIndex;
            StoryData.Stats = _gameStatsHandler.GetStatsToSave();
            StoryData.BackgroundSaveData = _backgroundBuildMode.GetBackgroundSaveData();
            StoryData.WardrobeSaveDatas = SaveService.CreateWardrobeSaveDatas(_levelLoadDataHandler.CharacterProviderBuildMode.CustomizableCharacters);
            StoryData.CurrentAudioClipIndex = _globalSound.CurrentMusicClipIndex;
            StoryData.LowPassEffectIsOn = _globalSound.AudioEffectsCustodian.LowPassEffectIsOn;
            StoryData.CustomizableCharacterIndex = _wardrobeCharacterViewer.CustomizableCharacterIndex;
            SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = StoryData;
            SaveServiceProvider.SaveLevelProgress();
        }
    }

    private void InitLevelUIProvider()
    {
        CustomizationCharacterPanelUI customizationCharacterPanelUI =
            PrefabsProvider.CustomizationCharacterPanelAssetProvider.CreateCustomizationCharacterPanelUI(LevelUIView
                .transform);
        customizationCharacterPanelUI.transform.SetSiblingIndex(customizationCharacterPanelUI.SublingIndex);
        customizationCharacterPanelUI.gameObject.SetActive(false);
        _levelUIProviderBuildMode = new LevelUIProviderBuildMode(LevelUIView, _darkeningBackgroundFrameUIHandler, _wallet, DisableNodesContentEvent,
            SwitchToNextNodeEvent, customizationCharacterPanelUI, _blockGameControlPanelUIEvent, _levelLocalizationHandler, _globalSound,
            _panelsLocalizationHandler, _globalUIHandler,
            new ButtonTransitionToMainSceneUIHandler(_globalUIHandler.LoadScreenUIHandler, OnSceneTransitionEvent, _globalSound.SmoothAudio),
            _levelLoadDataHandler.LoadAssetsPercentHandler, _onAwaitLoadContentEvent, _onEndGameEvent);
    }
    private async UniTask TryCreateBlackFrameUIHandler()
    {
        if (_darkeningBackgroundFrameUIHandler == null)
        {
            _darkeningBackgroundFrameUIHandler = new BlackFrameUIHandler();
            await _darkeningBackgroundFrameUIHandler.Init(LevelUIView.transform);
            _darkeningBackgroundFrameUIHandler.SetAsLastSibling();
            _darkeningBackgroundFrameUIHandler.BlackFrameView.Image.color = Color.clear;
        }
    }
    protected override void InitGlobalSound()
    {
        _globalSound.SetAudioClipProvider(_levelLoadDataHandler.AudioClipProvider);
        if (LoadSaveData == true)
        {
            _globalSound.Construct(SaveData.SoundStatus);
        }
        else
        {
            _globalSound.Construct(true);
        }
    }

    protected override void InitWardrobeCharacterViewer(ViewerCreator viewerCreator)
    {
        _wardrobeCharacterViewer =
            PrefabsProvider.WardrobeCharacterViewerAssetProvider.CreateWardrobeCharacterViewer(transform);
        _wardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreator);
        var ps = PrefabsProvider.WardrobePSProvider.CreateWardrobePS(_wardrobeCharacterViewer.transform);
        _wardrobeCharacterViewer.InitParticleSystem(ps);
        PrefabsProvider.SpriteViewerAssetProvider.UnloadAsset();
    }
}