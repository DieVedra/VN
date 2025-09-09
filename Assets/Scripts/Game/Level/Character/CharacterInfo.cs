using UnityEngine;

[System.Serializable]
public class CharacterInfo
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string NameKey { get; private set; }
    [field: SerializeField] public bool IsCustomizationCharacter  { get; private set; }  
}