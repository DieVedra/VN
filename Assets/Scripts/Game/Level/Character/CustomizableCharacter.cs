
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomizableCharacter", menuName = "Character/CustomizableCharacter", order = 51)]
public class CustomizableCharacter : Character
{
    [SerializeField, ReadOnly] private int _bodyIndex;
    [SerializeField, ReadOnly] private int _clothesIndex;
    [SerializeField, ReadOnly] private int _swimsuitsIndex;
    [SerializeField, ReadOnly] private int _hairstyleIndex;
    
    
    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("Bodies"), Expandable] private List<MySprite> _bodiesSprites;

    [SerializeField, BoxGroup("Emotions"), Expandable] private List<SpriteData> _emotionsData;
    // [SerializeField, BoxGroup("Emotions"), Expandable] private SpriteData _emotionsData3;
    [SerializeField, HorizontalLine(color:EColor.Pink), BoxGroup("Clothes"), Expandable] private SpriteData _clothesData;
    [SerializeField, HorizontalLine(color:EColor.Blue), BoxGroup("Swimsuits"), Expandable] private SpriteData _swimsuitsData;
    [SerializeField, HorizontalLine(color:EColor.Green), BoxGroup("Hairstyles"), Expandable] private SpriteData _hairstylesData;

    private const int CustomizationEmotionIndex = 2;
    private const int EuropeoidBodyIndex = 0;
    private const int MongoloidBodyIndex = 1;
    private const int NegroidBodyIndex = 2;
    
    private WardrobeSaveData _wardrobeSaveData;
    public SpriteData ClothesData => _clothesData;
    public SpriteData SwimsuitsData => _swimsuitsData;
    public SpriteData HairstylesData => _hairstylesData;
    public int BodyIndex => _bodyIndex;
    public int ClothesIndex => _clothesIndex;
    public int SwimsuitsIndex => _swimsuitsIndex;
    public int HairstyleIndex => _hairstyleIndex;
    public IReadOnlyList<MySprite> BodiesSprites => _bodiesSprites;

    public void Construct(WardrobeSaveData wardrobeSaveData)
    {
        _wardrobeSaveData = wardrobeSaveData;
        SetIndexes(wardrobeSaveData.BodyIndex, wardrobeSaveData.HairstyleIndex, wardrobeSaveData.ClothesIndex, wardrobeSaveData.SwimsuitsIndex);
    }

    public WardrobeSaveData GetWardrobeSaveData()
    {
        return _wardrobeSaveData;
    }

    public SpriteData GetCurrentEmotionsDataByBodyIndex()
    {
        SpriteData spriteData = null;
        for (int i = 0; i < _emotionsData.Count; ++i)
        {
            if (_bodyIndex == i)
            {
                spriteData = _emotionsData[i];
                break;
            }
        }
        return spriteData;
    }

    public override MySprite GetEmotionMySprite(int index = 0)
    {
        return _emotionsData[_bodyIndex].MySprites[index];
    }

    public override MySprite GetLookMySprite(int index = 0)
    {
        return _bodiesSprites[_bodyIndex];
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
        return _bodiesSprites[_bodyIndex];
    }

    public void SetBodyIndex(int bodyIndex)
    {
        _bodyIndex = bodyIndex;
        _wardrobeSaveData.BodyIndex = bodyIndex;
    }

    public void SetHairstyleIndex(int hairstyleIndex)
    {
        _hairstyleIndex = hairstyleIndex;
        _wardrobeSaveData.HairstyleIndex = hairstyleIndex;
    }

    public void SetClothesIndex(int clothesIndex)
    {
        _clothesIndex = clothesIndex;
        _wardrobeSaveData.ClothesIndex = clothesIndex;
    }

    public void SetSwimsuitsIndex(int swimsuitsIndex)
    {
        _swimsuitsIndex = swimsuitsIndex;
        _wardrobeSaveData.SwimsuitsIndex = swimsuitsIndex;
    }

    public void SetIndexes(int bodyIndex = 0, int hairstyleIndex = 0, int clothesIndex = 0, int swimsuitsIndex = 0)
    {
        SetBodyIndex(bodyIndex);
        SetHairstyleIndex(hairstyleIndex);
        SetClothesIndex(clothesIndex);
        SetSwimsuitsIndex(swimsuitsIndex);
    }
}