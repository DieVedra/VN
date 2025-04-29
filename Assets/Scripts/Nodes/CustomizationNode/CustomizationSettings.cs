using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomizationSettings : ICustomizationSettings
{
    [SerializeField/*, HideInInspector*/] private string _name;
    [SerializeField/*, HideInInspector*/] private int _index;
    [SerializeField/*, HideInInspector*/] private bool _keyAdd;
    [SerializeField/*, HideInInspector*/] private bool _keyShowParams;
    [SerializeField/*, HideInInspector*/] private bool _keyShowStats;
    [SerializeField/*, HideInInspector*/] private int _price;

    [SerializeField/*, HideInInspector*/] private List<Stat> _gameStats;
    public string Name => _name;
    public int Index => _index;
    public bool KeyAdd => _keyAdd;
    public bool KeyShowParams => _keyShowParams;
    public bool KeyShowStats => _keyShowStats;
    public int Price => _price;
    public List<Stat> GameStats => _gameStats;
    public CustomizationSettings(List<Stat> gameStats, string name, int index, int price,
        bool keyAdd, bool keyShowParams, bool keyShowStats)
    {
        _gameStats = gameStats;
        _name = name;
        _index = index;
        _price = price;
        _keyAdd = keyAdd;
        _keyShowParams = keyShowParams;
        _keyShowStats = keyShowStats;
    }
    public CustomizationSettings(List<Stat> gameStats, string name, int index, int price)
    {
        _gameStats = gameStats;
        _name = name;
        _index = index;
        _price = price;
        _keyAdd = false;
        _keyShowParams  = false;
        _keyShowStats  = true;
    }
    public CustomizationSettings(List<Stat> gameStats, string name, int index, int price, bool keyShowStats)
    {
        _gameStats = gameStats;
        _name = name;
        _index = index;
        _price = price;
        _keyAdd = false;
        _keyShowParams  = false;
        _keyShowStats = keyShowStats;
    }
}