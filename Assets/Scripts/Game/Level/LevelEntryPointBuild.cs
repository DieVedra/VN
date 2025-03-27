using NaughtyAttributes;
using UniRx;
using UnityEngine;
using Zenject;

public class LevelEntryPointBuild : LevelEntryPoint
{
    [SerializeField, ReadOnly] private WardrobeCharacterViewer _wardrobeCharacterViewer;
    [SerializeField, ReadOnly] private BackgroundContent _wardrobeBackground;

    [SerializeField, Expandable] protected AudioData _audioData;
    [Inject] private GlobalSound _globalSound;
    private SwitchToNextNodeEvent _switchToNextNodeEvent;
    private SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> _switchToAnotherNodeGraphEvent;
    private DisableNodesContentEvent _disableNodesContentEvent;
    private NodeGraphInitializer _nodeGraphInitializer;
    private LevelUIProvider _levelUIProvider;
    private Wallet _wallet;
    private SaveData _saveData;
    private StoryData _storyData;
    private ReactiveCommand _onSceneTransition;
    private async void Awake()
    {
        Init();
        _onSceneTransition.Subscribe(_ =>
        {
            CharacterViewer.Dispose();
            GameSeriesHandler.Dispose();
            _levelUIProvider.Dispose();
            EventSystem.gameObject.SetActive(false);
            Save();
        });
        //
        // var handle = Addressables.LoadAssetAsync<SeriaPartNodeGraph>("8 SeriaPartNodeGraph 2.1");
        // _test1 = await handle.Task;
    }

    public void Init()
    {
        if (LoadSaveData == true)
        {
            _saveData = SaveServiceProvider.SaveData;
            _storyData = _saveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            TestMonets = _saveData.Monets;
            TestHearts = _saveData.Hearts;
            _wallet = new Wallet(_saveData);
            GameStatsCustodian.Init(_storyData.Stats);
            _globalSound.Init(_saveData.SoundIsOn);
            _globalSound.SetAudioData(_audioData);
        }
        else
        {
            _wallet = new Wallet(TestMonets, TestHearts);
            GameStatsCustodian.Init();
            _globalSound.Init();
            _globalSound.SetAudioData(_audioData);
        }

        _onSceneTransition = new ReactiveCommand();
        _switchToNextNodeEvent = new SwitchToNextNodeEvent();
        _switchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        _disableNodesContentEvent = new DisableNodesContentEvent();

        InitLevelUIProvider();
        ViewerCreatorBuildMode viewerCreatorBuildMode = new ViewerCreatorBuildMode();
        CharacterViewer.Construct(_disableNodesContentEvent, viewerCreatorBuildMode);
        InitWardrobeCharacterViewer(viewerCreatorBuildMode);

        SpriteRendererCreator spriteRendererCreator = new SpriteRendererCreatorBuild();
        InitBackground(spriteRendererCreator);
        
        _nodeGraphInitializer = new NodeGraphInitializer(Characters, Background.GetBackgroundContent, Background,
            _levelUIProvider,
            CharacterViewer, _wardrobeCharacterViewer, CustomizableCharacter, _globalSound, GameStatsCustodian,
            _wallet,
            _switchToNextNodeEvent, _switchToAnotherNodeGraphEvent, _disableNodesContentEvent, SwitchToNextSeriaEvent);

        if (_saveData == null)
        {
            GameSeriesHandler.Construct(_nodeGraphInitializer, SwitchToNextSeriaEvent);

        }
        else
        {
            GameSeriesHandler.Construct(_nodeGraphInitializer, SwitchToNextSeriaEvent,
                _storyData.CurrentNodeGraphIndex, _storyData.CurrentNodeIndex);
        }


    }

    private void OnApplicationQuit()
    {
        CharacterViewer.Dispose();
        GameSeriesHandler.Dispose();
        _levelUIProvider.Dispose();
        EventSystem.gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        if (LoadSaveData == true)
        {
            _storyData.CurrentNodeGraphIndex = GameSeriesHandler.CurrentNodeGraphIndex;
            _storyData.CurrentNodeIndex = GameSeriesHandler.CurrentNodeIndex;
            _storyData.Stats = GameStatsCustodian.GetSaveStatsToSave();
            _saveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = _storyData;
            SaveServiceProvider.SaveService.Save(_saveData);
        }
    }

    private void InitLevelUIProvider()
    {
        LevelUIView.CustomizationCharacterPanelUI.gameObject.SetActive(false);
        CustomizationCharacterPanelUI customizationCharacterPanelUI =
            PrefabsProvider.CustomizationCharacterPanelAssetProvider.CreateCustomizationCharacterPanelUI(LevelUIView
                .transform);
        customizationCharacterPanelUI.transform.SetSiblingIndex(customizationCharacterPanelUI.SublingIndex);
        _levelUIProvider = new LevelUIProvider(LevelUIView, _wallet, _onSceneTransition, _disableNodesContentEvent,
            _switchToNextNodeEvent, customizationCharacterPanelUI);
    }

    protected override void InitWardrobeCharacterViewer(ViewerCreator viewerCreator)
    {
        if (Application.isPlaying)
        {
            Destroy(_wardrobeCharacterViewer.gameObject);

            _wardrobeCharacterViewer =
                PrefabsProvider.WardrobeCharacterViewerAssetProvider.CreateWardrobeCharacterViewer(transform);
            _wardrobeCharacterViewer.Construct(_disableNodesContentEvent, viewerCreator);
        }
    }

    protected override void InitBackground(SpriteRendererCreator spriteRendererCreator)
    {
        Background.Construct(_disableNodesContentEvent, CharacterViewer, spriteRendererCreator, _wardrobeBackground);
    }
}