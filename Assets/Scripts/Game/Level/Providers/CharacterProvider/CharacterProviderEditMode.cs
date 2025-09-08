using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CharacterProviderEditMode : MonoBehaviour, ICharacterProvider
{
    [SerializeField, BoxGroup("Simple Characters Data: "), Expandable] private List<CharactersData> _charactersDatas;
    [SerializeField, BoxGroup("CustomizableCharacter: "), Expandable] private List<CustomizableCharacter> _customizableCharacters;
    [SerializeField, BoxGroup("AdditionalCharactersSpriteDatas: "), Expandable] private List<AdditionalCharactersSpriteDatas> _additionalCharactersSpriteDatas;
    

    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("All Characters: "), ReadOnly] private List<Character> _allCharacters;
    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters => _customizableCharacters;
    public IReadOnlyList<AdditionalCharactersSpriteDatas> AdditionalCharactersSpriteDatas => _additionalCharactersSpriteDatas;

    public void Construct(WardrobeSaveData[] wardrobeSaveDatas = null)
    {
        if (wardrobeSaveDatas != null)
        {
            for (int i = 0; i < CustomizableCharacters.Count; i++)
            {
                _customizableCharacters[i].Construct(wardrobeSaveDatas[i]);
            }
        }
        _allCharacters = new List<Character>();
        for (int i = 0; i < _customizableCharacters.Count; i++)
        {
            _allCharacters.Add(_customizableCharacters[i]);
        }
        for (int i = 0; i < _charactersDatas.Count; i++)
        {
            _allCharacters.AddRange(_charactersDatas[i].SimpleCharacters);
        }
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return _allCharacters;
    }
}