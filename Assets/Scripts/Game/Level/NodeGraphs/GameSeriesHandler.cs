using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameSeriesHandler : MonoBehaviour
{
    [SerializeField, Expandable] private List<SeriaNodeGraphsHandler> _seriaNodeGraphsHandlers;

    private int _currentSeriaIndex;
    private NodeGraphInitializer _nodeGraphInitializer;
    private SwitchToNextSeriaEvent<bool> _switchToNextSeriaEvent;


    public int CurrentSeriaIndex => _currentSeriaIndex;
    public int CurrentNodeGraphIndex => _seriaNodeGraphsHandlers[_currentSeriaIndex].CurrentNodeGraphIndex;
    public int CurrentNodeIndex => _seriaNodeGraphsHandlers[_currentSeriaIndex].CurrentNodeIndex;
    
    public void Construct(NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        int currentSeriaIndex = 0, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _switchToNextSeriaEvent = switchToNextSeriaEvent;
        _switchToNextSeriaEvent.Subscribe(SwitchSeria);
        _currentSeriaIndex = currentSeriaIndex;
        _nodeGraphInitializer = nodeGraphInitializer;
        if (Application.isPlaying == false)
        {
            for (int i = 0; i < _seriaNodeGraphsHandlers.Count; ++i)
            {
                InitSeria(i, currentNodeGraphIndex, currentNodeIndex);

            }
        }
        else
        {
            InitSeria(_currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
        }
    }

    public void Dispose()
    {
        foreach (var handler in _seriaNodeGraphsHandlers)
        {
            handler.Dispose();
        }
    }
    public void AddSeria(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        _seriaNodeGraphsHandlers.Add(seriaNodeGraphsHandler);
    }

    private void InitSeria(int currentSeriaIndex, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _seriaNodeGraphsHandlers[currentSeriaIndex].Construct(_nodeGraphInitializer, currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }
    private void SwitchSeria(bool putSwimsuits = false)
    {
        if (_currentSeriaIndex < _seriaNodeGraphsHandlers.Count - 1)
        {
            _currentSeriaIndex++;
            InitSeria(_currentSeriaIndex);
        }
        else
        {
            //end game invoke result panel
        }
    }
}