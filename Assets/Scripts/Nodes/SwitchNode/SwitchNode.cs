using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using XNode;

[NodeWidth(280),NodeTint("#A53383")]
public class SwitchNode : BaseNode, IPutOnSwimsuit
{
    [Output] public Empty OutputTrueBool;
    [SerializeField] private List<CaseForStats> _casesForStats;
    [SerializeField] private bool _isNodeForStats;
    [SerializeField] private bool _isNodeForBool;
    private const int _maxDynamicPortsCount = 10;
    private const string _namePort = "OutputTrueBool";
    private const string _port = "Port ";
    private const string _equalSymbol = "=";
    private const string _greatSymbol = ">";
    private const string _lessSymbol = "<";
    private const string _greatEqualSymbol = ">=";
    private const string _lessEqualSymbol = "<=";
    private readonly string[] _operators = {_equalSymbol, _greatSymbol, _lessSymbol, _greatEqualSymbol, _lessEqualSymbol};
    private IGameStatsProvider _gameStatsProvider;
    private SwitchNodeLogic _switchNodeLogic;
    private SwitchNodeInitializer _switchNodeInitializer;
    private bool _putOnSwimsuit;
    public IReadOnlyList<string> Operators => _operators;
    public IReadOnlyList<CaseForStats> CaseLocalizations => _casesForStats;
    public void ConstructMySwitchNode(IGameStatsProvider gameStatsProvider, int seriaIndex)
    {
        _gameStatsProvider = gameStatsProvider;
        _switchNodeLogic = new SwitchNodeLogic(_operators);
        if (IsPlayMode() == false)
        {
            if (_switchNodeInitializer == null)
            {
                _switchNodeInitializer = new SwitchNodeInitializer(_gameStatsProvider.GetStatsFromCurrentSeria(seriaIndex));
            }
            _switchNodeInitializer.TryReinitAllCases(_casesForStats);
        }
    }
    public override UniTask Enter(bool isMerged = false)
    {
        if (_isNodeForStats)
        {
            SwitchNodeLogicResult result = _switchNodeLogic.GetPortIndexOnSwitchResult(_gameStatsProvider.GameStatsHandler.Stats, _casesForStats);
            if (result.CaseFoundSuccessfuly == true)
            {
                SetNextNode(DynamicOutputs.ElementAt(result.IndexCase).Connection.node as BaseNode);
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
            var baseStats = _switchNodeInitializer.CreateCaseBaseStat();
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