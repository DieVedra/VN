using NaughtyAttributes;
using UniRx;
using UnityEngine;

public class GameSeriesHandlerEditorMode : GameSeriesHandler
{
    [SerializeField, ReadOnly] private ReactiveProperty<int> _seriaIndexToShowOnInspector;
    [SerializeField, ReadOnly] private ReactiveProperty<int> _nodeGraphIndexToShowOnInspector;
    [SerializeField, ReadOnly] private ReactiveProperty<int> _nodeIndexToShowOnInspector;
    public void Construct(
        NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent, ReactiveProperty<int> currentSeriaIndexReactiveProperty,
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

        Construct();
        SubscribeReactivePropertiesToShowOnInspector();
    }

    private void SubscribeReactivePropertiesToShowOnInspector()
    {
        _seriaIndexToShowOnInspector = new ReactiveProperty<int>(CurrentSeriaIndexReactiveProperty.Value);
        _nodeIndexToShowOnInspector = new ReactiveProperty<int>(SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeIndex);
        _nodeGraphIndexToShowOnInspector = new ReactiveProperty<int>(SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeGraphIndex);

        this.ObserveEveryValueChanged(x => x.CurrentSeriaIndexReactiveProperty.Value)
            .Subscribe(newValue => _seriaIndexToShowOnInspector.Value = newValue)
            .AddTo(this);
        
        this.ObserveEveryValueChanged(x => x.SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeIndex)
            .Subscribe(newValue => _nodeIndexToShowOnInspector.Value = newValue)
            .AddTo(this);

        this.ObserveEveryValueChanged(x => x.SeriaNodeGraphsHandlers[CurrentSeriaIndex].CurrentNodeGraphIndex)
            .Subscribe(newValue => _nodeGraphIndexToShowOnInspector.Value = newValue)
            .AddTo(this);
    }
}