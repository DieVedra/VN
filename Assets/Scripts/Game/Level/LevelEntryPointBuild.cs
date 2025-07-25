﻿using Cysharp.Threading.Tasks;
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
    private MainMenuLocalizationHandler _mainMenuLocalizationHandler;
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
    private GameStatsHandler _gameStatsHandler => _levelLoadDataHandler.SeriaGameStatsProviderBuild.GameStatsHandler;
    
    [Inject]
    private void Construct(GlobalSound globalSound, PrefabsProvider prefabsProvider, GlobalUIHandler globalUIHandler,
        Wallet wallet, MainMenuLocalizationHandler mainMenuLocalizationHandler)
    {
        _globalSound = globalSound;
        PrefabsProvider = prefabsProvider;
        _globalUIHandler = globalUIHandler;
        _wallet = wallet;
        _mainMenuLocalizationHandler = mainMenuLocalizationHandler;
    }
    private async void Awake()
    {
        _setLocalizationChangeEvent = new SetLocalizationChangeEvent();
        _blockGameControlPanelUIEvent = new BlockGameControlPanelUIEvent<bool>();
        _currentSeriaIndexReactiveProperty = new ReactiveProperty<int>(DefaultSeriaIndex);
        _levelLocalizationProvider = new LevelLocalizationProvider(_mainMenuLocalizationHandler, _currentSeriaIndexReactiveProperty);
        _backgroundContentCreator = new BackgroundContentCreator(_backgroundBuildMode.transform, PrefabsProvider.SpriteRendererAssetProvider);
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();

        if (LoadSaveData == true)
        {
            SaveData = SaveServiceProvider.SaveData;
            StoryData = SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            _currentSeriaIndexReactiveProperty.Value = StoryData.CurrentSeriaIndex;
            _levelLoadDataHandler = new LevelLoadDataHandler(_mainMenuLocalizationHandler, _backgroundContentCreator,
                _levelLocalizationProvider, SwitchToNextSeriaEvent, CurrentSeriaNumberProvider.GetCurrentSeriaNumber(_currentSeriaIndexReactiveProperty.Value));
        }
        else
        {
            _levelLoadDataHandler = new LevelLoadDataHandler(_mainMenuLocalizationHandler, _backgroundContentCreator,
                _levelLocalizationProvider, SwitchToNextSeriaEvent);
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

        await _globalUIHandler.LoadScreenUIHandler.HideOnLevelMove();
        _levelLoadDataHandler.LoadNextSeriesContent().Forget();
    }

    private void Init()
    {
        if (LoadSaveData == true)
        {
            _gameStatsHandler.UpdateStatFromSave(StoryData.Stats);
            StoryData.StoryStarted = true;
        }
        InitGlobalSound();
        OnSceneTransitionEvent = new OnSceneTransitionEvent();
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
            _levelUIProviderBuildMode, CharacterViewer, _wardrobeCharacterViewer,
            _levelLoadDataHandler.WardrobeSeriaDataProviderBuildMode, _globalSound, _wallet, _levelLoadDataHandler.SeriaGameStatsProviderBuild,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent, _setLocalizationChangeEvent);

        if (SaveData == null)
        {
            _gameSeriesHandlerBuildMode.Construct(_gameStatsHandler, _levelLocalizationHandler,_levelLoadDataHandler.GameSeriesProvider, NodeGraphInitializer, SwitchToNextSeriaEvent,
                _currentSeriaIndexReactiveProperty);
        }
        else
        {
            _gameSeriesHandlerBuildMode.Construct(_gameStatsHandler, _levelLocalizationHandler,_levelLoadDataHandler.GameSeriesProvider, NodeGraphInitializer, SwitchToNextSeriaEvent, 
                _currentSeriaIndexReactiveProperty, StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeIndex);
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
            StoryData.Stats = _gameStatsHandler.GetStatsToSave();
            StoryData.BackgroundSaveData = _backgroundBuildMode.GetBackgroundSaveData();
            
            StoryData.CurrentAudioClipIndex = _globalSound.CurrentMusicClipIndex;
            StoryData.LowPassEffectIsOn = _globalSound.AudioEffectsCustodian.LowPassEffectIsOn;

            SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = StoryData;
            SaveServiceProvider.SaveService.Save(SaveData);
        }
    }

    private void InitLevelUIProvider()
    {
        CustomizationCharacterPanelUI customizationCharacterPanelUI =
            PrefabsProvider.CustomizationCharacterPanelAssetProvider.CreateCustomizationCharacterPanelUI(LevelUIView
                .transform);
        customizationCharacterPanelUI.transform.SetSiblingIndex(customizationCharacterPanelUI.SublingIndex);
        customizationCharacterPanelUI.gameObject.SetActive(false);
        _levelUIProviderBuildMode = new LevelUIProviderBuildMode(LevelUIView, _darkeningBackgroundFrameUIHandler, _wallet, OnSceneTransitionEvent, DisableNodesContentEvent,
            SwitchToNextNodeEvent, customizationCharacterPanelUI, _blockGameControlPanelUIEvent, _levelLocalizationHandler, _globalSound, _mainMenuLocalizationHandler, _globalUIHandler);
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