using UnityEngine;

[System.Serializable]
public class CharacterInfo
{
    [field: SerializeField] public string Name { get; private set; }
    [field: SerializeField] public string NameKey { get; private set; }
    [field: SerializeField] public bool IsCustomizationCharacter  { get; private set; }

    private LocalizationString _localizationString;

    public LocalizationString LocalizationString
    {
        get
        {
            if (_localizationString == null)
            {
                _localizationString = new LocalizationString(Name, NameKey);
            }
            else
            {
                _localizationString.SetText(Name);
                _localizationString.SetKey(NameKey);
            }
            return _localizationString;
        }
    }
}