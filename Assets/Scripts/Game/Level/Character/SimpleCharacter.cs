using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleCharacter", menuName = "Character/SimpleCharacter", order = 51)]
public class SimpleCharacter : Character
{
    [SerializeField, HorizontalLine(color:EColor.Red), BoxGroup("Emotions"), Expandable] private SpriteData _emotionsData;
    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("Look"), Expandable] private SpriteData _looksData;
    
    public SpriteData EmotionsData => _emotionsData;
    public SpriteData LooksData => _looksData;
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