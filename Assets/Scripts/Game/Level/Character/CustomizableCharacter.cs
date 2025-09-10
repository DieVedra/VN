using System.Collections.Generic;
using UniRx;

public class CustomizableCharacter : Character
{
    private List<BodySpriteData> _bodiesData;
    private List<MySprite> _clothesData;
    private List<MySprite> _swimsuitsData;
    private List<MySprite> _hairstylesData;
    
    private readonly ReactiveProperty<int> _bodyIndexRP;
    private readonly ReactiveProperty<int> _clothesIndexRP;
    private readonly ReactiveProperty<int> _swimsuitsIndexRP;
    private readonly ReactiveProperty<int> _hairstyleIndexRP;

    public CustomizableCharacter(CustomizableCharacterIndexesCustodian customizableCharacterIndexesCustodian,
        CustomizationCharacterData customizationCharacterData)
    {
        _bodyIndexRP = customizableCharacterIndexesCustodian.BodyIndexRP;
        _clothesIndexRP = customizableCharacterIndexesCustodian.ClothesIndexRP;
        _swimsuitsIndexRP = customizableCharacterIndexesCustodian.SwimsuitsIndexRP;
        _hairstyleIndexRP = customizableCharacterIndexesCustodian.HairstyleIndexRP;
        _bodiesData = new List<BodySpriteData>();
        _clothesData = new List<MySprite>();
        _swimsuitsData = new List<MySprite>();
        _hairstylesData = new List<MySprite>();
        AddDataSeria(customizationCharacterData.BodiesDataSeria, customizationCharacterData.ClothesDataSeria.MySprites,
            customizationCharacterData.HairstylesDataSeria.MySprites, customizationCharacterData.SwimsuitsDataSeria.MySprites);
    }

    // public WardrobeSaveData WardrobeSaveData { get; private set; }
    public IReadOnlyList<BodySpriteData> BodiesData => _bodiesData;
    public IReadOnlyList<MySprite> ClothesData => _clothesData;
    public IReadOnlyList<MySprite> SwimsuitsData => _swimsuitsData;
    public IReadOnlyList<MySprite> HairstylesData => _hairstylesData;
    public int BodyIndex => _bodyIndexRP.Value;
    public int ClothesIndex => _clothesIndexRP.Value;
    public int SwimsuitsIndex => _swimsuitsIndexRP.Value;
    public int HairstyleIndex => _hairstyleIndexRP.Value;
    
    // public void AddWardrobeDataSeria(WardrobeSeriaData wardrobeSeriaData)
    // {
    //     AddDataSeria(wardrobeSeriaData.BodiesDataSeria, wardrobeSeriaData.ClothesDataSeria.MySprites,
    //         wardrobeSeriaData.HairstylesDataSeria.MySprites, wardrobeSeriaData.SwimsuitsDataSeria.MySprites);
    // }

    // public override void TryMerge(Character character)
    // {
    //     if (character is CustomizableCharacter customizableCharacter)
    //     {
    //         AddDataSeria(customizableCharacter.BodiesData, customizableCharacter.ClothesData,
    //             customizableCharacter.SwimsuitsData, customizableCharacter.HairstylesData);
    //     }
    // }

    private void AddDataSeria(IReadOnlyList<BodySpriteData> bodiesDataSeria, IReadOnlyList<MySprite> clothesDataSeria,
        IReadOnlyList<MySprite> swimsuitsDataSeria, IReadOnlyList<MySprite> hairstylesDataSeria)
    {
        if (bodiesDataSeria != null && bodiesDataSeria.Count > 0)
        {
            _bodiesData.AddRange(bodiesDataSeria);
        }
        if (clothesDataSeria != null && clothesDataSeria.Count > 0)
        {
            _clothesData.AddRange(clothesDataSeria);
        }
        if (hairstylesDataSeria != null && hairstylesDataSeria.Count > 0)
        {
            _hairstylesData.AddRange(hairstylesDataSeria);
        }
        if (swimsuitsDataSeria != null && swimsuitsDataSeria.Count > 0)
        {
            _swimsuitsData.AddRange(swimsuitsDataSeria);
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
            return _bodiesData[_bodyIndexRP.Value].EmotionsData.MySprites[--index];
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