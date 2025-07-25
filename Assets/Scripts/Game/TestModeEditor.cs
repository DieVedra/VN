
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TestModeEditor
{
    [SerializeField] private bool _isTestMode;
    [SerializeField] private int _seriaIndex;
    [SerializeField] private int _graphIndex;
    [SerializeField] private int _nodeIndex;
    [SerializeField] private int _addMonets;
    [SerializeField] private int _addHearts;
    [SerializeField] private List<BaseStat> _stats;
    public bool IsTestMode => _isTestMode;
    public int SeriaIndex => _seriaIndex;
    public int GraphIndex => _graphIndex;
    public int NodeIndex => _nodeIndex;
    public IReadOnlyList<BaseStat> Stats => _stats;

    public void SetStats(List<BaseStat> stats)
    {
        _stats = stats;
    }
    public void TryUpdateStats()
    {
        // if (_stats)
        // {
        //     
        // }
    }
}