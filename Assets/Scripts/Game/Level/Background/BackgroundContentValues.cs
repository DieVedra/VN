
using UnityEngine;

[System.Serializable]
public class BackgroundContentValues
{
    [SerializeField] private string _nameSprite;
    [SerializeField] private float _movementDuringDialogueValue = 0.25f;
    [SerializeField] private Color _colorLighting = Color.white;

    public BackgroundContentValues(string nameSprite, float movementDuringDialogueValue, Color colorLighting)
    {
        _nameSprite = nameSprite;
        _movementDuringDialogueValue = movementDuringDialogueValue;
        _colorLighting = colorLighting;
    }
}