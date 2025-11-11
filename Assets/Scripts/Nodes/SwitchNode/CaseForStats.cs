using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CaseForStats
{
    [SerializeField] private bool _foldoutKey;
    [SerializeField] private List<CaseBaseStat> _caseStats;
    [SerializeField] private List<AdditionalCaseStats> _additionalCaseStats;
    [SerializeField] private string _name;
    public string Name => _name;
    public IReadOnlyList<CaseBaseStat> CaseStats => _caseStats;
    public List<AdditionalCaseStats> AdditionalCaseStats => _additionalCaseStats;
    public IReadOnlyList<ILocalizationString> StatsLocalizations => _caseStats;

    public CaseForStats(List<CaseBaseStat> caseStats, string name, List<AdditionalCaseStats> additionalCaseStats = null)
    {
        _caseStats = caseStats;
        _additionalCaseStats = additionalCaseStats;
        _name = name;
    }
}