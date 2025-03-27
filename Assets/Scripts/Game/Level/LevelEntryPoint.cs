
using System.Collections.Generic;
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

    protected SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;


    protected abstract void InitWardrobeCharacterViewer(ViewerCreator viewerCreator);
    protected abstract void InitBackground(SpriteRendererCreator spriteRendererCreator);
}