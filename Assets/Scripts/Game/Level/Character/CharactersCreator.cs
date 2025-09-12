using System.Collections.Generic;
using System.Linq;

public class CharactersCreator
{
    private IReadOnlyList<CharactersDataProvider> _charactersDataProviders;
    private IReadOnlyList<CharactersProvider> _charactersProvider;
    private Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;

    public CharactersCreator(IReadOnlyList<CharactersDataProvider> charactersDataProviders, IReadOnlyList<CharactersProvider> charactersProvider,
        Dictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians)
    {
        _charactersDataProviders = charactersDataProviders;
        _charactersProvider = charactersProvider;
        _customizableCharacterIndexesCustodians = customizableCharacterIndexesCustodians;
    }

    public List<Character> CreateCharactersToSeria(int seriaIndex)
    {
        List<Character> characters = new List<Character>();
        List<CharacterInfo> combinedCharactersInfo = CombineCharactersInfoBySeria(seriaIndex);
        CharacterInfo info;
        for (int i = 0; i < combinedCharactersInfo.Count; i++)
        {
            info = combinedCharactersInfo[i];
            if (info.IsCustomizationCharacter == true)
            {
                CustomizableCharacterIndexesCustodian customizableCharacterIndexesCustodian;
                if (_customizableCharacterIndexesCustodians.TryGetValue(info.NameKey, out CustomizableCharacterIndexesCustodian value))
                {
                    customizableCharacterIndexesCustodian = value;
                }
                else
                {
                    customizableCharacterIndexesCustodian = new CustomizableCharacterIndexesCustodian(combinedCharactersInfo[i].NameKey);
                    _customizableCharacterIndexesCustodians.Add(info.NameKey, customizableCharacterIndexesCustodian);
                }
                List<CustomizationCharacterData> customizationCharacterData = CombineCharactersDataToSeria<CustomizationCharacterData>(info.NameKey, seriaIndex);
                characters.Add(new CustomizableCharacter(customizableCharacterIndexesCustodian, customizationCharacterData, info.LocalizationString));
            }
            else
            {
                List<CharacterData> characterData = CombineCharactersDataToSeria<CharacterData>(info.NameKey, seriaIndex);
                characters.Add(new SimpleCharacter(characterData, info.LocalizationString, seriaIndex));
            }
        }
        return characters;
    }

    private List<CharacterInfo> CombineCharactersInfoBySeria(int seriaIndex)
    {
        Dictionary<string, CharacterInfo> dictionary = new Dictionary<string, CharacterInfo>();
        CharactersProvider provider;
        CharacterInfo info;
        int count;
        for (int i = 0; i < _charactersProvider.Count; i++)
        {
            provider = _charactersProvider[i];
            if (provider.SeriaIndex <= seriaIndex)
            {
                count = provider.CharactersInfo.Count;
                for (int j = 0; j < count; j++)
                {
                    info = _charactersProvider[i].CharactersInfo[j];
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

    private List<T> CombineCharactersDataToSeria<T>(string nameKey, int seriaIndex) where T : BaseCharacterData
    {
        List<T> selectedCharacterDatas = new List<T>();
        int count = 0;
        CharactersDataProvider provider;

        for (int i = 0; i < _charactersDataProviders.Count; i++)
        {
            provider = _charactersDataProviders[i];
            if (provider.SeriaIndex <= seriaIndex)
            {
                count = provider.CharactersDatas.Count;
                for (int j = 0; j < count; j++)
                {
                    var characterData = provider.CharactersDatas[j];
                    if (characterData.MySeriaIndex <= seriaIndex && characterData.CharacterNameKey == nameKey)
                    {
                        if (characterData is T data)
                        {
                            selectedCharacterDatas.Add(data);
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