using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(400),NodeTint("#A53383")]
public class SwitchNode : BaseNode, IPutOnSwimsuit
{
    [Output] public Empty OutputTrueBool;
    [SerializeField] private List<CaseForStats> _casesForStats;
    [SerializeField] private bool _isNodeForStats;
    [SerializeField] private bool _isNodeForBool;
    [SerializeField] private int _removeAdditionalCaseIndex;
    private const int _maxDynamicPortsCount = 10;
    private const string _namePort = "OutputTrueBool";
    private const string _port = "Port ";
    private IGameStatsProvider _gameStatsProvider;
    private SwitchNodeLogic _switchNodeLogic;
    private SwitchNodeInitializer _switchNodeInitializer;
    private bool _putOnSwimsuit;
    public IReadOnlyList<CaseForStats> CaseLocalizations => _casesForStats;
    public SwitchNodeLogic SwitchNodeLogic => _switchNodeLogic;
    public void ConstructMySwitchNode(IGameStatsProvider gameStatsProvider, int seriaIndex)
    {
        _gameStatsProvider = gameStatsProvider;
        _switchNodeLogic = new SwitchNodeLogic(gameStatsProvider.GameStatsHandler.StatsDictionary, _gameStatsProvider.GetEmptyStatsFromCurrentSeria(seriaIndex));
        if (IsPlayMode() == false)
        {
            if (_switchNodeInitializer == null)
            {
                _switchNodeInitializer = new SwitchNodeInitializer(_gameStatsProvider.GetEmptyStatsFromCurrentSeria(seriaIndex));
            }
            _switchNodeInitializer.TryReinitAllCases(_casesForStats);
        }
    }
    public override UniTask Enter(bool isMerged = false)
    {
        if (_isNodeForStats)
        {
            var result = _switchNodeLogic.GetPortIndexOnSwitchResult(_casesForStats);
            bool caseFoundSuccessfuly = result.Item1;
            int indexCase = result.Item2;
            if (caseFoundSuccessfuly == true)
            {
                SetNextNode(DynamicOutputs.ElementAt(indexCase).Connection.node as BaseNode);
            }
            else
            {
                SetNextNode(OutputPortBaseNode.Connection.node as BaseNode);
            }
        }

        if (_isNodeForBool)
        {
            if (_putOnSwimsuit == true)
            {
                SetNextNode(GetOutputPort(_namePort).Connection.node as BaseNode);
            }
            else
            {
                SetNextNode(OutputPortBaseNode.Connection.node as BaseNode);
            }
        }

        if (isMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }

        return default;
    }
    private void AddDynamicPort()
    {
        if (DynamicOutputs.Count() < _maxDynamicPortsCount)
        {
            string name = $"{_port}{DynamicOutputs.Count()}";
            if (_casesForStats == null)
            {
                _casesForStats = new List<CaseForStats>();
            }
            var baseStats = _gameStatsProvider.GameStatsHandler.CreateCaseBaseStatForm();
            var caseForStats = new CaseForStats(baseStats, name);
            _casesForStats.Add(caseForStats);
            AddDynamicOutput(typeof(Empty), ConnectionType.Override, fieldName: name);
        }
    }
    private void RemoveStatDynamicPort()
    {
        if (_casesForStats != null && _casesForStats.Count > 0)
        {
            RemoveDynamicPort(_casesForStats[_casesForStats.Count - 1].Name);
            _casesForStats.Remove(_casesForStats[_casesForStats.Count - 1]);
        }
    }

    public void PutOnSwimsuit(bool putOnSwimsuitKey)
    {
        _putOnSwimsuit = putOnSwimsuitKey;
    }
}