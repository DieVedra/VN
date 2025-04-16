using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "SeriaNodeGraphsHandler", menuName = "NodeGraphs/SeriaNodeGraphsHandler", order = 51)]
public class SeriaNodeGraphsHandler : ScriptableObject
{
    [SerializeField, Expandable] private List<SeriaPartNodeGraph> _seriaPartNodeGraphs;

    public int CurrentNodeGraphIndex { get; private set; }
    public int CurrentNodeIndex => _seriaPartNodeGraphs[CurrentNodeGraphIndex].CurrentNodeIndex;

    private NodeGraphInitializer _nodeGraphInitializer;
    public void Construct(NodeGraphInitializer nodeGraphInitializer,
        int currentNodeGraphIndex, int currentNodeIndex)
    {
        _nodeGraphInitializer = nodeGraphInitializer;
        CurrentNodeGraphIndex = currentNodeGraphIndex;
        _nodeGraphInitializer.SwitchToNextNodeEvent.Subscribe(MoveNext);
        _nodeGraphInitializer.SwitchToAnotherNodeGraphEvent.Subscribe(SwitchToAnotherNodeGraph);
        if (_seriaPartNodeGraphs.Count > 0)
        {
            if (Application.isPlaying == true)
            {
                InitCurrentGraph(currentNodeIndex);
            }
            else
            {
                for (int i = 0; i < _seriaPartNodeGraphs.Count; i++)
                {
                    _seriaPartNodeGraphs[i].Init(_nodeGraphInitializer);
                }
            }
        }
    }

    public void Dispose()
    {
        if (_seriaPartNodeGraphs.Count > 0)
        {
            foreach (var graph in _seriaPartNodeGraphs)
            {
                graph.Dispose();
            }
        }
    }
    private void SwitchToAnotherNodeGraph(SeriaPartNodeGraph seriaPartNodeGraph)
    {
        CurrentNodeGraphIndex = GetIndexCurrentNode(seriaPartNodeGraph);
        InitCurrentGraph();
    }
    private void InitCurrentGraph(int currentNodeIndex = 0)
    {
        _seriaPartNodeGraphs[CurrentNodeGraphIndex].Init(_nodeGraphInitializer, currentNodeIndex);

    }
    private int GetIndexCurrentNode(SeriaPartNodeGraph seriaPartNodeGraph)
    {
        return _seriaPartNodeGraphs.IndexOf(seriaPartNodeGraph);
    }
    private void MoveNext()
    {
        _seriaPartNodeGraphs[CurrentNodeGraphIndex].MoveNext().Forget();
    }
}
