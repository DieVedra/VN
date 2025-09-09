using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharactersProviderSeria", menuName = "Character/CharactersProviderSeria", order = 51)]
public class CharactersProvider : ScriptableObject
{
    [SerializeField] private List<CharacterInfo> _charactersInfo;
    
    public IReadOnlyList<CharacterInfo> CharactersInfo => _charactersInfo;
}