using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterData", menuName = "Character/CharacterData", order = 51)]
public class CharacterData : BaseCharacterData
{
    [SerializeField, HorizontalLine(color:EColor.Red), Expandable] private SpriteData _emotionsData;
    [SerializeField, HorizontalLine(color:EColor.Yellow), Expandable] private SpriteData _looksData;
    public SpriteData LooksData => _looksData;
    public SpriteData EmotionsData => _emotionsData;
}
public class BaseCharacterData : ScriptableObject
{
    [SerializeField, HorizontalLine(color:EColor.Blue)] private string _characterNameKey;
    public string CharacterNameKey => _characterNameKey;
}