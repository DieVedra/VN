using System.Collections.Generic;
using System.Linq;

public class CharactersCreator : ICharacterProvider
{
    private readonly IReadOnlyList<CharactersDataProvider> _charactersDataProviders;
    private readonly IReadOnlyList<CharactersProvider> _charactersProvider;
    private readonly Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;

    private readonly Dictionary<int, IReadOnlyList<Character>> _createdCharacters;
    private readonly Dictionary<int, IReadOnlyList<CustomizableCharacter>> _createdCustomizableCharacters;
    public CharactersCreator(IReadOnlyList<CharactersDataProvider> charactersDataProviders, IReadOnlyList<CharactersProvider> charactersProvider,
        Dictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians)
    {
        _charactersDataProviders = charactersDataProviders;
        _charactersProvider = charactersProvider;
        _customizableCharacterIndexesCustodians = customizableCharacterIndexesCustodians;
        _createdCharacters = new Dictionary<int, IReadOnlyList<Character>>();
        _createdCustomizableCharacters = new Dictionary<int, IReadOnlyList<CustomizableCharacter>>();
    }
    public IReadOnlyList<Character> GetCharacters(int seriaIndex)
    {
        IReadOnlyList<Character> result;
        if (_createdCharacters.TryGetValue(seriaIndex, out IReadOnlyList<Character> value))
        {
            result = value;
        }
        else
        {
            result = CreateCharactersToSeria(seriaIndex);
            _createdCharacters.Add(seriaIndex, result);
        }
        return result;
    }
    public IReadOnlyList<CustomizableCharacter> GetCustomizationCharacters(int seriaIndex)
    {
        IReadOnlyList<CustomizableCharacter> result = null;
        if (_createdCustomizableCharacters.TryGetValue(seriaIndex, out IReadOnlyList<CustomizableCharacter> extractedCustomizableCharacters))
        {
            result = extractedCustomizableCharacters;
        }
        else if(_createdCharacters.TryGetValue(seriaIndex, out IReadOnlyList<Character> extractedCharacters) == true)
        {
            result = SortAndGetCustomizationCharacters(extractedCharacters, seriaIndex);
        }
        else
        {
            IReadOnlyList<Character> characters = GetCharacters(seriaIndex);
            result = SortAndGetCustomizationCharacters(characters, seriaIndex);
        }
        return result;
    }

    private IReadOnlyList<CustomizableCharacter> SortAndGetCustomizationCharacters(IReadOnlyList<Character> extractedCharacters, int seriaIndex)
    {
        List<CustomizableCharacter> customizableCharacters = null;
        for (int i = 0; i < extractedCharacters.Count; i++)
        {
            if (extractedCharacters[i] is CustomizableCharacter customizableCharacter)
            {
                if (customizableCharacters == null)
                {
                    customizableCharacters = new List<CustomizableCharacter>();
                }
                customizableCharacters.Add(customizableCharacter);
            }
        }

        if (customizableCharacters != null)
        {
            _createdCustomizableCharacters.Add(seriaIndex, customizableCharacters);
        }
        return customizableCharacters;
    }

    private List<Character> CreateCharactersToSeria(int seriaIndex)
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