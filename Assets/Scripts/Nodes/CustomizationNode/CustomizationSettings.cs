﻿using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomizationSettings : ICustomizationSettings
{
    [SerializeField] private LocalizationString _name;
    [SerializeField] private int _index;
    [SerializeField] private bool _keyAdd;
    [SerializeField] private bool _keyShowParams;
    [SerializeField] private bool _keyShowStats;
    [SerializeField] private int _price;
    [SerializeField] private int _priceAdditional;

    [SerializeField] private List<Stat> _gameStats;
    public string Name => _name;
    public LocalizationString LocalizationName => _name;
    public int Index => _index;
    public bool KeyAdd => _keyAdd;
    public bool KeyShowParams => _keyShowParams;
    public bool KeyShowStats => _keyShowStats;
    public int Price => _price;
    public int PriceAdditional => _priceAdditional;
    public List<Stat> GameStats => _gameStats;
    public IReadOnlyList<ILocalizationString> GameStatsLocalizationStrings => _gameStats;
    public CustomizationSettings(List<Stat> gameStats, string name, int index, int price, int priceAdditional, 
        bool keyAdd, bool keyShowParams, bool keyShowStats)
    {
        _gameStats = gameStats;
        _name = name;
        _index = index;
        _price = price;
        _priceAdditional = priceAdditional;
        _keyAdd = keyAdd;
        _keyShowParams = keyShowParams;
        _keyShowStats = keyShowStats;
    }
    public CustomizationSettings(List<Stat> gameStats, string name, int index, int price, int priceAdditional)
    {
        _gameStats = gameStats;
        _name = name;
        _index = index;
        _price = price;
        _priceAdditional = priceAdditional;
        _keyAdd = false;
        _keyShowParams  = false;
        _keyShowStats  = true;
    }
    public CustomizationSettings(List<Stat> gameStats, string name, int index, int price, int priceAdditional, bool keyShowStats)
    {
        _gameStats = gameStats;
        _name = name;
        _index = index;
        _price = price;
        _priceAdditional = priceAdditional;
        _keyAdd = false;
        _keyShowParams  = false;
        _keyShowStats = keyShowStats;
    }
}