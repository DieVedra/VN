using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "WardrobeDataSeria", menuName = "WardrobeSeriaData", order = 51)]
public class WardrobeSeriaData : ScriptableObject
{
    [SerializeField] private int _mySeriaIndex;
    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("Bodies"), Expandable] private List<BodySpriteData> _bodiesDataSeria;
    [SerializeField, HorizontalLine(color:EColor.Pink), BoxGroup("Clothes"), Expandable] private SpriteData _clothesDataSeria;
    [SerializeField, HorizontalLine(color:EColor.Blue), BoxGroup("Swimsuits"), Expandable] private SpriteData _swimsuitsDataSeria;
    [SerializeField, HorizontalLine(color:EColor.Green), BoxGroup("Hairstyles"), Expandable] private SpriteData _hairstylesDataSeria;

    public IReadOnlyList<BodySpriteData> BodiesDataSeria => _bodiesDataSeria;
    public SpriteData ClothesDataSeria => _clothesDataSeria;
    public SpriteData SwimsuitsDataSeria => _swimsuitsDataSeria;
    public SpriteData HairstylesDataSeria => _hairstylesDataSeria;
    public int MySeriaIndex => _mySeriaIndex;
    
    public IReadOnlyList<MySprite> GetBodiesSprites()
    {
        List<MySprite> bodiesSprites = new List<MySprite>(_bodiesDataSeria.Count);
        for (int i = 0; i < _bodiesDataSeria.Count; ++i)
        {
            bodiesSprites.Add(_bodiesDataSeria[i].Body);
        }

        return bodiesSprites;
    }
}