
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomizableCharacter", menuName = "Character/CustomizableCharacter", order = 0)]
public class CustomizableCharacter : Character
{
    [SerializeField] private int _bodyIndex;
    [SerializeField] private int _clothesIndex;
    [SerializeField] private int _swimsuitsIndex;
    [SerializeField] private int _hairstyleIndex;
    [SerializeField] private List<int> _purchasedClothes;
    [SerializeField] private List<int> _purchasedSwimsuits;
    [SerializeField] private List<int> _purchasedHairstyle;
    
    [SerializeField, BoxGroup("Emotions"), Expandable] private SpriteData _emotionsData2;
    [SerializeField, BoxGroup("Emotions"), Expandable] private SpriteData _emotionsData3;
    [SerializeField, HorizontalLine(color:EColor.Pink), BoxGroup("Clothes"), Expandable] private SpriteData _clothesData;
    [SerializeField, HorizontalLine(color:EColor.Blue), BoxGroup("Swimsuits"), Expandable] private SpriteData _swimsuitsData;
    [SerializeField, HorizontalLine(color:EColor.Green), BoxGroup("Hairstyles"), Expandable] private SpriteData _hairstylesData;
    
    public SpriteData ClothesData => _clothesData;
    public SpriteData SwimsuitsData => _swimsuitsData;
    public SpriteData HairstylesData => _hairstylesData;
    public int BodyIndex => _bodyIndex;
    public int ClothesIndex => _clothesIndex;
    public int SwimsuitsIndex => _swimsuitsIndex;
    public int HairstyleIndex => _hairstyleIndex;

    public SpriteData GetCurrentEmotionsData()
    {
        SpriteData spriteData = null;
        switch (_bodyIndex)
        {
            case 0:
                spriteData = EmotionsData;
                break;
            case 1:
                spriteData =  _emotionsData2;
                break;
            case 2:
                spriteData = _emotionsData3;
                break;
        }

        return spriteData;
    }

    public MySprite GetEmotionToCustomization()
    {
        return GetCurrentEmotionsData().MySprites[2];
    }
    public MySprite GetClothesSprite()
    {
        return _clothesData.MySprites[_clothesIndex];
    }
    public MySprite GetSwimsuitSprite()
    {
        return _swimsuitsData.MySprites[_swimsuitsIndex];
    }
    public MySprite GetHairstyleSprite()
    {
        return _hairstylesData.MySprites[_hairstyleIndex];
    }
    public MySprite GetBodySprite()
    {
        return LooksData.MySprites[_bodyIndex];
    }

    public void SetBodyIndex(int bodyIndex)
    {
        _bodyIndex = bodyIndex;
    }
    
    public void SetHairstyleIndex(int hairstyleIndex)
    {
        _hairstyleIndex = hairstyleIndex;
    }
    public void SetClothesIndex(int clothesIndex)
    {
        _clothesIndex = clothesIndex;
    }
    public void SetSwimsuitsIndex(int swimsuitsIndex)
    {
        _swimsuitsIndex = swimsuitsIndex;
    }
    public void SetIndexes(int bodyIndex = 0, int hairstyleIndex = 0, int clothesIndex = 0, int swimsuitsIndex = 0)
    {
        _bodyIndex = bodyIndex;
        _hairstyleIndex = hairstyleIndex;
        _clothesIndex = clothesIndex;
        _swimsuitsIndex = swimsuitsIndex;
    }
}