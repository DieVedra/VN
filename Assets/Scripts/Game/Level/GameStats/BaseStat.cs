using System;
using UnityEngine;

[Serializable]
public class BaseStat : ILocalizationString
{
    [SerializeField] private LocalizationString _name;
    [SerializeField] private int _value;
    
    public string Name => _name;
    public int Value => _value;
    public LocalizationString LocalizationName => _name;


    public BaseStat(string name, int value)
    {
        _name = name;
        _value = value;
    }
}