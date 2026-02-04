using UnityEngine;

public struct AdditionalImageData
{
    public string Key;
    public float PosX, PosY;
    public Color Color;

    public AdditionalImageData(string key, float posX, float posY, Color color)
    {
        Key = key;
        PosX = posX;
        PosY = posY;
        Color = color;
    }
}
