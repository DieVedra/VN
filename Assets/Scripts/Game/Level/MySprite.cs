
using UnityEngine;

[System.Serializable]
public class MySprite
{
    [SerializeField] private float _offsetXValue = 0.5f;
    [SerializeField] private float _offsetYValue = 0.5f;
    [SerializeField] private float _scaleValue = 0.5f;
    [SerializeField] private int _price;
    [SerializeField] private Sprite _sprite;
    public string Name => _sprite.name;
    public float OffsetXValue => _offsetXValue;
    public float OffsetYValue => _offsetYValue;
    public float ScaleValue => _scaleValue;
    public int Price => _price;
    public Sprite Sprite => _sprite;
    public Texture2D texture => _sprite.texture;
    public MySprite(Sprite sprite, float offsetXValue = 0.5f, float offsetYValue = 0.5f, float scaleValue = 0.5f, int price = 0)
    {
        _sprite = sprite;
        _offsetXValue = offsetXValue;
        _offsetYValue = offsetYValue;
        _scaleValue = scaleValue;
        _price = price;
    }
}