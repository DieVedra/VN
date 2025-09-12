using System.Collections.Generic;
using NaughtyAttributes;
using UniRx;
using UnityEngine;

public class GameSeriesHandler : MonoBehaviour
{
    [SerializeField, Expandable] protected List<SeriaNodeGraphsHandler> SeriaNodeGraphsHandlers;

    protected NodeGraphInitializer NodeGraphInitializer;
    protected SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    protected ReactiveProperty<int> CurrentSeriaIndexReactiveProperty;
    protected ICharacterProvider CharacterProvider;
    public int CurrentSeriaIndex => CurrentSeriaIndexReactiveProperty.Value;
    
    public int CurrentNodeGraphIndex => SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeGraphIndex;
    public int CurrentNodeIndex => SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeIndex;

    public virtual void Dispose()
    {
        foreach (var handler in SeriaNodeGraphsHandlers)
        {
            handler.Dispose();
        }
    }

    protected virtual void InitSeria(int currentSeriaIndex, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        if (currentSeriaIndex > 0)
        {
            SeriaNodeGraphsHandlers[currentSeriaIndex - 1].Dispose();
        }
        SeriaNodeGraphsHandlers[currentSeriaIndex].Construct(NodeGraphInitializer, CharacterProvider, currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }
}