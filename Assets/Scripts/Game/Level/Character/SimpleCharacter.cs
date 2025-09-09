using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SimpleCharacter", menuName = "Character/SimpleCharacter", order = 51)]
public class SimpleCharacter : Character
{
    [SerializeField] private int _mySeriaIndex;
    [SerializeField, ReadOnly] private SpriteData _emotionsData;
    [SerializeField, ReadOnly] private SpriteData _looksData;
    
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

    public override void TryMerge(Character character)
    {
        if (character is SimpleCharacter simpleCharacter)
        {
            if (simpleCharacter.EmotionsData.Sprites.Count > 0)
            {
                _emotionsData.Add(simpleCharacter.EmotionsData.Sprites, simpleCharacter.EmotionsData.MySprites);
            }
            if (simpleCharacter.LooksData.Sprites.Count > 0)
            {
                _looksData.Add(simpleCharacter.LooksData.Sprites, simpleCharacter.LooksData.MySprites);
            }
        }
    }
}