using System;
using System.Collections.Generic;

[Serializable]
public class CustomizableCharacter : Character
{
    public readonly CustomizableCharacterIndexesCustodian CustomizableCharacterIndexesCustodian;
    private readonly List<BodySpriteData> _bodiesData;
    private readonly List<MySprite> _clothesData;
    private readonly List<MySprite> _swimsuitsData;
    private readonly List<MySprite> _hairstylesData;
    public CustomizableCharacter(CustomizableCharacterIndexesCustodian customizableCharacterIndexesCustodian,
        List<CustomizationCharacterData> customizationCharacterData, LocalizationString name) : base(name)
    {
        CustomizableCharacterIndexesCustodian = customizableCharacterIndexesCustodian;
        
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
    public int BodyIndex => CustomizableCharacterIndexesCustodian.BodyIndexRP.Value;
    public int ClothesIndex => CustomizableCharacterIndexesCustodian.ClothesIndexRP.Value;
    public int SwimsuitsIndex => CustomizableCharacterIndexesCustodian.SwimsuitsIndexRP.Value;
    public int HairstyleIndex => CustomizableCharacterIndexesCustodian.HairstyleIndexRP.Value;
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
            if (CustomizableCharacterIndexesCustodian.BodyIndexRP.Value == i)
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
            return _bodiesData[CustomizableCharacterIndexesCustodian.BodyIndexRP.Value].EmotionsData.GetMySprites[--index];
        }
    }

    public override MySprite GetLookMySprite(int index = 0)
    {
        return _bodiesData[CustomizableCharacterIndexesCustodian.BodyIndexRP.Value].Body;
    }

    public MySprite GetClothesSprite()
    {
        return _clothesData[CustomizableCharacterIndexesCustodian.ClothesIndexRP.Value];
    }

    public MySprite GetSwimsuitSprite()
    {
        return _swimsuitsData[CustomizableCharacterIndexesCustodian.SwimsuitsIndexRP.Value];
    }

    public MySprite GetHairstyleSprite()
    {
        return _hairstylesData[CustomizableCharacterIndexesCustodian.HairstyleIndexRP.Value];
    }
    public void SetBodyIndex(int bodyIndex)
    {
        CustomizableCharacterIndexesCustodian.BodyIndexRP.Value = bodyIndex;
    }

    public void SetHairstyleIndex(int hairstyleIndex)
    {
        CustomizableCharacterIndexesCustodian.HairstyleIndexRP.Value = hairstyleIndex;
    }

    public void SetClothesIndex(int clothesIndex)
    {
        CustomizableCharacterIndexesCustodian.ClothesIndexRP.Value = clothesIndex;
    }

    public void SetSwimsuitsIndex(int swimsuitsIndex)
    {
        CustomizableCharacterIndexesCustodian.SwimsuitsIndexRP.Value = swimsuitsIndex;
    }

    public void SetIndexes(int bodyIndex = 0, int hairstyleIndex = 0, int clothesIndex = 0, int swimsuitsIndex = 0)
    {
        SetBodyIndex(bodyIndex);
        SetHairstyleIndex(hairstyleIndex);
        SetClothesIndex(clothesIndex);
        SetSwimsuitsIndex(swimsuitsIndex);
    }
}