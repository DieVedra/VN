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
    public IReadOnlyList<Character> GetCharacters(int seriaIndex)
    {
        List<Character> characters = new List<Character>();
        
        
        
        throw new System.NotImplementedException();
    }

    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters { get; private set; }
    public void Construct(WardrobeSaveData wardrobeSaveData)
    {
        // WardrobeSaveData = wardrobeSaveData;
        // SetIndexes(wardrobeSaveData.CurrentBodyIndex, wardrobeSaveData.CurrentHairstyleIndex,
        //     wardrobeSaveData.CurrentClothesIndex, wardrobeSaveData.CurrentSwimsuitsIndex);
    }
    public void Construct(WardrobeSaveData[] wardrobeSaveDatas = null)
    {
        _customizableCharacterIndexesCustodians = new Dictionary<string, CustomizableCharacterIndexesCustodian>();
        
        
        
        
        
        
        List<CustomizableCharacter> customizableCharacters = new List<CustomizableCharacter>();
        // _allCharactersNames = new List<string>();
        // _characters = new Dictionary<string, Character>();
        //
        // foreach (var charDat in _charactersDatas)
        // {
        //     foreach (var character in charDat.Characters)
        //     {
        //         
        //         
        //         
        //         _allCharactersNames.Add(character.Name.DefaultText);
        //
        //         for (int i = 0; i < UPPER; i++)
        //         {
        //             
        //         }
        //         
        //         _characters.Add(character);
        //         if (character is CustomizableCharacter customizableCharacter)
        //         {
        //             customizableCharacters.Add(customizableCharacter);
        //         }
        //     }
        // }

        // CustomizableCharacters = customizableCharacters;
        if (wardrobeSaveDatas != null)
        {
            for (int i = 0; i < CustomizableCharacters.Count; i++)
            {
                // CustomizableCharacters[i].Construct(wardrobeSaveDatas[i]);
            }
        }
    }

    // public IReadOnlyList<CustomizableCharacter> GetCustomizableCharacters(int seriaIndex)
    // {
    //     Dictionary<string, CustomizableCharacter> customizableCharacterCharacters = new Dictionary<string, CustomizableCharacter>();
    //     foreach (var charDat in _charactersDatas)
    //     {
    //         if (charDat.SeriaIndex <= seriaIndex)
    //         {
    //             foreach (var character in charDat.Characters)
    //             {
    //                 if (customizableCharacterCharacters.TryGetValue(character.Name.Key, out CustomizableCharacter value))
    //                 {
    //                     value.TryMerge(character);
    //                 }
    //                 else
    //                 {
    //                     if (character is CustomizableCharacter customizableCharacter)
    //                     {
    //                         customizableCharacterCharacters.Add(character.Name.Key, customizableCharacter);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     return customizableCharacterCharacters.Select(x => x.Value).ToList();
    // }
    // public IReadOnlyList<Character> GetCharacters(int seriaIndex)
    // {
    //     Dictionary<string, Character> characters = new Dictionary<string, Character>();
    //     SimpleCharacter simpleCharacter = ScriptableObject.CreateInstance<SimpleCharacter>();
    //     
    //     foreach (var charDat in _charactersDatas)
    //     {
    //         if (charDat.SeriaIndex <= seriaIndex)
    //         {
    //             foreach (var character in charDat.Characters)
    //             {
    //                 if (characters.TryGetValue(character.Name.Key, out Character value))
    //                 {
    //                     value.TryMerge(character);
    //                 }
    //                 else
    //                 {
    //                     characters.Add(character.Name.Key, character);
    //                 }
    //             }
    //         }
    //     }
    //     return characters.Select(x => x.Value).ToList();
    // }
    private void CreateCharacters1()
    {
        for (int i = 0; i < _charactersProvider.Count; i++)
        {
            for (int j = 0; j < _charactersProvider[i].CharactersInfo.Count; j++)
            {
                if (_charactersProvider[i].CharactersInfo[j].IsCustomizationCharacter == true)
                {
                    if (_customizableCharacterIndexesCustodians.ContainsKey(_charactersProvider[i].CharactersInfo[j].NameKey) == false)
                    {
                        _customizableCharacterIndexesCustodians.Add(_charactersProvider[i].CharactersInfo[j].NameKey, new CustomizableCharacterIndexesCustodian());
                    }
                }
                else
                {
                    
                }
                
            }
            
        }
    }
    private void CreateCharacter(CharacterInfo characterInfo)
    {
        if (characterInfo.IsCustomizationCharacter == true)
        {
            CustomizableCharacterIndexesCustodian custodian;
            if (_customizableCharacterIndexesCustodians.TryGetValue(characterInfo.NameKey, out CustomizableCharacterIndexesCustodian value))
            {
                custodian = value;
            }
            else
            {
                custodian = new CustomizableCharacterIndexesCustodian();
                _customizableCharacterIndexesCustodians.Add(characterInfo.NameKey, custodian);
            }

            var characterData = GetCharacterContent(characterInfo.NameKey);
            
            CustomizableCharacter customizableCharacter = new CustomizableCharacter(custodian, characterData as CustomizationCharacterData);
        }
    }

    private BaseCharacterData GetCharacterContent(string nameKey)
    {
        BaseCharacterData result = null;
        foreach (var dataProvider in _charactersDataProviders)
        {
            foreach (var charactersData in dataProvider.CharactersDatas)
            {
                if (charactersData.CharacterNameKey == nameKey)
                {
                    result = charactersData;
                    break;
                }
            }

            if (result != null)
            {
                break;
            }
        }
        return result;
    }
}