using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CaseForStats
{
    [SerializeField] private bool _foldoutKey;
    [SerializeField] private List<CaseBaseStat> _caseStats;
    [SerializeField] private string _name;
    public string Name => _name;
    public IReadOnlyList<CaseBaseStat> CaseStats => _caseStats;
    public IReadOnlyList<ILocalizationString> StatsLocalizations => _caseStats;

    public CaseForStats(List<CaseBaseStat> caseStats, string name)
    {
        _caseStats = caseStats;
        _name = name;
    }
}