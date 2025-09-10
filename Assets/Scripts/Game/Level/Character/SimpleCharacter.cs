
public class SimpleCharacter : Character
{
    private readonly int _mySeriaIndex;
    private readonly SpriteData _emotionsData;
    private readonly SpriteData _looksData;

    public SimpleCharacter(SpriteData emotionsData, SpriteData looksData, int mySeriaIndex)
    {
        _emotionsData = emotionsData;
        _looksData = looksData;
        _mySeriaIndex = mySeriaIndex;
    }

    public SpriteData EmotionsData => _emotionsData;
    public SpriteData LooksData => _looksData;
    public int MySeriaIndex => _mySeriaIndex;
    public override MySprite GetLookMySprite(int index)
    {
        return _looksData.MySprites[index];
    }

    public override MySprite GetEmotionMySprite(int index)
    {
        if (index > _emotionsData.MySprites.Count -1 || index < 0)
        {
            return _emotionsData.MySprites[0];
        }
        else
        {
            return _emotionsData.MySprites[index];
        }
    }
}