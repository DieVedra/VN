using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterHandler : MonoBehaviour
{
    [SerializeField, BoxGroup("Simple Characters")] private List<Character> _characters;
    [SerializeField, BoxGroup("CustomizableCharacter")] private CustomizableCharacter _customizableCharacter;

    public List<Character> Characters => _characters;
    public CustomizableCharacter CustomizableCharacter => _customizableCharacter;


    public void TryLoadNextSeriesCharacters()
    {
        
    }
}