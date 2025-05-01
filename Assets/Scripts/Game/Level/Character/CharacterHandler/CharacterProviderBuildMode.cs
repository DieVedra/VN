
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class CharacterProviderBuildMode : ICharacterProvider
{
    private const string CharactersDataName = "CharactersDataSeria";
    private readonly List<Character> _allCharacters;

    public CustomizableCharacter CustomizableCharacter { get; private set; }

    public CharacterProviderBuildMode()
    {
        _allCharacters = new List<Character>();
    }

    public IReadOnlyList<Character> GetCharacters()
    {
        return _allCharacters;
    }
    public UniTask LoadFirstSeriaCharacters()
    {
        
        
        return default;
    }
}