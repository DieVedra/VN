using System.Collections.Generic;

public interface ICharacterProvider
{
    public IReadOnlyList<Character> GetCharacters();
    public IReadOnlyList<CustomizableCharacter> CustomizableCharacters { get; }
}