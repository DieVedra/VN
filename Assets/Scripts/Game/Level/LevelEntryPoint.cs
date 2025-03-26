
using System.Collections.Generic;
using UnityEngine;

public abstract class LevelEntryPoint : MonoBehaviour
{
    [SerializeField] protected GameObject _eventSystem;
    [SerializeField] protected CharacterViewer _characterViewer;
    [SerializeField] protected NodeGraphsHandler _nodeGraphsHandler;
    [SerializeField] protected List<Character> _characters;
    [SerializeField] protected CustomizableCharacter _customizableCharacter;
    [SerializeField] protected Background _background;
    [SerializeField] protected LevelUIView _levelUIView;
    [SerializeField] protected GameStatsCustodian _gameStatsCustodian;

    [SerializeField, Space(10f)] protected int _testMonets;
    [SerializeField, Space(10f)] protected int _testHearts;
    
    [SerializeField] protected bool _loadSaveData;



    protected abstract void InitWardrobeCharacterViewer(ViewerCreator viewerCreator);
}