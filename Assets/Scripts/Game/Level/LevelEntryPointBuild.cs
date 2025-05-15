
using UniRx;
using UnityEngine;
using Zenject;

public class LevelEntryPointBuild : LevelEntryPoint
{
    [SerializeField] private GameSeriesHandlerBuildMode _gameSeriesHandlerBuildMode;
    [SerializeField] private BackgroundBuildMode _backgroundBuildMode;
    [Inject, SerializeField] private GlobalSound _globalSound;
    
    private LevelLoadDataHandler _levelLoadDataHandler;
    private BackgroundContentCreator _backgroundContentCreator;

    private async void Awake()
    {
        _backgroundContentCreator = new BackgroundContentCreator(_backgroundBuildMode.transform);
        _levelLoadDataHandler = new LevelLoadDataHandler(_backgroundContentCreator, 5);

        await _levelLoadDataHandler.LoadFirstSeriaContent();
        
        
        Init();
        OnSceneTransition.Subscribe(_ =>
        {
            Dispose();
        });
        
        _levelLoadDataHandler.LoadNextSeriesContent().Forget();
    }

    private void Init()
    {
        if (LoadSaveData == true)
        {
            SaveData = SaveServiceProvider.SaveData;
            StoryData = SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            TestMonets = SaveData.Monets;
            TestHearts = SaveData.Hearts;
            Wallet = new Wallet(SaveData);
            GameStatsCustodian.Init(StoryData.Stats);
        }
        else
        {
            Wallet = new Wallet(TestMonets, TestHearts);
            GameStatsCustodian.Init();
        }
        InitGlobalSound();
        OnSceneTransition = new ReactiveCommand();
        SwitchToNextNodeEvent = new SwitchToNextNodeEvent();
        SwitchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        DisableNodesContentEvent = new DisableNodesContentEvent();
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();
        InitLevelUIProvider();
        ViewerCreatorBuildMode viewerCreatorBuildMode = new ViewerCreatorBuildMode();
        CharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorBuildMode);
        InitWardrobeCharacterViewer(viewerCreatorBuildMode);

        SpriteRendererCreator spriteRendererCreator = new SpriteRendererCreatorBuild();
        InitBackground();
        
        NodeGraphInitializer = new NodeGraphInitializer(_levelLoadDataHandler.CharacterProviderBuildMode.GetCharacters(),
            _backgroundBuildMode.GetBackgroundContent, _backgroundBuildMode,
            LevelUIProvider,
            CharacterViewer, WardrobeCharacterViewer,
            _levelLoadDataHandler.CharacterProviderBuildMode.CustomizableCharacter,
            _levelLoadDataHandler.WardrobeSeriaDataProviderBuildMode,
            _globalSound, GameStatsCustodian,
            Wallet,
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
    }

    protected override void InitBackground()
    {
        if (LoadSaveData == true)
        {
            _backgroundBuildMode.InitSaveData(StoryData.BackgroundSaveData);
        }

        _backgroundBuildMode.Construct(_levelLoadDataHandler.BackgroundDataProvider, _backgroundContentCreator, CharacterViewer);
    }
    protected override void Dispose()
    {
        _gameSeriesHandlerBuildMode.Dispose();
        _levelLoadDataHandler.WardrobeSeriaDataProviderBuildMode.Dispose();
        _levelLoadDataHandler.CharacterProviderBuildMode.Dispose();
        _levelLoadDataHandler.GameSeriesProvider.Dispose();
        _levelLoadDataHandler.AudioClipProvider.Dispose();
        _levelLoadDataHandler.BackgroundDataProvider.Dispose();
        _backgroundContentCreator.Dispose();
        base.Dispose();
    }
    private void Save()
    {
        if (LoadSaveData == true)
        {
            StoryData.CurrentNodeGraphIndex = _gameSeriesHandlerBuildMode.CurrentNodeGraphIndex;
            StoryData.CurrentNodeIndex = _gameSeriesHandlerBuildMode.CurrentNodeIndex;
            StoryData.Stats = GameStatsCustodian.GetSaveStatsToSave();
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
        LevelUIProvider = new LevelUIProvider(LevelUIView, Wallet, OnSceneTransition, DisableNodesContentEvent,
            SwitchToNextNodeEvent, customizationCharacterPanelUI);
    }

    protected override void InitGlobalSound()
    {
        if (LoadSaveData == true)
        {
            _globalSound.Construct(_levelLoadDataHandler.AudioClipProvider, SaveData.SoundIsOn);
        }
        else
        {
            _globalSound.Construct(_levelLoadDataHandler.AudioClipProvider, true);
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