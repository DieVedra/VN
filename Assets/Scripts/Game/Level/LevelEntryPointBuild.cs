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
    private SwitchToAnotherNodeGraphEvent _switchToAnotherNodeGraphEvent;
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
            _characterViewer.Dispose();
            _nodeGraphsHandler.Dispose();
            _levelUIProvider.Dispose();
            _eventSystem.gameObject.SetActive(false);
            Save();
        });
        //
        // var handle = Addressables.LoadAssetAsync<LevelPartNodeGraph>("8 LevelPartNodeGraph 2.1");
        // _test1 = await handle.Task;
    }

    public void Init()
    {
        if (_loadSaveData == true)
        {
            _saveData = SaveServiceProvider.SaveData;
            _storyData = _saveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            _testMonets = _saveData.Monets;
            _testHearts = _saveData.Hearts;
            _wallet = new Wallet(_saveData);
            _gameStatsCustodian.Init(_storyData.Stats);
            _globalSound.Init(_saveData.SoundIsOn);
            _globalSound.SetAudioData(_audioData);
        }
        else
        {
            _wallet = new Wallet(_testMonets, _testHearts);
            _gameStatsCustodian.Init();
            _globalSound.Init();
            _globalSound.SetAudioData(_audioData);
        }

        _onSceneTransition = new ReactiveCommand();
        _switchToNextNodeEvent = new SwitchToNextNodeEvent();
        _switchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent();
        _disableNodesContentEvent = new DisableNodesContentEvent();

        InitLevelUIProvider();
        ViewerCreatorBuildMode viewerCreatorBuildMode = new ViewerCreatorBuildMode();
        _characterViewer.Construct(_disableNodesContentEvent, viewerCreatorBuildMode);
        InitWardrobeCharacterViewer(viewerCreatorBuildMode);

        SpriteRendererCreator spriteRendererCreator = new SpriteRendererCreatorBuild();
        InitBackground(spriteRendererCreator);
        
        _nodeGraphInitializer = new NodeGraphInitializer(_characters, _background.GetBackgroundContent, _background,
            _levelUIProvider,
            _characterViewer, _wardrobeCharacterViewer, _customizableCharacter, _globalSound, _gameStatsCustodian,
            _wallet,
            _switchToNextNodeEvent, _switchToAnotherNodeGraphEvent, _disableNodesContentEvent);

        if (_saveData == null)
        {
            _nodeGraphsHandler.Init(_nodeGraphInitializer, _switchToNextNodeEvent, _switchToAnotherNodeGraphEvent);

        }
        else
        {
            _nodeGraphsHandler.Init(_nodeGraphInitializer, _switchToNextNodeEvent, _switchToAnotherNodeGraphEvent,
                _storyData.CurrentNodeGraphIndex, _storyData.CurrentNodeIndex);
        }


    }

    private void OnApplicationQuit()
    {
        _characterViewer.Dispose();
        _nodeGraphsHandler.Dispose();
        _levelUIProvider.Dispose();
        _eventSystem.gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        if (_loadSaveData == true)
        {
            _storyData.CurrentNodeGraphIndex = _nodeGraphsHandler.CurrentNodeGraphIndex;
            _storyData.CurrentNodeIndex = _nodeGraphsHandler.CurrentNodeIndex;
            _storyData.Stats = _gameStatsCustodian.GetSaveStatsToSave();
            _saveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = _storyData;
            SaveServiceProvider.SaveService.Save(_saveData);
        }
    }

    private void InitLevelUIProvider()
    {
        _levelUIView.CustomizationCharacterPanelUI.gameObject.SetActive(false);
        CustomizationCharacterPanelUI customizationCharacterPanelUI =
            PrefabsProvider.CustomizationCharacterPanelAssetProvider.CreateCustomizationCharacterPanelUI(_levelUIView
                .transform);
        customizationCharacterPanelUI.transform.SetSiblingIndex(customizationCharacterPanelUI.SublingIndex);
        _levelUIProvider = new LevelUIProvider(_levelUIView, _wallet, _onSceneTransition, _disableNodesContentEvent,
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
        _background.Construct(_disableNodesContentEvent, _characterViewer, spriteRendererCreator, _wardrobeBackground);
    }
}