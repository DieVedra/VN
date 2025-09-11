using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class CharacterProviderEditMode : MonoBehaviour, ICharacterProvider
{
    [SerializeField, Expandable] private List<CharactersDataProvider> _charactersDataProviders;
    [SerializeField, Expandable] private List<CharactersProvider> _charactersProvider;
    
    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("All Characters: "), ReadOnly] private List<string> _allCharactersNames;
    private Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    private Dictionary<string, WardrobeSaveData> _wardrobeSaveData;

    public IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> CustomizableCharacterIndexesCustodians => _customizableCharacterIndexesCustodians;
    public IReadOnlyList<Character> GetCharacters(int seriaIndex)
    {
        return CreateCharactersToSeria(CombineCharactersInfoBySeria(seriaIndex), seriaIndex);
    }

    public void Construct(WardrobeSaveData[] wardrobeSaveDatas = null)
    {
        _customizableCharacterIndexesCustodians = new Dictionary<string, CustomizableCharacterIndexesCustodian>();
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
            _wardrobeSaveData = wardrobeSaveDatas.ToDictionary(x => x.NameKey, x=> x);
        }
    }
    

    private List<CharacterInfo> CombineCharactersInfoBySeria(int seriaIndex)
    {
        Dictionary<string, CharacterInfo> dictionary = new Dictionary<string, CharacterInfo>();
        for (int i = 0; i < _charactersProvider.Count; i++)
        {
            if (_charactersProvider[i].SeriaIndex <= seriaIndex)
            {
                foreach (var info in _charactersProvider[i].CharactersInfo)
                {
                    if (dictionary.ContainsKey(info.NameKey) == false)
                    {
                        dictionary.Add(info.NameKey, info);
                    }
                }
            }
            else
            {
                break;
            }
        }
        return dictionary.Select(x=>x.Value).ToList();
    }

    private List<Character> CreateCharactersToSeria(List<CharacterInfo> combinedCharactersInfo, int seriaIndex)
    {
        List<Character> characters = new List<Character>();
        for (int i = 0; i < combinedCharactersInfo.Count; i++)
        {
            if (combinedCharactersInfo[i].IsCustomizationCharacter == true)
            {
                CustomizableCharacterIndexesCustodian customizableCharacterIndexesCustodian;
                if (_wardrobeSaveData.TryGetValue(combinedCharactersInfo[i].NameKey, out WardrobeSaveData wardrobeSaveData))
                {
                    customizableCharacterIndexesCustodian = new CustomizableCharacterIndexesCustodian(wardrobeSaveData, combinedCharactersInfo[i].NameKey);
                }
                else
                {
                    customizableCharacterIndexesCustodian = new CustomizableCharacterIndexesCustodian(combinedCharactersInfo[i].NameKey);
                }

                _customizableCharacterIndexesCustodians.Add(combinedCharactersInfo[i].NameKey, customizableCharacterIndexesCustodian);
                List<CustomizationCharacterData> customizationCharacterData = CombineCharactersDataToSeria<CustomizationCharacterData>(combinedCharactersInfo[i].NameKey, seriaIndex);
                characters.Add(new CustomizableCharacter(customizableCharacterIndexesCustodian, customizationCharacterData));
            }
            else
            {
                List<CharacterData> characterData = CombineCharactersDataToSeria<CharacterData>(combinedCharactersInfo[i].NameKey, seriaIndex);
                characters.Add(new SimpleCharacter(characterData, seriaIndex));
            }
        }
        return characters;
    }

    private List<T> CombineCharactersDataToSeria<T>(string nameKey, int seriaIndex) where T : BaseCharacterData
    {
        List<T> selectedCharacterDatas = new List<T>();
        for (int i = 0; i < _charactersDataProviders.Count; i++)
        {
            if (_charactersDataProviders[i].SeriaIndex <= seriaIndex)
            {
                for (int j = 0; i < _charactersDataProviders[i].CharactersDatas.Count; j++)
                {
                    var characterData = (T) _charactersDataProviders[i].CharactersDatas[j];
                    if (characterData.MySeriaIndex <= seriaIndex && characterData.CharacterNameKey == nameKey)
                    {
                        if (characterData.GetType() == typeof(T))
                        {
                            selectedCharacterDatas.Add(characterData);
                        }
                    }
                }
            }
            else
            {
                break;
            }
        }
        return selectedCharacterDatas;
    }
}