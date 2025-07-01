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
    
    [SerializeField, HorizontalLine(color:EColor.Yellow)] private List<BodySpriteData> _bodiesData;
    [SerializeField, HorizontalLine(color:EColor.Pink)] private List<MySprite> _clothesData;
    [SerializeField, HorizontalLine(color:EColor.Blue)] private List<MySprite> _swimsuitsData;
    [SerializeField, HorizontalLine(color:EColor.Green)] private List<MySprite> _hairstylesData;
    
    private WardrobeSaveData _wardrobeSaveData;
    public IReadOnlyList<MySprite> ClothesData => _clothesData;
    public IReadOnlyList<MySprite> SwimsuitsData => _swimsuitsData;
    public IReadOnlyList<MySprite> HairstylesData => _hairstylesData;
    public int BodyIndex => _bodyIndex;
    public int ClothesIndex => _clothesIndex;
    public int SwimsuitsIndex => _swimsuitsIndex;
    public int HairstyleIndex => _hairstyleIndex;

    public void Construct(WardrobeSaveData wardrobeSaveData)
    {
        _wardrobeSaveData = wardrobeSaveData;
        SetIndexes(wardrobeSaveData.BodyIndex, wardrobeSaveData.HairstyleIndex, wardrobeSaveData.ClothesIndex, wardrobeSaveData.SwimsuitsIndex);
    }

    [Button()]
    private void Reset()
    {
        _bodiesData = new List<BodySpriteData>();
        _clothesData = new List<MySprite>();
        _swimsuitsData = new List<MySprite>();
        _hairstylesData = new List<MySprite>();
    }

    public void AddWardrobeDataSeria(WardrobeSeriaData wardrobeSeriaData)
    {
        if (Application.isPlaying == false)
        {
            Reset();
        }
        if (wardrobeSeriaData.BodiesDataSeria.Count > 0)
        {
            _bodiesData.AddRange(wardrobeSeriaData.BodiesDataSeria);
        }

        if (wardrobeSeriaData.ClothesDataSeria.MySprites.Count > 0)
        {
            _clothesData.AddRange(wardrobeSeriaData.ClothesDataSeria.MySprites);
        }

        if (wardrobeSeriaData.HairstylesDataSeria.MySprites.Count > 0)
        {
            _hairstylesData.AddRange(wardrobeSeriaData.HairstylesDataSeria.MySprites);
        }

        if (wardrobeSeriaData.SwimsuitsDataSeria.MySprites.Count > 0)
        {
            _swimsuitsData.AddRange(wardrobeSeriaData.SwimsuitsDataSeria.MySprites);
        }
    }
    public WardrobeSaveData GetWardrobeSaveData()
    {
        return _wardrobeSaveData;
    }

    public SpriteData GetCurrentEmotionsDataByBodyIndex()
    {
        SpriteData spriteData = null;
        for (int i = 0; i < _bodiesData.Count; ++i)
        {
            if (_bodyIndex == i)
            {
                spriteData = _bodiesData[i].EmotionsData;
                break;
            }
        }
        return spriteData;
    }

    public override MySprite GetEmotionMySprite(int index = 0)
    {
        if (index == 0)
        {
            return null;
        }
        else
        {
            return _bodiesData[_bodyIndex].EmotionsData.MySprites[--index];
        }
    }

    public override MySprite GetLookMySprite(int index = 0)
    {
        return _bodiesData[_bodyIndex].Body;
    }

    public MySprite GetClothesSprite()
    {
        return _clothesData[_clothesIndex];
    }

    public MySprite GetSwimsuitSprite()
    {
        return _swimsuitsData[_swimsuitsIndex];
    }

    public MySprite GetHairstyleSprite()
    {
        return _hairstylesData[_hairstyleIndex];
    }

    public void SetBodyIndex(int bodyIndex)
    {
        _bodyIndex = bodyIndex;
        if (_wardrobeSaveData != null)
        {
            _wardrobeSaveData.BodyIndex = bodyIndex;
        }
    }

    public void SetHairstyleIndex(int hairstyleIndex)
    {
        _hairstyleIndex = hairstyleIndex;
        if (_wardrobeSaveData != null)
        {
            _wardrobeSaveData.HairstyleIndex = hairstyleIndex;
        }
    }

    public void SetClothesIndex(int clothesIndex)
    {
        _clothesIndex = clothesIndex;
        if (_wardrobeSaveData != null)
        {
            _wardrobeSaveData.ClothesIndex = clothesIndex;
        }
    }

    public void SetSwimsuitsIndex(int swimsuitsIndex)
    {
        _swimsuitsIndex = swimsuitsIndex;
        if (_wardrobeSaveData != null)
        {
            _wardrobeSaveData.SwimsuitsIndex = swimsuitsIndex;
        }
    }

    public void SetIndexes(int bodyIndex = 0, int hairstyleIndex = 0, int clothesIndex = 0, int swimsuitsIndex = 0)
    {
        SetBodyIndex(bodyIndex);
        SetHairstyleIndex(hairstyleIndex);
        SetClothesIndex(clothesIndex);
        SetSwimsuitsIndex(swimsuitsIndex);
    }
}