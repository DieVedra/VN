using UnityEngine;

public class CustomizationData
{
    public readonly MySprite LookSprite;
    public readonly MySprite EmotionSprite;
    public readonly MySprite HairstyleSprite;
    public readonly MySprite ClothesSprite;
    
    
    public readonly DirectionType DirectionType;
    public readonly string Name;

    public CustomizationData(MySprite lookSprite, MySprite emotionSprite, MySprite hairstyleSprite, MySprite clothesSprite,
        DirectionType directionType, string name)
    {
        LookSprite = lookSprite;
        EmotionSprite = emotionSprite;
        HairstyleSprite = hairstyleSprite;
        ClothesSprite = clothesSprite;
        DirectionType = directionType;
        Name = name;
    }
}