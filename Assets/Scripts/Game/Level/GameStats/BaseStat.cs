using System;
using UnityEngine;

[Serializable]
public class BaseStat : ILocalizationString
{
    [SerializeField] private LocalizationString _name;
    [SerializeField] private int _value;
    [SerializeField] private bool _notificationKey;
    
    public string NameText => _name.DefaultText;
    public string NameKey => _name.Key;
    public int Value => _value;
    public bool NotificationKey => _notificationKey;
    public LocalizationString LocalizationName => _name;


    public BaseStat(string name, int value)
    {
        _name = name;
        _value = value;
    }
    public BaseStat(string name, string key, int value)
    {
        _name = new LocalizationString(name, key);
        _value = value;
    }
}