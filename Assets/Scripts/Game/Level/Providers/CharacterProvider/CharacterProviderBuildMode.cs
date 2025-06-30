
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CharacterProviderBuildMode :  ICharacterProvider
{
    private const string _charactersDataName = "CharactersDataSeria";
    private const string _customizableCharacterName = "CustomizableCharacter";

    private readonly DataProvider<CharactersData> _charactersDataProvider;
    private readonly DataProvider<CustomizableCharacter> _customizableCharacterDataProvider;
    private readonly List<Character> _allCharacters;
    public CustomizableCharacter CustomizableCharacter { get; private set; }
    public IParticipiteInLoad CharactersDataProviderParticipiteInLoad => _charactersDataProvider;
    public IParticipiteInLoad CustomizableCharacterDataProviderParticipiteInLoad => _customizableCharacterDataProvider;

    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters => _customizableCharacterDataProvider.Datas;

    public CharacterProviderBuildMode()
    {
        _allCharacters = new List<Character>();
        
        _charactersDataProvider = new DataProvider<CharactersData>();
        _customizableCharacterDataProvider = new DataProvider<CustomizableCharacter>();
    }

    public async UniTask Construct()
    {
        await UniTask.WhenAll(_charactersDataProvider.CreateNames(_charactersDataName),
            _customizableCharacterDataProvider.CreateNames(_customizableCharacterName));
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return _allCharacters;
    }

    public void Dispose()
    {
        _charactersDataProvider.Dispose();
        _customizableCharacterDataProvider.Dispose();
    }

    public void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        _charactersDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        _customizableCharacterDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }
    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        if (await _customizableCharacterDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        {
            CustomizableCharacter = _customizableCharacterDataProvider.Datas[nextSeriaNameAssetIndex];
            _allCharacters.Add(CustomizableCharacter);
        }
        if (await _charactersDataProvider.TryLoadData(nextSeriaNameAssetIndex))
        {
            _allCharacters.AddRange(_charactersDataProvider.Datas[nextSeriaNameAssetIndex].SimpleCharacters);
        }
    }
}