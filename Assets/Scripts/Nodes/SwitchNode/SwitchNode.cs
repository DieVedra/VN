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
    [SerializeField, HideInInspector] private List<CaseForStats> _casesForStats;
    [SerializeField] private bool _isNodeForStats;
    [SerializeField] private bool _isNodeForBool;
    private readonly int _maxDynamicPortsCount = 10;
    private readonly string[] _operators = new [] {"=", ">", "<", ">=", "<="};
    private int _seriaIndex;
    private IGameStatsProvider _gameStatsProvider;
    private SwitchNodeLogic _switchNodeLogic;
    private bool _putOnSwimsuit;
    public IReadOnlyList<string> Operators => _operators;
    public IReadOnlyList<CaseForStats> CaseLocalizations => _casesForStats;
    public void ConstructMySwitchNode(IGameStatsProvider gameStatsProvider, int seriaIndex)
    {
        _gameStatsProvider = gameStatsProvider;
        _switchNodeLogic = new SwitchNodeLogic(_operators);
        _seriaIndex = seriaIndex;
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
                SetNextNode(GetOutputPort("OutputTrueBool").Connection.node as BaseNode);
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
    
    private void TryReInitCases()
    {
        if (_casesForStats != null)
        {
            List<CaseBaseStat> newStats = null;
            for (int i = 0; i < _casesForStats.Count; i++)
            {
                newStats = CreateCaseBaseStat();

                if (_casesForStats[i].CaseStats.Count > newStats.Count)
                {
                    for (int j = 0; j < newStats.Count; j++)
                    {
                        ReInitStat(ref newStats, j, _casesForStats[i].CaseStats[j]);
                    }
                }
                else if (_casesForStats[i].CaseStats.Count < newStats.Count)
                {
                    for (int j = 0; j < _casesForStats[i].CaseStats.Count; j++)
                    {
                        ReInitStat(ref newStats, j, _casesForStats[i].CaseStats[j]);
                    }
                }

                _casesForStats[i] = new CaseForStats(newStats, _casesForStats[i].Name);
            }
        }
    }
    private void AddDynamicPort()
    {
        if (DynamicOutputs.Count() < _maxDynamicPortsCount)
        {
            string name = $"Port {DynamicOutputs.Count()}";
            if (_casesForStats == null)
            {
                _casesForStats = new List<CaseForStats>();
            }
            _casesForStats.Add(new CaseForStats(CreateCaseBaseStat(), name));
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
    private List<CaseBaseStat> CreateCaseBaseStat()
    {
        List<BaseStat> stats = _gameStatsProvider.GameStatsHandler.GetGameBaseStatsForm();
        List<CaseBaseStat> caseStats = new List<CaseBaseStat>(stats.Count);
        for (int i = 0; i < stats.Count; i++)
        {
            caseStats.Add(new CaseBaseStat(stats[i].Name, stats[i].Value, 0, false));
        }
        return caseStats;
    }
    private void ReInitStat(ref List<CaseBaseStat> newStats, int index, CaseBaseStat oldStat)
    {
        if (oldStat.Name == newStats[index].Name)
        {
            newStats[index] = new CaseBaseStat(oldStat.Name,
                oldStat.Value,
                oldStat.IndexCurrentOperator,
                oldStat.IncludeKey);
        }
    }

    public void PutOnSwimsuit(bool putOnSwimsuitKey)
    {
        _putOnSwimsuit = putOnSwimsuitKey;
    }
}