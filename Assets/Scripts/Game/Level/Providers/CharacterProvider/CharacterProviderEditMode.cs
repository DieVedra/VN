using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class CharacterProviderEditMode : MonoBehaviour, ILocalizable
{
    [SerializeField, Expandable] private List<CharactersDataProvider> _charactersDataProviders;
    [SerializeField, Expandable] private List<CharactersProvider> _charactersProvider;
    
    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("All Characters: "), ReadOnly] private List<string> _allCharactersNames;
    private Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    private CharactersCreator _charactersCreator;
    public IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> CustomizableCharacterIndexesCustodians => _customizableCharacterIndexesCustodians;
    public ICharacterProvider CharacterProvider => _charactersCreator;
    public void Construct(IReadOnlyList<WardrobeSaveData> wardrobeSaveDatas = null)
    {
        _customizableCharacterIndexesCustodians = new Dictionary<string, CustomizableCharacterIndexesCustodian>();
        _charactersCreator = new CharactersCreator(_charactersDataProviders, _charactersProvider,
            _customizableCharacterIndexesCustodians);
        Dictionary<string, string> names = new Dictionary<string, string>();
        for (int i = 0; i < _charactersProvider.Count; i++)
        {
            for (int j = 0; j < _charactersProvider[i].CharactersInfo.Count; j++)
            {
                if (names.ContainsKey(_charactersProvider[i].CharactersInfo[j].NameKey) == false)
                {
                    names.Add(_charactersProvider[i].CharactersInfo[j].NameKey, _charactersProvider[i].CharactersInfo[j].Name);
                }
            }
        }
        _allCharactersNames = names.Select(x=>x.Value).ToList();
        
        if (wardrobeSaveDatas != null)
        {
            for (int i = 0; i < wardrobeSaveDatas.Count; i++)
            {
                _customizableCharacterIndexesCustodians.Add(
                    wardrobeSaveDatas[i].NameKey, 
                    new CustomizableCharacterIndexesCustodian(wardrobeSaveDatas[i], wardrobeSaveDatas[i].NameKey));
            }
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        int count;
        for (int i = 0; i < _charactersProvider.Count; i++)
        {
            count = _charactersProvider[i].CharactersInfo.Count;
            for (int j = 0; j < count; j++)
            {
                strings.Add(_charactersProvider[i].CharactersInfo[j].LocalizationString);
            }
        }
        return strings;
    }
}