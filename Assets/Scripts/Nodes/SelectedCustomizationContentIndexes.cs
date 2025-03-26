using System.Collections.Generic;

public class SelectedCustomizationContentIndexes
{
    public readonly IReadOnlyList<ICustomizationSettings> SpriteIndexesBodies;
    public readonly IReadOnlyList<ICustomizationSettings> SpriteIndexesHairstyles;
    public readonly IReadOnlyList<ICustomizationSettings> SpriteIndexesClothes;
    public readonly IReadOnlyList<ICustomizationSettings> SpriteIndexesSwimsuits;
    public readonly CustomizableCharacter CustomizableCharacter;
    public readonly ArrowSwitchMode StartMode;
    
    private readonly List<IReadOnlyList<ICustomizationSettings>> _indexesSpriteIndexes;
    public IReadOnlyList<IReadOnlyList<ICustomizationSettings>> IndexesSpriteIndexes => _indexesSpriteIndexes;

    public SelectedCustomizationContentIndexes(
        IReadOnlyList<ICustomizationSettings> spriteIndexesBodies,
        IReadOnlyList<ICustomizationSettings> spriteIndexesHairstyles,
        IReadOnlyList<ICustomizationSettings> spriteIndexesClothes,
        IReadOnlyList<ICustomizationSettings> spriteIndexesSwimsuits,
        CustomizableCharacter customizableCharacter)
    {
        _indexesSpriteIndexes = new List<IReadOnlyList<ICustomizationSettings>>()
        {
            spriteIndexesBodies, spriteIndexesHairstyles, spriteIndexesClothes, spriteIndexesSwimsuits
        };
        
        for (int i = 0; i < _indexesSpriteIndexes.Count; i++)
        {
            if (_indexesSpriteIndexes[i].Count > 0)
            {
                switch (i)
                {
                    case (int)ArrowSwitchMode.SkinColor:
                        StartMode = ArrowSwitchMode.SkinColor;
                        break;
                    case (int)ArrowSwitchMode.Hairstyle:
                        StartMode = ArrowSwitchMode.Hairstyle;
                        break;
                    case (int)ArrowSwitchMode.Clothes:
                        StartMode = ArrowSwitchMode.Clothes;
                        break;
                    case (int)ArrowSwitchMode.Swimsuits:
                        StartMode = ArrowSwitchMode.Clothes;
                        break;
                }
                break;
            }
        }
        SpriteIndexesBodies = spriteIndexesBodies;
        SpriteIndexesHairstyles = spriteIndexesHairstyles;
        SpriteIndexesClothes = spriteIndexesClothes;
        SpriteIndexesSwimsuits = spriteIndexesSwimsuits;
        CustomizableCharacter = customizableCharacter;
    }
}