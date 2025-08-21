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
    public int CurrentSeriaIndex => CurrentSeriaIndexReactiveProperty.Value;
    
    public int CurrentNodeGraphIndex => SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeGraphIndex;
    public int CurrentNodeIndex => SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeIndex;

    public void Dispose()
    {
        foreach (var handler in SeriaNodeGraphsHandlers)
        {
            handler.Dispose();
        }
    }

    protected virtual void InitSeria(int currentSeriaIndex, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        AddWardrobeData(currentSeriaIndex);
        SeriaNodeGraphsHandlers[currentSeriaIndex].Construct(NodeGraphInitializer, currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }
    private void AddWardrobeData(int seriaIndex)
    {
        var wardrobeSeriaData = NodeGraphInitializer.WardrobeSeriaDataProvider.GetWardrobeSeriaData(seriaIndex);
        if (wardrobeSeriaData != null && wardrobeSeriaData.MySeriaIndex == seriaIndex)
        {
            foreach (var customizableCharacter in NodeGraphInitializer.CustomizableCharacters)
            {
                customizableCharacter.AddWardrobeDataSeria(wardrobeSeriaData);
            }
        }
    }
}