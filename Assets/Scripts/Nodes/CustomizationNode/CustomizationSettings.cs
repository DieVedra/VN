using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomizationSettings : ICustomizationSettings
{
    [SerializeField] private LocalizationString _localizationNameToGame;
    [SerializeField] private string _spriteName;
    [SerializeField] private int _index;
    [SerializeField] private bool _keyAdd;
    [SerializeField] private bool _keyShowParams;
    [SerializeField] private bool _keyShowStats;
    [SerializeField] private int _price;
    [SerializeField] private int _priceAdditional;

    [SerializeField] private List<CustomizationStat> _gameStats;
    public string Name => _localizationNameToGame;
    public string SpriteName => _spriteName;
    public LocalizationString LocalizationNameToGame => _localizationNameToGame;
    public int Index => _index;
    public bool KeyAdd => _keyAdd;
    public bool KeyShowParams => _keyShowParams;
    public bool KeyShowStats => _keyShowStats;
    public int Price => _price;
    public int PriceAdditional => _priceAdditional;
    public List<CustomizationStat> GameStats => _gameStats;
    public IReadOnlyList<ILocalizationString> GameStatsLocalizationStrings => _gameStats;
    public CustomizationSettings(List<CustomizationStat> gameStats, string spriteName, string localizationNameToGame, int index, int price, int priceAdditional, 
        bool keyAdd, bool keyShowParams, bool keyShowStats)
    {
        _gameStats = gameStats;
        _spriteName = spriteName;
        _localizationNameToGame = localizationNameToGame;
        _index = index;
        _price = price;
        _priceAdditional = priceAdditional;
        _keyAdd = keyAdd;
        _keyShowParams = keyShowParams;
        _keyShowStats = keyShowStats;
    }
    public CustomizationSettings(List<CustomizationStat> gameStats, string spriteName, string localizationNameToGame, int index, int price, int priceAdditional)
    {
        _gameStats = gameStats;
        _spriteName = spriteName;
        _localizationNameToGame = localizationNameToGame;
        _index = index;
        _price = price;
        _priceAdditional = priceAdditional;
        _keyAdd = false;
        _keyShowParams  = false;
        _keyShowStats  = true;
    }
    public CustomizationSettings(List<CustomizationStat> gameStats, string spriteName, string localizationNameToGame, int index, int price, int priceAdditional, bool keyShowStats)
    {
        _gameStats = gameStats;
        _spriteName = spriteName;
        _localizationNameToGame = localizationNameToGame;
        _index = index;
        _price = price;
        _priceAdditional = priceAdditional;
        _keyAdd = false;
        _keyShowParams  = false;
        _keyShowStats = keyShowStats;
    }
}