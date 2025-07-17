using UniRx;
using UnityEngine;

public class GameSeriesHandlerEditorMode : GameSeriesHandler
{
    public void Construct(NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent, ReactiveProperty<int> currentSeriaIndexReactiveProperty,
        int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
        SwitchToNextSeriaEvent.Subscribe(SwitchSeria);
        CurrentSeriaIndexReactiveProperty = currentSeriaIndexReactiveProperty;
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

    private void SwitchSeria(bool putSwimsuits = false)
    {
        if (CurrentSeriaIndex < SeriaNodeGraphsHandlers.Count - 1)
        {
            CurrentSeriaIndexReactiveProperty.Value++;
            InitSeria(CurrentSeriaIndex);
        }
        else
        {
            //end game invoke result panel
        }
    }
}