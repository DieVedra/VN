using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;

public class CharacterProviderBuildMode :  ICharacterProvider
{
    private const string _charactersDataProviderName = "CharactersDataProviderSeria";
    private const string _charactersProviderName = "CharactersProviderSeria";

    private readonly DataProvider<CharactersProvider> _charactersProvider;
    private readonly DataProvider<CharactersDataProvider> _charactersDataProvider;

    private Dictionary<string, Character> _dictionaryAllCharacters;
    public IParticipiteInLoad CharactersDataProviderParticipiteInLoad => _charactersDataProvider;
    public IParticipiteInLoad CustomizableCharacterDataProviderParticipiteInLoad => null;

    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters => null;

    public CharacterProviderBuildMode()
    {
        _charactersProvider = new DataProvider<CharactersProvider>();
        _charactersDataProvider = new DataProvider<CharactersDataProvider>();
    }

    public async UniTask Construct()
    {
        await UniTask.WhenAll(_charactersProvider.CreateNames(_charactersProviderName),
            _charactersDataProvider.CreateNames(_charactersDataProviderName));
    }

    public IReadOnlyList<Character> GetCharacters(int seriaIndex = 0)
    {
        
        return null;
    }

    public void Dispose()
    {
        _charactersDataProvider.Dispose();
        _charactersProvider.Dispose();
    }

    public void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        _charactersDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        _charactersProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }
    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        await _charactersProvider.TryLoadData(nextSeriaNameAssetIndex);
        await _charactersDataProvider.TryLoadData(nextSeriaNameAssetIndex);
        
        
        // if (await _charactersProvider.TryLoadData(nextSeriaNameAssetIndex))
        // {
        //     var customizableCharacter = _customizableCharacterDataProvider.GetDatas[nextSeriaNameAssetIndex];
        // }
        // if (await _charactersDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        // {
        //     // List<SimpleCharacter> newSimpleCharacters = _charactersDataProvider.GetDatas[nextSeriaNameAssetIndex].SimpleCharacters;
        //     // if (_allCharacters.Count > 0)
        //     // {
        //     //     Dictionary<string, Character> dictionaryAllCharacters = _allCharacters.ToDictionary(
        //     //         x => x.Name.Key, x=> x);
        //     //     for (int i = 0; i < newSimpleCharacters.Count; i++)
        //     //     {
        //     //         if (dictionaryAllCharacters.TryGetValue(newSimpleCharacters[i].Name.Key, out Character character))
        //     //         {
        //     //             if (character is SimpleCharacter simpleCharacter)
        //     //             {
        //     //                 simpleCharacter.TryMerge(newSimpleCharacters[i]);
        //     //             }
        //     //         }
        //     //         else
        //     //         {
        //     //             _allCharacters.Add(newSimpleCharacters[i]);
        //     //         }
        //     //     }
        //     // }
        //     // else
        //     // {
        //     //     _allCharacters.AddRange(newSimpleCharacters);
        //     // }
        // }
    }
}