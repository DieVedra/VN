using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChoiceCase
{
    [SerializeField] private LocalizationString _localizationChoiceText;
    [SerializeField] private string _choiceText;
    [SerializeField] private int _choicePrice;
    [SerializeField] private int _choiceAdditionaryPrice;
    [SerializeField] private bool _showStatsChoiceKey;
    [SerializeField] private bool _showNotificationChoice = false;
    [SerializeField] private List<BaseStat> _baseStatsChoice;
    public int ChoicePrice => _choicePrice;
    public int ChoiceAdditionaryPrice => _choiceAdditionaryPrice;
    public bool ShowStatsChoiceKey => _showStatsChoiceKey;
    public bool ShowNotificationChoice => _showNotificationChoice;

    public IReadOnlyList<ILocalizationString> BaseStatsChoiceLocalizations => _baseStatsChoice;
    public IEnumerable<BaseStat> BaseStatsChoice => _baseStatsChoice;
    public IReadOnlyList<BaseStat> BaseStatsChoiceIReadOnly => _baseStatsChoice;

    public ChoiceCase(List<BaseStat> baseStatsChoice, string choiceText,
        int choicePrice, int choiceAdditionaryPrice, bool showStatsChoiceKey, bool showNotificationChoice)
    {
        _choiceText = choiceText;
        _choicePrice = choicePrice;
        _choiceAdditionaryPrice = choiceAdditionaryPrice;
        _showStatsChoiceKey = showStatsChoiceKey;
        _showNotificationChoice = showNotificationChoice;
        _baseStatsChoice = baseStatsChoice;
    }

    public void InitLocalizationString()
    {
        _localizationChoiceText = new LocalizationString(_choiceText);
    }
    public ChoiceCase(List<BaseStat> baseStatsChoice)
    {
        _baseStatsChoice = baseStatsChoice;
    }
    
    public LocalizationString GetLocalizationString()
    {
        return _localizationChoiceText;
    }
}