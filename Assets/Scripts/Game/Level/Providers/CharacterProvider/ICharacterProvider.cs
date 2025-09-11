using System.Collections.Generic;

public interface ICharacterProvider
{
    public IReadOnlyList<Character> GetCharacters(int seriaIndex);
}