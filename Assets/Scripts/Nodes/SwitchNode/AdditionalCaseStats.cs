using UnityEngine;

[System.Serializable]
public class AdditionalCaseStats
{
    [SerializeField] private int _indexCurrentOperator;
    [SerializeField] private int _indexStat1;
    [SerializeField] private int _indexStat2;
    [SerializeField] private LocalizationString _localizationStringStat1;
    [SerializeField] private LocalizationString _localizationStringStat2;
    public int IndexCurrentOperator => _indexCurrentOperator;
    public int IndexStat1 => _indexStat1;
    public int IndexStat2 => _indexStat2;
    public string Stat1Key => _localizationStringStat1.Key;
    public string Stat2Key => _localizationStringStat2.Key;
}