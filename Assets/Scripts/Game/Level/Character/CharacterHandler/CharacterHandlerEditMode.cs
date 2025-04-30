
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterHandlerEditMode : MonoBehaviour, ICharacterHandler
{
    [SerializeField, BoxGroup("Simple Characters: ")] private List<SimpleCharacter> _simpleCharacters;
    [SerializeField, BoxGroup("CustomizableCharacter: ")] private CustomizableCharacter _customizableCharacter;

    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("All Characters: "), ReadOnly] private List<Character> _allCharacters;
    public CustomizableCharacter CustomizableCharacter => _customizableCharacter;


    public IReadOnlyList<Character> GetCharacters()
    {
        _allCharacters = new List<Character>();
        _allCharacters.Add(_customizableCharacter);
        _allCharacters.AddRange(_simpleCharacters);
        return _allCharacters;
    }
}