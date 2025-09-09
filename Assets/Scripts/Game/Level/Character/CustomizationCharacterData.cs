
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CustomizationCharacterData", menuName = "Character/CustomizationCharacterData", order = 51)]
public class CustomizationCharacterData : BaseCharacterData
{
    [SerializeField] private int _mySeriaIndex;
    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("Bodies"), Expandable] private List<BodySpriteData> _bodiesDataSeria;
    [SerializeField, HorizontalLine(color:EColor.Pink), BoxGroup("Clothes"), Expandable] private SpriteData _clothesDataSeria;
    [SerializeField, HorizontalLine(color:EColor.Blue), BoxGroup("Swimsuits"), Expandable] private SpriteData _swimsuitsDataSeria;
    [SerializeField, HorizontalLine(color:EColor.Green), BoxGroup("Hairstyles"), Expandable] private SpriteData _hairstylesDataSeria;
}