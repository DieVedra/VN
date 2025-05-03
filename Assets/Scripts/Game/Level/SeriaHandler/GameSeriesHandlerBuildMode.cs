
using System;
using UnityEngine;

public class GameSeriesHandlerBuildMode : GameSeriesHandler
{
    private GameSeriesProvider _gameSeriesProvider;
    public void Construct(GameSeriesProvider gameSeriesProvider, NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        int currentSeriaIndex = 0, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
        SwitchToNextSeriaEvent.Subscribe(SwitchSeria);
        CurrentSeriaIndex = currentSeriaIndex;
        _gameSeriesProvider = gameSeriesProvider;
        NodeGraphInitializer = nodeGraphInitializer;
        _gameSeriesProvider.OnLoad += AddSeria;
        if (Application.isPlaying == false)
        {
            for (int i = 0; i < SeriaNodeGraphsHandlers.Count; ++i)
            {
                InitSeria(i, currentNodeGraphIndex, currentNodeIndex);
            }
        }
        else
        {
            InitSeria(CurrentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
        }
    }

    private void OnDestroy()
    {
        _gameSeriesProvider.OnLoad -= AddSeria;
    }

    private void AddSeria(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        SeriaNodeGraphsHandlers.Add(seriaNodeGraphsHandler);
    }
}