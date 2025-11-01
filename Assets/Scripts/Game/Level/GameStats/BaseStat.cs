using System;
using UnityEngine;

[Serializable]
public class BaseStat : ILocalizationString
{
    [SerializeField] private Color _colorField = Color.white;

    [SerializeField] private LocalizationString _name;
    [SerializeField] protected int _value;
    [SerializeField] protected bool _notificationKey;
    
    public string NameText => _name.DefaultText;
    public string NameKey => _name.Key;
    public int Value => _value;
    public bool NotificationKey => _notificationKey;
    public LocalizationString LocalizationName => _name;
    public Color ColorField => _colorField;

    public BaseStat(string name, int value)
    {
        _name = name;
        _value = value;
    }
    public BaseStat(string name, int value, Color colorField)
    {
        _name = name;
        _value = value;
        _colorField = colorField;
    }
    public BaseStat(string name, string key, int value, Color colorField)
    {
        _name = new LocalizationString(name, key);
        _value = value;
        _colorField = colorField;
    }
    public BaseStat(string name, string key, int value, bool notificationKey, Color colorField)
    {
        _name = new LocalizationString(name, key);
        _value = value;
        _notificationKey = notificationKey;
        _colorField = colorField;
    }
}