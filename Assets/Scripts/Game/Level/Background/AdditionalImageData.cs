using UnityEngine;

public struct AdditionalImageData
{
    public string Key;
    public Vector2 Position;
    public Color Color;

    public AdditionalImageData(string key, Vector2 position, Color color)
    {
        Key = key;
        Position = position;
        Color = color;
    }
}
