using NaughtyAttributes;
using UnityEngine;

public class BaseCharacterData : ScriptableObject
{
    [SerializeField] private int _mySeriaIndex;
    [SerializeField, HorizontalLine(color:EColor.Blue)] private string _characterNameKey;
    public string CharacterNameKey => _characterNameKey;
    public int MySeriaIndex => _mySeriaIndex;
}