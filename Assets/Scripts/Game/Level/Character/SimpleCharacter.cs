using System.Collections.Generic;

public class SimpleCharacter : Character
{
    private readonly int _mySeriaIndex;
    private readonly List<MySprite> _emotions;
    private readonly List<MySprite> _looks;

    public SimpleCharacter(List<CharacterData> characterData, LocalizationString name, int mySeriaIndex) : base(name)
    {
        _emotions = new List<MySprite>();
        _looks = new List<MySprite>();
        for (int i = 0; i < characterData.Count; i++)
        {
            if (characterData[i].EmotionsData != null)
            {
                _emotions.AddRange(characterData[i].EmotionsData.GetMySprites);
            }
            if (characterData[i].LooksData != null)
            {
                _looks.AddRange(characterData[i].LooksData.GetMySprites);
            }
        }
        _mySeriaIndex = mySeriaIndex;
    }
    public IReadOnlyList<MySprite> Emotions => _emotions;
    public IReadOnlyList<MySprite> Looks => _looks;
    public int MySeriaIndex => _mySeriaIndex;
    public override MySprite GetLookMySprite(int index)
    {
        return _looks[index];
    }

    public override MySprite GetEmotionMySprite(int index)
    {
        if (index > _emotions.Count -1 || index < 0)
        {
            return _emotions[0];
        }
        else
        {
            return _emotions[index];
        }
    }
}