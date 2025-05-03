
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CharacterProviderBuildMode : ICharacterProvider
{
    private const string CharactersDataName = "CharactersDataSeria";
    private const string CustomizableCharacterName = "CustomizableCharacter";
    private readonly AssetExistsHandler _assetExistsHandler;
    private readonly ScriptableObjectAssetLoader _scriptableObjectAssetLoader;
    private readonly List<Character> _allCharacters;
    private List<string> _names;

    public CustomizableCharacter CustomizableCharacter { get; private set; }

    public CharacterProviderBuildMode()
    {
        _allCharacters = new List<Character>();
        _assetExistsHandler = new AssetExistsHandler();
        _scriptableObjectAssetLoader = new ScriptableObjectAssetLoader();
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return _allCharacters;
    }
    public void Dispose()
    {
        _scriptableObjectAssetLoader.UnloadAll();
    }
    public async UniTask LoadFirstSeriaCharacters()
    {
        _names = await _assetExistsHandler.CheckExistsAssetsNames(CharactersDataName);
        
        CustomizableCharacter = await _scriptableObjectAssetLoader.Load<CustomizableCharacter>(CustomizableCharacterName);
        AddContent(CustomizableCharacter);
        AddContent(await _scriptableObjectAssetLoader.Load<CharactersData>(_names[0]));
        LoadOtherCharacters().Forget();
    }
    private async UniTaskVoid LoadOtherCharacters()
    {
        for (int i = 1; i < _names.Count; i++)
        {
            AddContent(await _scriptableObjectAssetLoader.Load<CharactersData>(_names[i]));
        }
    }
    private void AddContent(Character character)
    {
        _allCharacters.Add(character);
    }
    private void AddContent(CharactersData charactersData)
    {
        for (int j = 0; j < charactersData.SimpleCharacters.Count; j++)
        {
            AddContent(charactersData.SimpleCharacters[j]);
        }
    }
}