
using UnityEngine;

[System.Serializable]
public class TestModeEditor
{
    [SerializeField] private bool _isTestMode;
    [SerializeField] private int _seriaIndex;
    [SerializeField] private int _graphIndex;
    [SerializeField] private int _nodeIndex;
    
    public bool IsTestMode => _isTestMode;
    public int SeriaIndex => _seriaIndex;
    public int GraphIndex => _graphIndex;
    public int NodeIndex => _nodeIndex;
}