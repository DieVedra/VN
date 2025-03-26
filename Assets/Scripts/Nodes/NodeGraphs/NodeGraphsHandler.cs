using System.Collections.Generic;
using UnityEngine;

public class NodeGraphsHandler : MonoBehaviour
{
    [SerializeField] private List<LevelPartNodeGraph> _levelPartNodeGraphs;

    public int CurrentNodeGraphIndex { get; private set; }
    public int CurrentNodeIndex => _levelPartNodeGraphs[CurrentNodeGraphIndex].CurrentNodeIndex;

    private NodeGraphInitializer _nodeGraphInitializer;
    public void Init(NodeGraphInitializer nodeGraphInitializer,
        SwitchToNextNodeEvent switchToNextNodeEvent, SwitchToAnotherNodeGraphEvent switchToAnotherNodeGraphEvent,
        int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _nodeGraphInitializer = nodeGraphInitializer;
        CurrentNodeGraphIndex = currentNodeGraphIndex;
        switchToNextNodeEvent.Subscribe(MoveNext);
        switchToAnotherNodeGraphEvent.Subscribe(SwitchToAnotherNodeGraph);
        if (Application.isPlaying == true)
        {
            InitCurrentGraph();
        }
        else
        {
            if (_levelPartNodeGraphs.Count > 0)
            {
                for (int i = 0; i < _levelPartNodeGraphs.Count; i++)
                {
                    _levelPartNodeGraphs[i].Init(_nodeGraphInitializer);
                }
            }
        }
        
    }

    public void Dispose()
    {
        if (_levelPartNodeGraphs.Count > 0)
        {
            foreach (var graph in _levelPartNodeGraphs)
            {
                graph.Dispose();
            }
        }
    }
    private void SwitchToAnotherNodeGraph(LevelPartNodeGraph levelPartNodeGraph)
    {
        CurrentNodeGraphIndex = GetIndexCurrentNode(levelPartNodeGraph);
        InitCurrentGraph();
    }
    private void InitCurrentGraph()
    {
        // Debug.Log($"Init {_levelPartNodeGraphs[CurrentNodeGraphIndex].name}");
        _levelPartNodeGraphs[CurrentNodeGraphIndex].Init(_nodeGraphInitializer);

    }
    private int GetIndexCurrentNode(LevelPartNodeGraph levelPartNodeGraph)
    {
        return _levelPartNodeGraphs.IndexOf(levelPartNodeGraph);
    }
    private void MoveNext()
    {
        _levelPartNodeGraphs[CurrentNodeGraphIndex].MoveNext().Forget();
    }
}
