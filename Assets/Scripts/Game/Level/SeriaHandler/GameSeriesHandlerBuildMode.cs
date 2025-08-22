using UniRx;
using UnityEngine;

public class GameSeriesHandlerBuildMode : GameSeriesHandler
{
    private LevelLocalizationHandler _levelLocalizationHandler;
    private GameStatsHandler _gameStatsHandler;
    private OnEndGameEvent _onEndGameEvent;
    private OnContentIsLoadProperty<bool> _onContentIsLoadProperty;
    private OnAwaitLoadContentEvent<bool> _onAwaitLoadContentEvent;
    private CurrentSeriaLoadedNumberProperty<int> _currentSeriaLoadedNumberProperty;
    private CompositeDisposable _compositeDisposable;

    public void Construct(GameStatsHandler gameStatsHandler, LevelLocalizationHandler levelLocalizationHandler, GameSeriesProvider gameSeriesProvider,
        NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        ReactiveProperty<int> currentSeriaIndexReactiveProperty, OnContentIsLoadProperty<bool> onContentIsLoadProperty,
        OnAwaitLoadContentEvent<bool> onAwaitLoadContentEvent, CurrentSeriaLoadedNumberProperty<int> currentSeriaLoadedNumberProperty, 
        OnEndGameEvent onEndGameEvent,
        int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _gameStatsHandler = gameStatsHandler;
        _levelLocalizationHandler = levelLocalizationHandler;
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
        _onEndGameEvent = onEndGameEvent;
        _onContentIsLoadProperty = onContentIsLoadProperty;
        _onAwaitLoadContentEvent = onAwaitLoadContentEvent;
        _currentSeriaLoadedNumberProperty = currentSeriaLoadedNumberProperty;
        SwitchToNextSeriaEvent.Subscribe(SwitchSeria);
        CurrentSeriaIndexReactiveProperty = currentSeriaIndexReactiveProperty;
            NodeGraphInitializer = nodeGraphInitializer;
        AddSeria(gameSeriesProvider.LastLoaded);
        gameSeriesProvider.OnLoad.Subscribe(AddSeria);
        InitSeria(CurrentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
        levelLocalizationHandler.OnTrySwitchLocalization += TrySetCurrentLocalizationToCurrentSeria;
    }

    protected override void InitSeria(int currentSeriaIndex, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        // TrySetCurrentLocalizationToCurrentSeria();
        _levelLocalizationHandler.TrySetCurrentLocalization(SeriaNodeGraphsHandlers[currentSeriaIndex], _gameStatsHandler);
        base.InitSeria(currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }
    private void AddSeria(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        SeriaNodeGraphsHandlers.Add(seriaNodeGraphsHandler);
    }

    private void TrySetCurrentLocalizationToCurrentSeria()
    {
        _levelLocalizationHandler.TrySetCurrentLocalization(SeriaNodeGraphsHandlers[CurrentSeriaIndexReactiveProperty.Value], _gameStatsHandler);
    }
    private void SwitchSeria(bool putSwimsuits = false)
    {
        // _currentSeriaLoadedNumberProperty.GetValue
        // if (CurrentSeriaIndex < SeriaNodeGraphsHandlers.Count - 1)
        if (CurrentSeriaIndex < _currentSeriaLoadedNumberProperty.GetValue - 1)
        {
            Debug.Log($"SwitchSeria true  _currentSeriaLoadedNumberProperty.GetValue: {_currentSeriaLoadedNumberProperty.GetValue}   CurrentSeriaIndex {CurrentSeriaIndex}");

            InitOnSwitchSeria();
        }
        else
        {
            Debug.Log($"SwitchSeria False");

            if (_onContentIsLoadProperty.GetValue == true)
            {
                _compositeDisposable = _onContentIsLoadProperty.SubscribeWithCompositeDisposable(SeriaInitOnLoadFinal);
                _onAwaitLoadContentEvent.Execute(true);
            }
            else
            {
                NodeGraphInitializer.SwitchToNextNodeEvent.Dispose();
                NodeGraphInitializer.SendCurrentNodeEvent.Dispose();
                NodeGraphInitializer.SwitchToNextNodeEvent.Dispose();
                NodeGraphInitializer.SwitchToAnotherNodeGraphEvent.Dispose();
                NodeGraphInitializer.DisableNodesContentEvent.Dispose();
                NodeGraphInitializer.SwitchToNextSeriaEvent.Dispose();
                _onEndGameEvent.Execute();
            }
        }
    }

    private void SeriaInitOnLoadFinal(bool key)
    {
        if (_onContentIsLoadProperty.GetValue == false)
        {
            _compositeDisposable.Clear();
            _onAwaitLoadContentEvent.Execute(false);
            InitOnSwitchSeria();
        }
    }

    private void InitOnSwitchSeria()
    {
        CurrentSeriaIndexReactiveProperty.Value++;
        InitSeria(CurrentSeriaIndexReactiveProperty.Value);
    }
}