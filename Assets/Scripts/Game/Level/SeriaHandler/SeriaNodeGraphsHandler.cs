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
    private ICharacterProvider _characterProvider;
    private IReadOnlyList<Character> _seriaCharacters;
    public IReadOnlyList<SeriaPartNodeGraph> SeriaPartNodeGraphs => _seriaPartNodeGraphs;
    public void Construct(NodeGraphInitializer nodeGraphInitializer, ICharacterProvider characterProvider, int currentSeriaIndex,
        int currentNodeGraphIndex, int currentNodeIndex)
    {
        _nodeGraphInitializer = nodeGraphInitializer;
        CurrentNodeGraphIndex = currentNodeGraphIndex;
        _characterProvider = characterProvider;
        _currentSeriaIndex = currentSeriaIndex;
        _compositeDisposable = _nodeGraphInitializer.SwitchToNextNodeEvent.SubscribeWithCompositeDisposable(MoveNext);
        _nodeGraphInitializer.SwitchToAnotherNodeGraphEvent.Subscribe(SwitchToAnotherNodeGraph);
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
        InitGraph(currentNodeGraphIndex: CurrentNodeGraphIndex);
    }
    private void InitGraph(int currentSeriaIndex = 0, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _seriaPartNodeGraphs[currentNodeGraphIndex].Init(_nodeGraphInitializer, TryGetCharacters, currentSeriaIndex: currentSeriaIndex, currentNodeIndex: currentNodeIndex);
    }
    private int GetIndexCurrentNode(SeriaPartNodeGraph seriaPartNodeGraph)
    {
        return _seriaPartNodeGraphs.IndexOf(seriaPartNodeGraph);
    }
    private void MoveNext()
    {
        _seriaPartNodeGraphs[CurrentNodeGraphIndex].MoveNext().Forget();
    }

    private IReadOnlyList<Character> TryGetCharacters()
    {
        if (_seriaCharacters == null)
        {
            _seriaCharacters = _characterProvider.GetCharacters(_currentSeriaIndex);
        }
        return _seriaCharacters;
    }
}
