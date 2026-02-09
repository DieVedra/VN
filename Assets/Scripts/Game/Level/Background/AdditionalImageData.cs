using UnityEngine;

public struct AdditionalImageData
{
    public string Key;
    public float PosX, PosY;
    public float ColorR, ColorG, ColorB, ColorA;

    public AdditionalImageData(string key, float posX, float posY, Color color)
    {
        Key = key;
        PosX = posX;
        PosY = posY;
        ColorR = color.r;
        ColorG = color.g;
        ColorB = color.b;
        ColorA = color.a;
    }
}
