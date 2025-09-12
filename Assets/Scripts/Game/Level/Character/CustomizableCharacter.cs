using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class CustomizableCharacter : Character
{
    private readonly List<BodySpriteData> _bodiesData;
    private readonly List<MySprite> _clothesData;
    private readonly List<MySprite> _swimsuitsData;
    private readonly List<MySprite> _hairstylesData;
    
    private readonly ReactiveProperty<int> _bodyIndexRP;
    private readonly ReactiveProperty<int> _clothesIndexRP;
    private readonly ReactiveProperty<int> _swimsuitsIndexRP;
    private readonly ReactiveProperty<int> _hairstyleIndexRP;

    public CustomizableCharacter(CustomizableCharacterIndexesCustodian customizableCharacterIndexesCustodian,
        List<CustomizationCharacterData> customizationCharacterData, LocalizationString name) : base(name)
    {
        _bodyIndexRP = customizableCharacterIndexesCustodian.BodyIndexRP;
        _clothesIndexRP = customizableCharacterIndexesCustodian.ClothesIndexRP;
        _swimsuitsIndexRP = customizableCharacterIndexesCustodian.SwimsuitsIndexRP;
        _hairstyleIndexRP = customizableCharacterIndexesCustodian.HairstyleIndexRP;
        _bodiesData = new List<BodySpriteData>();
        _clothesData = new List<MySprite>();
        _swimsuitsData = new List<MySprite>();
        _hairstylesData = new List<MySprite>();

        for (int i = 0; i < customizationCharacterData.Count; i++)
        {
            AddDataSeria(customizationCharacterData[i]);
        }
    }
    public IReadOnlyList<BodySpriteData> BodiesData => _bodiesData;
    public IReadOnlyList<MySprite> ClothesData => _clothesData;
    public IReadOnlyList<MySprite> SwimsuitsData => _swimsuitsData;
    public IReadOnlyList<MySprite> HairstylesData => _hairstylesData;
    public int BodyIndex => _bodyIndexRP.Value;
    public int ClothesIndex => _clothesIndexRP.Value;
    public int SwimsuitsIndex => _swimsuitsIndexRP.Value;
    public int HairstyleIndex => _hairstyleIndexRP.Value;
    public IReadOnlyList<MySprite> GetBodiesSprites()
    {
        List<MySprite> bodiesSprites = new List<MySprite>(_bodiesData.Count);
        for (int i = 0; i < _bodiesData.Count; ++i)
        {
            bodiesSprites.Add(_bodiesData[i].Body);
        }
    
        return bodiesSprites;
    }
    private void AddDataSeria(CustomizationCharacterData customizationCharacterData)
    {
        if (customizationCharacterData.BodiesDataSeria != null && customizationCharacterData.BodiesDataSeria.Count > 0)
        {
            _bodiesData.AddRange(customizationCharacterData.BodiesDataSeria);
        }
        if (customizationCharacterData.ClothesDataSeria != null && customizationCharacterData.ClothesDataSeria.GetMySprites.Count > 0)
        {
            _clothesData.AddRange(customizationCharacterData.ClothesDataSeria.GetMySprites);
        }
        if (customizationCharacterData.HairstylesDataSeria != null && customizationCharacterData.HairstylesDataSeria.GetMySprites.Count > 0)
        {
            _hairstylesData.AddRange(customizationCharacterData.HairstylesDataSeria.GetMySprites);
        }
        if (customizationCharacterData.SwimsuitsDataSeria != null && customizationCharacterData.SwimsuitsDataSeria.GetMySprites.Count > 0)
        {
            _swimsuitsData.AddRange(customizationCharacterData.SwimsuitsDataSeria.GetMySprites);
        }
    }

    public SpriteData GetCurrentEmotionsDataByBodyIndex()
    {
        SpriteData spriteData = null;
        for (int i = 0; i < _bodiesData.Count; ++i)
        {
            if (_bodyIndexRP.Value == i)
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
            return _bodiesData[_bodyIndexRP.Value].EmotionsData.GetMySprites[--index];
        }
    }

    public override MySprite GetLookMySprite(int index = 0)
    {
        return _bodiesData[_bodyIndexRP.Value].Body;
    }

    public MySprite GetClothesSprite()
    {
        return _clothesData[_clothesIndexRP.Value];
    }

    public MySprite GetSwimsuitSprite()
    {
        return _swimsuitsData[_swimsuitsIndexRP.Value];
    }

    public MySprite GetHairstyleSprite()
    {
        return _hairstylesData[_hairstyleIndexRP.Value];
    }

    public void SetBodyIndex(int bodyIndex)
    {
        _bodyIndexRP.Value = bodyIndex;
    }

    public void SetHairstyleIndex(int hairstyleIndex)
    {
        _hairstyleIndexRP.Value = hairstyleIndex;
    }

    public void SetClothesIndex(int clothesIndex)
    {
        _clothesIndexRP.Value = clothesIndex;
    }

    public void SetSwimsuitsIndex(int swimsuitsIndex)
    {
        _swimsuitsIndexRP.Value = swimsuitsIndex;
    }

    public void SetIndexes(int bodyIndex = 0, int hairstyleIndex = 0, int clothesIndex = 0, int swimsuitsIndex = 0)
    {
        SetBodyIndex(bodyIndex);
        SetHairstyleIndex(hairstyleIndex);
        SetClothesIndex(clothesIndex);
        SetSwimsuitsIndex(swimsuitsIndex);
    }
}