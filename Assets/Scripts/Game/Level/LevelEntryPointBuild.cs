
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using Zenject;

public class LevelEntryPointBuild : LevelEntryPoint
{
    [SerializeField] private GameSeriesHandlerBuildMode _gameSeriesHandlerBuildMode;
    [SerializeField] private BackgroundBuildMode _backgroundBuildMode;
    
    private GlobalSound _globalSound;
    private MainMenuLocalizationHandler _mainMenuLocalizationHandler;
    private LevelLoadDataHandler _levelLoadDataHandler;
    private BackgroundContentCreator _backgroundContentCreator;
    private GlobalUIHandler _globalUIHandler;
    private SpriteRendererCreatorBuild _spriteRendererCreator;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private LevelLocalizationProvider _levelLocalizationProvider;

    [Inject]
    private void Construct(GlobalSound globalSound, PrefabsProvider prefabsProvider, GlobalUIHandler globalUIHandler,
        Wallet wallet, MainMenuLocalizationHandler mainMenuLocalizationHandler)
    {
        _globalSound = globalSound;
        PrefabsProvider = prefabsProvider;
        _globalUIHandler = globalUIHandler;
        Wallet = wallet;
        _mainMenuLocalizationHandler = mainMenuLocalizationHandler;
    }
    private async void Awake()
    {
        _levelLocalizationProvider = new LevelLocalizationProvider(_mainMenuLocalizationHandler);
        _backgroundContentCreator = new BackgroundContentCreator(_backgroundBuildMode.transform, PrefabsProvider.SpriteRendererAssetProvider);
        _levelLoadDataHandler = new LevelLoadDataHandler(_backgroundContentCreator, SwitchToNextSeriaEvent);

        await _levelLoadDataHandler.LoadFirstSeriaContent();
        LevelCanvasAssetProvider levelCanvasAssetProvider = new LevelCanvasAssetProvider();
        LevelUIView = await levelCanvasAssetProvider.CreateAsset();
        if (LevelUIView.TryGetComponent(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }
        await TryCreateBlackFrameUIHandler();

        Init();
        OnSceneTransition.Subscribe(_ =>
        {
            Dispose();
            Save();
        });

        await _globalUIHandler.LoadScreenUIHandler.HideOnLevelMove();
    }

    private void Init()
    {
        // GameStatsCustodian.Construct();

        if (LoadSaveData == true)
        {
            SaveData = SaveServiceProvider.SaveData;
            StoryData = SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            TestMonets = SaveData.Monets;
            TestHearts = SaveData.Hearts;
            // GameStatsCustodian.UpdateStatFromSave(StoryData.Stats);
            StoryData.StoryStarted = true;
        }
        // else
        // {
        //     GameStatsCustodian.Construct();
        // }
        InitGlobalSound();
        OnSceneTransition = new ReactiveCommand();
        SwitchToNextNodeEvent = new SwitchToNextNodeEvent();
        SwitchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        DisableNodesContentEvent = new DisableNodesContentEvent();
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();
        InitLevelUIProvider();
        ViewerCreatorBuildMode viewerCreatorBuildMode = new ViewerCreatorBuildMode(PrefabsProvider.SpriteViewerAssetProvider);
        CharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorBuildMode);
        InitWardrobeCharacterViewer(viewerCreatorBuildMode);

        _spriteRendererCreator = new SpriteRendererCreatorBuild(PrefabsProvider.SpriteRendererAssetProvider);
        InitBackground();
        
        NodeGraphInitializer = new NodeGraphInitializer(_levelLoadDataHandler.CharacterProviderBuildMode,
            _backgroundBuildMode.GetBackgroundContent, _backgroundBuildMode,
            LevelUIProvider, CharacterViewer, WardrobeCharacterViewer,
            _levelLoadDataHandler.WardrobeSeriaDataProviderBuildMode, _globalSound, Wallet, _levelLoadDataHandler.SeriaGameStatsProviderBuild,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent);

        if (SaveData == null)
        {
            _gameSeriesHandlerBuildMode.Construct(_levelLoadDataHandler.GameSeriesProvider, NodeGraphInitializer, SwitchToNextSeriaEvent);
        }
        else
        {
            _gameSeriesHandlerBuildMode.Construct(_levelLoadDataHandler.GameSeriesProvider, NodeGraphInitializer, SwitchToNextSeriaEvent, 
                StoryData.CurrentSeriaIndex, StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeIndex);
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
        _levelLoadDataHandler.WardrobeSeriaDataProviderBuildMode.Dispose();
        _levelLoadDataHandler.CharacterProviderBuildMode.Dispose();
        _levelLoadDataHandler.GameSeriesProvider.Dispose();
        _levelLoadDataHandler.AudioClipProvider.Dispose();
        _levelLoadDataHandler.BackgroundDataProvider.Dispose();
        base.Dispose();
    }
    private void Save()
    {
        if (LoadSaveData == true)
        {
            StoryData.CurrentNodeGraphIndex = _gameSeriesHandlerBuildMode.CurrentNodeGraphIndex;
            StoryData.CurrentNodeIndex = _gameSeriesHandlerBuildMode.CurrentNodeIndex;
            // StoryData.Stats = GameStatsCustodian.GetSaveStatsToSave();
            StoryData.BackgroundSaveData = _backgroundBuildMode.GetBackgroundSaveData();
            
            StoryData.CurrentAudioClipIndex = _globalSound.CurrentMusicClipIndex;
            StoryData.LowPassEffectIsOn = _globalSound.AudioEffectsCustodian.LowPassEffectIsOn;

            SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = StoryData;
            SaveServiceProvider.SaveService.Save(SaveData);
        }
    }

    private void InitLevelUIProvider()
    {
        // LevelUIView.CustomizationCharacterPanelUI.gameObject.SetActive(false);
        CustomizationCharacterPanelUI customizationCharacterPanelUI =
            PrefabsProvider.CustomizationCharacterPanelAssetProvider.CreateCustomizationCharacterPanelUI(LevelUIView
                .transform);
        customizationCharacterPanelUI.transform.SetSiblingIndex(customizationCharacterPanelUI.SublingIndex);
        LevelUIProvider = new LevelUIProvider(LevelUIView, _darkeningBackgroundFrameUIHandler, Wallet, OnSceneTransition, DisableNodesContentEvent,
            SwitchToNextNodeEvent, customizationCharacterPanelUI, _globalSound, _mainMenuLocalizationHandler,
            _globalUIHandler);
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
        WardrobeCharacterViewer =
            PrefabsProvider.WardrobeCharacterViewerAssetProvider.CreateWardrobeCharacterViewer(transform);
        WardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreator);
        PrefabsProvider.SpriteViewerAssetProvider.UnloadAsset();
    }
}