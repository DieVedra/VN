using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CharactersDataProvider", menuName = "Character/CharactersDataProvider", order = 51)]
public class CharactersDataProvider : ScriptableObject
{
    [SerializeField] private int _seriaIndex;
    [SerializeField, Expandable] private List<BaseCharacterData> _charactersDatas;
    
    public IReadOnlyList<BaseCharacterData> CharactersDatas => _charactersDatas;
    public int SeriaIndex => _seriaIndex;
}