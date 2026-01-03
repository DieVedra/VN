using System.Collections.Generic;
using NaughtyAttributes;
using UniRx;
using UnityEngine;
using XNode;

public class GameSeriesHandler : MonoBehaviour
{
    [SerializeField, Expandable] protected List<SeriaNodeGraphsHandler> SeriaNodeGraphsHandlers;

    private const string _outputPortName = "Output";
    public const string InputPortName = "Input";
    protected NodeGraphInitializer NodeGraphInitializer;
    protected SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    protected ReactiveProperty<int> CurrentSeriaIndexReactiveProperty;
    public int CurrentSeriaIndex => CurrentSeriaIndexReactiveProperty.Value;
    
    public int CurrentNodeGraphIndex => SeriaNodeGraphsHandlers[CurrentSeriaIndexReactiveProperty.Value].CurrentNodeGraphIndex;
    // public int CurrentNodeIndex => SeriaNodeGraphsHandlers[CurrentSeriaIndexReactiveProperty.Value].CurrentNodeIndex;
    public bool PutOnSwimsuitKeyProperty => SeriaNodeGraphsHandlers[CurrentSeriaIndexReactiveProperty.Value].SeriaPartNodeGraphs[CurrentNodeGraphIndex].PutOnSwimsuitKey;

    public virtual void Shutdown()
    {
        foreach (var handler in SeriaNodeGraphsHandlers)
        {
            handler.Shutdown();
        }
    }

    protected virtual void InitSeria(int currentSeriaIndex, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        if (currentSeriaIndex > 0)
        {
            SeriaNodeGraphsHandlers[currentSeriaIndex - 1].Shutdown();
        }
        SeriaNodeGraphsHandlers[currentSeriaIndex].Construct(NodeGraphInitializer, currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }
    public NodePort GetNodePort(int nodeIndex)
    {
        return SeriaNodeGraphsHandlers[CurrentSeriaIndex].SeriaPartNodeGraphs[CurrentNodeGraphIndex].nodes[nodeIndex].GetOutputPort(_outputPortName);
    }

    public void GetInfoToSave(StoryData data)
    {
        data.CurrentSeriaIndex = CurrentSeriaIndex;
        data.CurrentNodeGraphIndex = CurrentNodeGraphIndex;
        data.CurrentNodeIndex = SeriaNodeGraphsHandlers[CurrentSeriaIndex].SeriaPartNodeGraphs[CurrentNodeGraphIndex].NodeIndexToSave;
    }
}