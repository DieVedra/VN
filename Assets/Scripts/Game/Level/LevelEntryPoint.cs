
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public abstract class LevelEntryPoint : MonoBehaviour
{
    [SerializeField] protected GameObject EventSystem;
    [SerializeField] protected CharacterViewer CharacterViewer;
    [SerializeField] protected GameSeriesHandler GameSeriesHandler;
    [SerializeField] protected List<Character> Characters;
    [SerializeField] protected CustomizableCharacter CustomizableCharacter;
    [SerializeField] protected Background Background;
    [SerializeField] protected LevelUIView LevelUIView;
    [SerializeField] protected GameStatsCustodian GameStatsCustodian;

    [SerializeField, Space(10f)] protected int TestMonets;
    [SerializeField, Space(10f)] protected int TestHearts;
    
    [SerializeField] protected bool LoadSaveData;

    protected Wallet Wallet;
    protected SaveData SaveData;
    protected StoryData StoryData;
    protected SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    protected ReactiveCommand OnSceneTransition;
    protected LevelUIProvider LevelUIProvider;
    protected NodeGraphInitializer NodeGraphInitializer;
    protected DisableNodesContentEvent DisableNodesContentEvent;
    protected SwitchToNextNodeEvent SwitchToNextNodeEvent;
    protected SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> SwitchToAnotherNodeGraphEvent;


    protected abstract void InitWardrobeCharacterViewer(ViewerCreator viewerCreator);
    protected abstract void InitGlobalSound();

    protected void InitBackground(SpriteRendererCreator spriteRendererCreator, BackgroundContent wardrobeBackground)
    {
        if (LoadSaveData == true)
        {
            Background.ConstructSaveOn(
                StoryData.BackgroundSaveData,
                DisableNodesContentEvent, CharacterViewer, spriteRendererCreator, wardrobeBackground);
        }
        else
        {
            Background.ConstructSaveOff(DisableNodesContentEvent, CharacterViewer, spriteRendererCreator, wardrobeBackground);
        }
    }
}