using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class CaseForStats
{
    [SerializeField] private List<CaseBaseStat> _caseStats;
    [SerializeField] private List<AdditionalCaseStats> _additionalCaseStats;
    [SerializeField] private bool _foldoutKey;
    [SerializeField] private string _name;
    public string Name => _name;
    public IReadOnlyList<CaseBaseStat> CaseStats => _caseStats;
    public IReadOnlyList<AdditionalCaseStats> AdditionalCaseStats => _additionalCaseStats;
    public IReadOnlyList<ILocalizationString> StatsLocalizations => _caseStats;
    public bool FoldoutKey => _foldoutKey;

    public CaseForStats(List<CaseBaseStat> caseStats, string name, bool foldoutKey = false, IReadOnlyList<AdditionalCaseStats> additionalCaseStats = null)
    {
        _caseStats = caseStats;
        _additionalCaseStats = additionalCaseStats?.ToList();
        _name = name;
        _foldoutKey = foldoutKey;
    }

    public void RemoveAdditionalElement(int index)
    {
        _additionalCaseStats.RemoveAt(index);
    }
}