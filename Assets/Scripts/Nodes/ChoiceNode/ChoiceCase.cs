using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ChoiceCase
{
    [SerializeField] private LocalizationString _localizationChoiceText;
    [SerializeField] private string _choiceText;
    [SerializeField] private int _choicePrice;
    [SerializeField] private int _choiceAdditionaryPrice;
    [SerializeField] private bool _showNotificationChoice = false;
    [SerializeField] private List<BaseStat> _baseStatsChoice;
    public int ChoicePrice => _choicePrice;
    public int ChoiceAdditionaryPrice => _choiceAdditionaryPrice;
    public bool ShowNotificationChoice => _showNotificationChoice;

    public IReadOnlyList<ILocalizationString> BaseStatsChoiceLocalizations => _baseStatsChoice;
    public IEnumerable<BaseStat> BaseStatsChoice => _baseStatsChoice;
    public IReadOnlyList<BaseStat> BaseStatsChoiceIReadOnly => _baseStatsChoice;

    public ChoiceCase(List<BaseStat> baseStatsChoice, LocalizationString choiceText,
        int choicePrice, int choiceAdditionaryPrice, bool showNotificationChoice)
    {
        _choiceText = choiceText;
        _localizationChoiceText = choiceText;
        _choicePrice = choicePrice;
        _choiceAdditionaryPrice = choiceAdditionaryPrice;
        _showNotificationChoice = showNotificationChoice;
        _baseStatsChoice = baseStatsChoice;
    }

    public ChoiceCase(List<BaseStat> baseStatsChoice)
    {
        _baseStatsChoice = baseStatsChoice;
    }

    public void InitLocalizationString(string text)
    {
        _localizationChoiceText = new LocalizationString(text);
    }

    public LocalizationString GetLocalizationString()
    {
        return _localizationChoiceText;
    }
}