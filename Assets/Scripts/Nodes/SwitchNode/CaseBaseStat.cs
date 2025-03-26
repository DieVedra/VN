using System;
using UnityEngine;

[Serializable]
public class CaseBaseStat : BaseStat
{
    [SerializeField] private int _indexCurrentOperator;
    [SerializeField] private bool _includeKey;
    
    public int IndexCurrentOperator => _indexCurrentOperator;
    public bool IncludeKey => _includeKey;
    public CaseBaseStat(string name, int value, int indexCurrentOperator, bool includeKey) : base(name, value)
    {
        _indexCurrentOperator = indexCurrentOperator;
        _includeKey = includeKey;
    }
}