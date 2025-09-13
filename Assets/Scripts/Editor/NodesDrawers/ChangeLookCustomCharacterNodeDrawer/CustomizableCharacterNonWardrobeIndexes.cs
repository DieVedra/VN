using System;
using UnityEngine;

[Serializable]
public class CustomizableCharacterNonWardrobeIndexes
{
    [SerializeField] private (string, int)[] _clothesIndexes;
    [SerializeField] private (string, int)[] _hairstyleIndexes;
    [SerializeField] private (string, int)[] _swimsuitsIndexes;

    public (string, int)[] ClothesIndexes => _clothesIndexes;
    public (string, int)[] HairstyleIndexes => _hairstyleIndexes;
    public (string, int)[] SwimsuitsIndexes => _swimsuitsIndexes;
    public CustomizableCharacterNonWardrobeIndexes((string, int)[] clothesIndexes, (string, int)[] hairstyleIndexes, (string, int)[] swimsuitsIndexes)
    {
        _clothesIndexes = clothesIndexes;
        _hairstyleIndexes = hairstyleIndexes;
        _swimsuitsIndexes = swimsuitsIndexes;
    }
}