
using UnityEngine;

[System.Serializable]
public class BackgroundContentValues
{
    public const string NameSpriteFieldName = "_nameSprite";
    public const string NameBackgroundFieldName = "_nameBackground";
    public const string ScaleFieldName = "_scale";
    public const string MovementDuringDialogueValueFieldName = "_movementDuringDialogueValue";
    public const string ColorLightingFieldName = "_colorLighting";
    public const string ColorFieldName = "_color";
    [SerializeField] private string _nameSprite;
    [SerializeField] private string _nameBackground;
    [SerializeField] private float _movementDuringDialogueValue = 0.25f;
    [SerializeField] private Vector2 _scale = Vector2.one;
    [SerializeField] private Color _colorLighting = Color.white;
    [SerializeField] private Color _color = Color.white;
    public string NameSprite => _nameSprite;
    public string NameBackground => _nameBackground;
    public Vector2 Scale => _scale;
    public float MovementDuringDialogueValue => _movementDuringDialogueValue;
    public Color ColorLighting => _colorLighting;
    public Color Color => _color;

    public BackgroundContentValues(string nameSprite, string nameBackground, Vector2 scale, Color colorLighting, Color color,   float movementDuringDialogueValue = 0.25f)
    {
        _nameSprite = nameSprite;
        _nameBackground = nameBackground;
        _scale = scale;
        _movementDuringDialogueValue = movementDuringDialogueValue;
        _colorLighting = colorLighting;
        _color = color;
    }
}