using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class GameSeriesHandler : MonoBehaviour
{
    [SerializeField, Expandable] protected List<SeriaNodeGraphsHandler> SeriaNodeGraphsHandlers;

    protected NodeGraphInitializer NodeGraphInitializer;
    protected SwitchToNextSeriaEvent<bool> SwitchToNextSeriaEvent;
    
    public int CurrentSeriaIndex {get; protected set; }
    public int CurrentNodeGraphIndex => SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeGraphIndex;
    public int CurrentNodeIndex => SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeIndex;

    public void Dispose()
    {
        foreach (var handler in SeriaNodeGraphsHandlers)
        {
            handler.Dispose();
        }
    }

    protected void InitSeria(int currentSeriaIndex, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        AddWardrobeData(currentSeriaIndex);
        SeriaNodeGraphsHandlers[currentSeriaIndex].Construct(NodeGraphInitializer, currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }
    protected void SwitchSeria(bool putSwimsuits = false)
    {
        if (CurrentSeriaIndex < SeriaNodeGraphsHandlers.Count - 1)
        {
            CurrentSeriaIndex++;
            InitSeria(CurrentSeriaIndex);
        }
        else
        {
            //end game invoke result panel
        }
    }
    protected void AddWardrobeData(int seriaIndex)
    {
        var wardrobeSeriaData = NodeGraphInitializer.WardrobeSeriaDataProvider.GetWardrobeSeriaData(seriaIndex);
        if (wardrobeSeriaData != null && wardrobeSeriaData.MySeriaIndex == seriaIndex)
        {
            NodeGraphInitializer.CustomizableCharacter.AddWardrobeDataSeria(wardrobeSeriaData);
        }
    }
}