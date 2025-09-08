using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;


[CreateAssetMenu(fileName = "CharactersData", menuName = "Character/CharactersData", order = 51)]
public class CharactersData : ScriptableObject
{
    [SerializeField, Expandable] private List<SimpleCharacter> _simpleCharacters;
    
    public List<SimpleCharacter> SimpleCharacters => _simpleCharacters;
}