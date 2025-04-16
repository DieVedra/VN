using System.Collections.Generic;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using Zenject;

public class LevelEntryPointBuild : LevelEntryPoint
{
    [SerializeField, ReadOnly] private WardrobeCharacterViewer _wardrobeCharacterViewer;
    [SerializeField, ReadOnly] private BackgroundContent _wardrobeBackground;

    [SerializeField, Expandable] private AudioData _audioData;
    [SerializeField, Expandable] private AudioData _additionalAudioData;
    [Inject] private GlobalSound _globalSound;
    private async void Awake()
    {
        Init();
        OnSceneTransition.Subscribe(_ =>
        {
            CharacterViewer.Dispose();
            GameSeriesHandler.Dispose();
            LevelUIProvider.Dispose();
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
            SaveData = SaveServiceProvider.SaveData;
            StoryData = SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex];
            TestMonets = SaveData.Monets;
            TestHearts = SaveData.Hearts;
            Wallet = new Wallet(SaveData);
            GameStatsCustodian.Init(StoryData.Stats);
            // _globalSound.Init(SaveData.SoundIsOn);
        }
        else
        {
            Wallet = new Wallet(TestMonets, TestHearts);
            GameStatsCustodian.Init();
            // _globalSound.Init();
        }
        InitGlobalSound();

        OnSceneTransition = new ReactiveCommand();
        SwitchToNextNodeEvent = new SwitchToNextNodeEvent();
        SwitchToAnotherNodeGraphEvent = new SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph>();
        DisableNodesContentEvent = new DisableNodesContentEvent();

        InitLevelUIProvider();
        ViewerCreatorBuildMode viewerCreatorBuildMode = new ViewerCreatorBuildMode();
        CharacterViewer.Construct(DisableNodesContentEvent, viewerCreatorBuildMode);
        InitWardrobeCharacterViewer(viewerCreatorBuildMode);

        SpriteRendererCreator spriteRendererCreator = new SpriteRendererCreatorBuild();
        InitBackground(spriteRendererCreator, _wardrobeBackground);
        
        NodeGraphInitializer = new NodeGraphInitializer(Characters, Background.GetBackgroundContent, Background,
            LevelUIProvider,
            CharacterViewer, _wardrobeCharacterViewer, CustomizableCharacter, _globalSound, GameStatsCustodian,
            Wallet,
            SwitchToNextNodeEvent, SwitchToAnotherNodeGraphEvent, DisableNodesContentEvent, SwitchToNextSeriaEvent);

        if (SaveData == null)
        {
            GameSeriesHandler.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent);

        }
        else
        {
            GameSeriesHandler.Construct(NodeGraphInitializer, SwitchToNextSeriaEvent, 
                StoryData.CurrentSeriaIndex, StoryData.CurrentNodeGraphIndex, StoryData.CurrentNodeIndex);
        }


    }

    private void OnApplicationQuit()
    {
        CharacterViewer.Dispose();
        GameSeriesHandler.Dispose();
        LevelUIProvider.Dispose();
        EventSystem.gameObject.SetActive(false);
        Save();
    }

    private void Save()
    {
        if (LoadSaveData == true)
        {
            StoryData.CurrentNodeGraphIndex = GameSeriesHandler.CurrentNodeGraphIndex;
            StoryData.CurrentNodeIndex = GameSeriesHandler.CurrentNodeIndex;
            StoryData.Stats = GameStatsCustodian.GetSaveStatsToSave();
            StoryData.BackgroundSaveData = Background.GetBackgroundSaveData();
            
            StoryData.CurrentAudioClipIndex = _globalSound.CurrentMusicClipIndex;
            StoryData.LowPassEffectIsOn = _globalSound.AudioEffectsCustodian.LowPassEffectIsOn;

            SaveData.StoryDatas[SaveServiceProvider.CurrentStoryIndex] = StoryData;
            SaveServiceProvider.SaveService.Save(SaveData);
        }
    }

    private void InitLevelUIProvider()
    {
        LevelUIView.CustomizationCharacterPanelUI.gameObject.SetActive(false);
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
            _globalSound.Init(SaveData.SoundIsOn);
        }
        else
        {
            _globalSound.Init();
        }
        _globalSound.SetAudioDatas(new List<AudioData> {_audioData}, new List<AudioData> {_additionalAudioData});
    }

    protected override void InitWardrobeCharacterViewer(ViewerCreator viewerCreator)
    {
        if (Application.isPlaying)
        {
            // Destroy(_wardrobeCharacterViewer.gameObject);

            _wardrobeCharacterViewer =
                PrefabsProvider.WardrobeCharacterViewerAssetProvider.CreateWardrobeCharacterViewer(transform);
            _wardrobeCharacterViewer.Construct(DisableNodesContentEvent, viewerCreator);
            PrefabsProvider.SpriteViewerAssetProvider.UnloadAsset();
        }
    }
}