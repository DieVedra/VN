using System.Collections.Generic;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

[CreateAssetMenu(fileName = "SeriaNodeGraphsHandler", menuName = "NodeGraphs/SeriaNodeGraphsHandler", order = 51)]
public class SeriaNodeGraphsHandler : ScriptableObject
{
    [SerializeField, Expandable] private List<SeriaPartNodeGraph> _seriaPartNodeGraphs;
    private CompositeDisposable _compositeDisposable;
    public int CurrentNodeGraphIndex { get; private set; }
    public int CurrentNodeIndex => _seriaPartNodeGraphs[CurrentNodeGraphIndex].CurrentNodeIndex;

    private int _currentSeriaIndex;
    private NodeGraphInitializer _nodeGraphInitializer;
    private ReactiveProperty<bool> _putOnSwimsuitKey;
    private ICharacterProvider _characterProvider;
    public IReadOnlyList<SeriaPartNodeGraph> SeriaPartNodeGraphs => _seriaPartNodeGraphs;
    public void Construct(ReactiveProperty<bool> putOnSwimsuitKey, NodeGraphInitializer nodeGraphInitializer, ICharacterProvider characterProvider,
        int currentSeriaIndex, int currentNodeGraphIndex, int currentNodeIndex)
    {
        _putOnSwimsuitKey = putOnSwimsuitKey;
        _nodeGraphInitializer = nodeGraphInitializer;
        CurrentNodeGraphIndex = currentNodeGraphIndex;
        _characterProvider = characterProvider;
        _currentSeriaIndex = currentSeriaIndex;
        _compositeDisposable = _nodeGraphInitializer.SwitchToNextNodeEvent.SubscribeWithCompositeDisposable(MoveNext);
        _nodeGraphInitializer.SwitchToAnotherNodeGraphEvent.SubscribeWithCompositeDisposable(SwitchToAnotherNodeGraph, _compositeDisposable);
        if (_seriaPartNodeGraphs.Count > 0)
        {
            if (Application.isPlaying == true)
            {
                InitGraph(currentSeriaIndex: currentSeriaIndex, currentNodeGraphIndex: currentNodeGraphIndex, currentNodeIndex: currentNodeIndex);
            }
            else
            {
                for (int i = 0; i < _seriaPartNodeGraphs.Count; i++)
                {
                    InitGraph(currentSeriaIndex: currentSeriaIndex, currentNodeGraphIndex: i);
                }
            }
        }
    }

    public void Dispose()
    {
        _compositeDisposable?.Clear();
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
        InitGraph(currentSeriaIndex: _currentSeriaIndex, currentNodeGraphIndex: CurrentNodeGraphIndex);
    }
    private void InitGraph(int currentSeriaIndex = 0, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _seriaPartNodeGraphs[currentNodeGraphIndex].Init(_putOnSwimsuitKey, _nodeGraphInitializer, currentSeriaIndex: currentSeriaIndex, currentNodeIndex: currentNodeIndex);
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
