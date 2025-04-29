using UnityEngine;

public abstract class Character : ScriptableObject
{
    [SerializeField] private string _name;
    public string MyName => _name;

    public abstract MySprite GetLookMySprite(int index);
    public abstract MySprite GetEmotionMySprite(int index);
}