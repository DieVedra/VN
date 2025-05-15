using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "CharactersData", menuName = "Character/CharactersData", order = 51)]
public class CharactersData : ScriptableObject
{
    [SerializeField] private List<SimpleCharacter> _simpleCharacters;
    
    public List<SimpleCharacter> SimpleCharacters => _simpleCharacters;
}