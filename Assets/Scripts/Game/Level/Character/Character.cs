using UnityEngine;

public abstract class Character : ScriptableObject
{
    [SerializeField] private string _name;
    [SerializeField] private LocalizationString _name1;
    public LocalizationString _name2;
    public string MyName => _name;

    public abstract MySprite GetLookMySprite(int index);
    public abstract MySprite GetEmotionMySprite(int index);
}