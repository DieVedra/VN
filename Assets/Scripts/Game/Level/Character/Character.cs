using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Character/Character", order = 0)]
public class Character : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField, HorizontalLine(color:EColor.Red), BoxGroup("Emotions"), Expandable] private SpriteData _emotionsData;
    [SerializeField, HorizontalLine(color:EColor.Yellow), BoxGroup("Look"), Expandable] private SpriteData _looksData;
    
    public SpriteData EmotionsData => _emotionsData;
    public SpriteData LooksData => _looksData;
    public string MyName => _name;
}