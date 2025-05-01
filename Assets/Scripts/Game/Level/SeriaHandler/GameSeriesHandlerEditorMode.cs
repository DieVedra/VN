
using UnityEngine;

public class GameSeriesHandlerEditorMode : GameSeriesHandler
{
    public void Construct(NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        int currentSeriaIndex = 0, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
        SwitchToNextSeriaEvent.Subscribe(SwitchSeria);
        CurrentSeriaIndex = currentSeriaIndex;
        NodeGraphInitializer = nodeGraphInitializer;
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
}