
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CharacterHandlerBuildMode : ICharacterHandler
{
    private const string CharactersDataName = "CharactersDataSeria";
    private readonly List<Character> _allCharacters;

    public CustomizableCharacter CustomizableCharacter { get; private set; }

    public CharacterHandlerBuildMode()
    {
        _allCharacters = new List<Character>();
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return _allCharacters;
    }
    public UniTask DownloadFirstsCharacters()
    {
        
        
        return default;
    }
}