using System.Collections.Generic;
using System.Threading;
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
    private BackgroundPool _backgroundPool;
    private GlobalUIHandler _globalUIHandler;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private LevelLocalizationProvider _levelLocalizationProvider;
    private LevelLocalizationHandler _levelLocalizationHandler;
    private PhoneUIView _phoneView;
    private BlockGameControlPanelUIEvent<bool> _blockGameControlPanelUIEvent;
    private ReactiveProperty<int> _currentSeriaIndexReactiveProperty;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private OnAwaitLoadContentEvent<AwaitLoadContentPanel> _onAwaitLoadContentEvent;
    private OnContentIsLoadProperty<bool> _onContentIsLoadProperty;
    private ReactiveProperty<bool> _phoneNodeIsActive;
    private CurrentSeriaLoadedNumberProperty<int> _currentSeriaLoadedNumberProperty;
    private OnEndGameEvent _onEndGameEvent;
    private CancellationTokenSource _cancellationTokenSource;
    private LevelUISpriteAtlasAssetProvider _levelUISpriteAtlasAssetProvider;
    private GameStatsHandler _gameStatsHandler => _levelLoadDataHandler.SeriaGameStatsProviderBuild.GameStatsHandler;
    private ICharacterProvider _characterProvider => _levelLoadDataHandler.CharacterProviderBuildMode.CharacterProvider;

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
        SwitchToNextSeriaEvent = new SwitchToNextSeriaEvent<bool>();
        SwitchToNextNodeEvent = new SwitchToNextNodeEvent();
        SwitchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        DisableNodesContentEvent = new DisableNodesContentEvent();
        _onEndGameEvent = new OnEndGameEvent();
        _onContentIsLoadProperty = new OnContentIsLoadProperty<bool>();
        var phoneMessagesCustodian = new PhoneMessagesCustodian();
        _phoneNodeIsActive = new ReactiveProperty<bool>();

        PhoneSaveHandler phoneSaveHandler = new PhoneSaveHandler(phoneMessagesCustodian, _phoneNodeIsActive);
        if (LoadSaveData == true)
        {
            if (SaveServiceProvider.SaveData.StoryDatas.TryGetValue(SaveServiceProvider.CurrentStoryKey, out StoryData))
            {
                _currentSeriaIndexReactiveProperty.Value = StoryData.CurrentSeriaIndex; 
                _currentSeriaLoadedNumberProperty.SetValue(StoryData.CurrentSeriaIndex);

            }
            _levelLoadDataHandler = new LevelLoadDataHandler(_panelsLocalizationHandler, phoneMessagesCustodian,
                _levelLocalizationProvider, phoneSaveHandler, CreatePhoneView, SwitchToNextSeriaEvent, _currentSeriaLoadedNumberProperty,
                _onContentIsLoadProperty);
        }
        else
        {
            _currentSeriaLoadedNumberProperty.SetValue(0);
            _levelLoadDataHandler = new LevelLoadDataHandler(_panelsLocalizationHandler, phoneMessagesCustodian,
                _levelLocalizationProvider, phoneSaveHandler, CreatePhoneView, SwitchToNextSeriaEvent, _currentSeriaLoadedNumberProperty, _onContentIsLoadProperty);
        }


        await CreateBackgroundPool();
        _backgroundBuildMode.SubscribeProviders(_levelLoadDataHandler.BackgroundDataProvider);
        _globalSound.SetAudioClipProvider(_levelLoadDataHandler.AudioClipProvider);
        await _levelLoadDataHandler.LoadStartSeriaContent(StoryData);
        ConstructSound();
        ConstructBackground();
        await InitLevelUIProvider(phoneMessagesCustodian, phoneSaveHandler);
        _levelLocalizationHandler = new LevelLocalizationHandler(_gameSeriesHandlerBuildMode, _levelLocalizationProvider,
            _levelLoadDataHandler.CharacterProviderBuildMode,
            _gameStatsHandler, _levelUIProviderBuildMode.PhoneUIHandler, _levelLoadDataHandler.PhoneProviderInBuildMode,
            phoneMessagesCustodian, _setLocalizationChangeEvent);
        _levelUIProviderBuildMode.GameControlPanelUIHandler.SettingsPanelButtonUIHandler.InitInLevel(_levelLocalizationHandler);
        Init();
        
        var storiesProviderAssetProvider = new StoriesProviderAssetProvider();
        StoriesProvider sp = await storiesProviderAssetProvider.Load();
        int allSeriesCount = 0;
        foreach (var story in sp.Stories)
        {
            if (story.StoryName == StoryData.StoryName)
            {
                allSeriesCount = story.AllSeriesCount;
            }
        }
        storiesProviderAssetProvider.Release();
        LevelCompletePercentCalculator = new LevelCompletePercentCalculator(_gameSeriesHandlerBuildMode, allSeriesCount);
        
        await _globalUIHandler.LoadScreenUIHandler.HideOnLevelMove();
        _levelLoadDataHandler.LoadNextSeriesContent().Forget();
    }

    private async UniTask CreateBackgroundPool()
    {
        SpriteRendererAssetProvider spriteRendererAssetProvider = new SpriteRendererAssetProvider();
        var prefab = await spriteRendererAssetProvider.LoadSpriteRendererPrefab();
        _backgroundPool = new BackgroundPool(prefab, _backgroundBuildMode.PoolParent);
    }

    private async UniTask CreatePhoneView()
    {
        _phoneView = await new PhoneUIPrefabAssetProvider().CreatePhoneUIView();
    }
    private void Init()
    {
        if (LoadSaveData == true)
        {
            _gameStatsHandler.UpdateStatFromSave(StoryData.Stats);
            StoryData.StoryStarted = true;
        }
        InitLocalization();
        ViewerCreatorBuildMode viewerCreatorBuildMode = new ViewerCreatorBuildMode(PrefabsProvider.SpriteViewerAssetProvider);
        CharacterViewer.Construct(viewerCreatorBuildMode);
        ConstructWardrobeCharacterViewer(viewerCreatorBuildMode);
        NodeGraphInitializer = new NodeGraphInitializer(_levelLoadDataHandler.CharacterProviderBuildMode.CustomizableCharacterIndexesCustodians,
            _characterProvider, _backgroundBuildMode,
            _levelUIProviderBuildMode, CharacterViewer, _wardrobeCharacterViewer,
            _globalSound, _wallet, _levelLoadDataHandler.SeriaGameStatsProviderBuild, _levelLoadDataHandler.PhoneProviderInBuildMode,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent, _setLocalizationChangeEvent, _phoneNodeIsActive);

        if (StoryData == null)
        {
            _gameSeriesHandlerBuildMode.Construct(_levelLocalizationHandler, _levelLoadDataHandler.GameSeriesProvider,
                NodeGraphInitializer, _currentSeriaIndexReactiveProperty, SwitchToNextSeriaEvent,
                _onContentIsLoadProperty, _onAwaitLoadContentEvent, _currentSeriaLoadedNumberProperty, _onEndGameEvent);
        }
        else
        {
            _gameSeriesHandlerBuildMode.Construct(_levelLocalizationHandler, _levelLoadDataHandler.GameSeriesProvider,
                NodeGraphInitializer, _currentSeriaIndexReactiveProperty, SwitchToNextSeriaEvent, _onContentIsLoadProperty,
                _onAwaitLoadContentEvent, _currentSeriaLoadedNumberProperty, _onEndGameEvent,
                StoryData.CurrentSeriaIndex, StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeIndex, StoryData.PutOnSwimsuitKey);
            _levelUIProviderBuildMode.PhoneUIHandler.TryRestartPhoneTime(StoryData.CurrentPhoneMinute);
        }
    }
    private void OnApplicationQuit()
    {
        Save();
        Shutdown();
        _cancellationTokenSource?.Cancel();
    }

    protected override void ConstructBackground()
    {
        if (LoadSaveData == true && StoryData.BackgroundSaveData != null)
        {
            _backgroundBuildMode.InitSaveData(StoryData.BackgroundSaveData);
        }
        _backgroundBuildMode.Construct(CharacterViewer, _backgroundPool);
    }
    protected override void Shutdown()
    {
        _levelLocalizationHandler.Shutdown();
        _setLocalizationChangeEvent.Shutdown();
        _panelsLocalizationHandler.UnsubscribeChangeLanguage();
        _gameSeriesHandlerBuildMode.Shutdown();
        _levelLoadDataHandler.Shutdown();
        _levelUIProviderBuildMode.Shutdown();
        _globalSound.ShutdownFromLevel();
        _levelUISpriteAtlasAssetProvider.Release();
        base.Shutdown();
    }
    private void Save()
    {
        if (LoadSaveData == true)
        {
            SaveServiceProvider.SaveData.Monets = _wallet.GetMonetsCount;
            SaveServiceProvider.SaveData.Hearts = _wallet.GetHeartsCount;
            SaveServiceProvider.SaveData.SoundStatus = _globalSound.SoundStatus.Value;
            
            StoryData.PutOnSwimsuitKey = _gameSeriesHandlerBuildMode.PutOnSwimsuitKeyProperty;
            _gameSeriesHandlerBuildMode.GetInfoToSave(StoryData);
            
            _gameStatsHandler.FillSaveStats(StoryData);
            _backgroundBuildMode.FillSaveData(StoryData);
            StoryData.WardrobeSaveDatas.Clear();
            StoryData.WardrobeSaveDatas.AddRange(SaveService.CreateWardrobeSaveDatas(_levelLoadDataHandler.CharacterProviderBuildMode.CustomizableCharacterIndexesCustodians));
            _globalSound.FillStoryDataToSave(StoryData);
            StoryData.CurrentProgressPercent = LevelCompletePercentCalculator.GetLevelProgressPercent();
            StoryData.CustomizableCharacterIndex = _wardrobeCharacterViewer.CustomizableCharacterIndex;
            _levelLoadDataHandler.PhoneProviderInBuildMode.FillPhoneSaveInfo(StoryData);
            SaveServiceProvider.SaveData.StoryDatas[SaveServiceProvider.CurrentStoryKey] = StoryData;
            SaveServiceProvider.SaveLevelProgress();
        }
    }

    private async UniTask InitLevelUIProvider(PhoneMessagesCustodian phoneMessagesCustodian, PhoneSaveHandler phoneSaveHandler)
    {
        LevelCanvasAssetProvider levelCanvasAssetProvider = new LevelCanvasAssetProvider();
        _levelUISpriteAtlasAssetProvider = new LevelUISpriteAtlasAssetProvider();
        await _levelUISpriteAtlasAssetProvider.LoadSpriteAtlas(StoryData.NameUISpriteAtlas);
        LevelUIView = await levelCanvasAssetProvider.CreateAsset();
        LevelUIView.NarrativePanelUI.Image.sprite =
            _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.NarrativePanelName);
        LevelUIView.NotificationPanelUI.Image.sprite = 
            _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.NotificationPanelName);
        LevelUIView.CharacterPanelUI.Image.sprite = 
            _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.DialogPanelName);
        
        ResourcePanelsSettingsAssetProvider resourcePanelsSettingsAssetProvider = new ResourcePanelsSettingsAssetProvider();
        ResourcePanelsSettingsProvider resourcePanelsSettingsProvider = await resourcePanelsSettingsAssetProvider.LoadLocalizationHandlerAsset();
        ResourcePanelPrefabProvider resourcePanelPrefabProvider = new ResourcePanelPrefabProvider();
        ResourcePanelHandler monetResourcePanelHandler = new ResourcePanelHandler();
        ResourcePanelHandler heartsResourcePanelHandler = new ResourcePanelHandler();

        monetResourcePanelHandler.Init(await resourcePanelPrefabProvider.CreateAsset(LevelUIView.MonetPanelRectTransform), _wallet.GetMonetsCount,
            resourcePanelsSettingsProvider.MonetPanelColor, resourcePanelsSettingsProvider.MonetPanelButtonColor, _wallet.MonetsCountChanged);
        heartsResourcePanelHandler.Init(await resourcePanelPrefabProvider.CreateAsset(LevelUIView.HeartsPanelRectTransform), _wallet.GetHeartsCount,
            resourcePanelsSettingsProvider.HeartsPanelColor, resourcePanelsSettingsProvider.HeartsPanelButtonColor, _wallet.HeartsCountChanged);
        monetResourcePanelHandler.SetSprite(resourcePanelsSettingsProvider.MonetSprite);
        heartsResourcePanelHandler.SetSprite(resourcePanelsSettingsProvider.HeartsSprite);

        PanelResourceVisionHandler panelResourceVisionHandler = new PanelResourceVisionHandler(monetResourcePanelHandler, heartsResourcePanelHandler);
        
        
        if (LevelUIView.TryGetComponent(out Canvas canvas))
        {
            canvas.worldCamera = Camera.main;
        }
        await TryCreateBlackFrameUIHandler();
        
        ChoicePanelCasePrefabProvider choicePanelCasePrefabProvider = new ChoicePanelCasePrefabProvider();
        List<ChoiceCaseView> choiceCasesViews = new List<ChoiceCaseView>(ChoiceNode.MaxCaseCount);
        for (int i = 0; i < ChoiceNode.MaxCaseCount; i++)
        {
            var caseChoice = await choicePanelCasePrefabProvider.InstantiatePrefab(LevelUIView.ChoicePanelUI.transform);
            ChoiceCaseView choiceCaseView = caseChoice.GetComponent<ChoiceCaseView>();
            choiceCaseView.ButtonChoice.image.sprite =
                _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.NarrativePanelName);
            choiceCasesViews.Add(choiceCaseView);
        }
        
        
        CustomizationCharacterPanelUI customizationCharacterPanelUI =
            PrefabsProvider.CustomizationCharacterPanelAssetProvider.CreateCustomizationCharacterPanelUI(LevelUIView
                .transform);
        customizationCharacterPanelUI.PanelImage.sprite = _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.PanelWardrobeName);
        customizationCharacterPanelUI.LeftArrowImage.sprite = _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.ArrowName);
        customizationCharacterPanelUI.RightArrowImage.sprite = _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.ArrowName);
        customizationCharacterPanelUI.SkinImage.sprite = _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.BodyIconName);
        customizationCharacterPanelUI.HairImage.sprite = _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.HairstyleIconName);
        customizationCharacterPanelUI.ClothImage.sprite = _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.ClothesIconName);
        customizationCharacterPanelUI.PlayButtonImage.sprite = _levelUISpriteAtlasAssetProvider.GetSprite(LevelUISpriteAtlasAssetProvider.NarrativePanelName);
        
        customizationCharacterPanelUI.transform.SetSiblingIndex(customizationCharacterPanelUI.SiblingIndex);
        customizationCharacterPanelUI.gameObject.SetActive(false);
        _cancellationTokenSource = new CancellationTokenSource();
        
        ButtonTransitionToMainSceneUIHandler buttonTransitionToMainSceneUIHandler =
            new ButtonTransitionToMainSceneUIHandler(_globalUIHandler.LoadScreenUIHandler, PreSceneTransition);
        

        var shopMoneyButtonsUIHandler = new ShopMoneyButtonsUIHandler(_globalUIHandler.ShopMoneyPanelUIHandler);
        shopMoneyButtonsUIHandler.InitFromGameControlPanel(LevelUIView.GameControlPanelView.ShopMoneyButtonView,
            monetResourcePanelHandler, heartsResourcePanelHandler);


        var gameControlPanelUIHandler = new GameControlPanelUIHandler(LevelUIView.GameControlPanelView, _globalUIHandler,
            _globalSound, _panelsLocalizationHandler, _darkeningBackgroundFrameUIHandler,
            buttonTransitionToMainSceneUIHandler, _blockGameControlPanelUIEvent);


        _levelUIProviderBuildMode = new LevelUIProviderBuildMode(LevelUIView, gameControlPanelUIHandler, shopMoneyButtonsUIHandler, choiceCasesViews,
            _darkeningBackgroundFrameUIHandler, _wallet, DisableNodesContentEvent, SwitchToNextNodeEvent,
            customizationCharacterPanelUI, _globalUIHandler, buttonTransitionToMainSceneUIHandler,
            _levelLoadDataHandler.LoadAssetsPercentHandler, _onAwaitLoadContentEvent, _onEndGameEvent,
            _levelLoadDataHandler.PhoneProviderInBuildMode.PhoneContentProvider, panelResourceVisionHandler,
            () =>
            {
                LevelUIView.PhoneUIView = _phoneView;
                _phoneView.transform.SetParent(LevelUIView.transform);
                LevelUIView.PhoneUIView.transform.SetSiblingIndex(PhoneUIHandler.PhoneSiblingIndex);
                _levelUIProviderBuildMode.PhoneUIHandler.Init(LevelUIView.PhoneUIView, phoneMessagesCustodian, phoneSaveHandler, _gameSeriesHandlerBuildMode.GetNodePort);
            });
    }
    private void InitLocalization()
    {
        _panelsLocalizationHandler.SetLocalizableContentFromLevel(_levelUIProviderBuildMode.GetLocalizableContent());
        _panelsLocalizationHandler.SubscribeChangeLanguage();
        _panelsLocalizationHandler.SetLanguagePanelsAndMenuStory();
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
    protected override void ConstructSound()
    {
        if (LoadSaveData == true)
        {
            _globalSound.Init(StoryData.AudioEffectsIsOn ,StoryData.CurrentAudioMusicKey, StoryData.CurrentAudioAmbientKey);
            _globalSound.TryPlayOnLoadSave().Forget();
        }
        else
        {
            _globalSound.Init();
        }
    }

    protected override void ConstructWardrobeCharacterViewer(ViewerCreator viewerCreator)
    {
        _wardrobeCharacterViewer =
            PrefabsProvider.WardrobeCharacterViewerAssetProvider.CreateWardrobeCharacterViewer(transform);
        _wardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreator);
        var ps = PrefabsProvider.WardrobePSProvider.CreateWardrobePS(_wardrobeCharacterViewer.transform);
        _wardrobeCharacterViewer.InitParticleSystem(ps);
        PrefabsProvider.SpriteViewerAssetProvider.UnloadAsset();
    }

    private async UniTask PreSceneTransition()
    {
        Save();
        Shutdown();
        await UniTask.WhenAll(
            _globalSound.SmoothAudio.SmoothStopAudio(_cancellationTokenSource.Token, AudioSourceType.Music),
            _globalSound.SmoothAudio.SmoothStopAudio(_cancellationTokenSource.Token, AudioSourceType.Ambient));
    }
}