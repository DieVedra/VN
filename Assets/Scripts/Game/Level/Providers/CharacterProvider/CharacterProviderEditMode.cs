using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class CharacterProviderEditMode : MonoBehaviour, ICharacterProvider
{
    [SerializeField, Expandable] private List<CharacterData> _charactersDatas;
    
    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("All Characters: "), ReadOnly] private List<string> _allCharactersNames;
    private Dictionary<string, Character> _characters;
    public IReadOnlyList<Character> GetCharacters(int seriaIndex)
    {
        throw new System.NotImplementedException();
    }

    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters { get; private set; }

    public void Construct(WardrobeSaveData[] wardrobeSaveDatas = null)
    {
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
                CustomizableCharacters[i].Construct(wardrobeSaveDatas[i]);
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
}