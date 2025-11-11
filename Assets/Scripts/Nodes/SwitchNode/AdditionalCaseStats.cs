using UnityEngine;

[System.Serializable]
public class AdditionalCaseStats
{
    [SerializeField] private int _indexCurrentOperator;
    [SerializeField] private int _indexStat1;
    [SerializeField] private int _indexStat2;
    [SerializeField] private string _stat1Key;
    [SerializeField] private string _stat2Key;
    public int IndexCurrentOperator => _indexCurrentOperator;
    public int IndexStat1 => _indexStat1;
    public int IndexStat2 => _indexStat2;
    public string Stat1Key => _stat1Key;
    public string Stat2Key => _stat2Key;
}