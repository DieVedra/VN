using System;
using UniRx;

public class GameSeriesHandlerBuildMode : GameSeriesHandler
{
    private const float _delay = 1f;
    private LevelLocalizationHandler _levelLocalizationHandler;
    private GameStatsHandler _gameStatsHandler;
    private OnEndGameEvent _onEndGameEvent;
    private OnContentIsLoadProperty<bool> _onContentIsLoadProperty;
    private OnAwaitLoadContentEvent<AwaitLoadContentPanel> _onAwaitLoadContentEvent;
    private CurrentSeriaLoadedNumberProperty<int> _currentSeriaLoadedNumberProperty;
    private CompositeDisposable _compositeDisposable;

    public void Construct(GameStatsHandler gameStatsHandler, LevelLocalizationHandler levelLocalizationHandler, GameSeriesProvider gameSeriesProvider,
        NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        ReactiveProperty<int> currentSeriaIndexReactiveProperty, OnContentIsLoadProperty<bool> onContentIsLoadProperty,
        OnAwaitLoadContentEvent<AwaitLoadContentPanel> onAwaitLoadContentEvent, CurrentSeriaLoadedNumberProperty<int> currentSeriaLoadedNumberProperty, 
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

    public override void Dispose()
    {
        _levelLocalizationHandler.OnTrySwitchLocalization -= TrySetCurrentLocalizationToCurrentSeria;
        base.Dispose();
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
        if (CurrentSeriaIndex < _currentSeriaLoadedNumberProperty.GetValue - 1)
        {
            InitOnSwitchSeria();
        }
        else
        {
            if (_onContentIsLoadProperty.GetValue == true)
            {
                _compositeDisposable = _onContentIsLoadProperty.SubscribeWithCompositeDisposable(SeriaInitOnLoadFinal);
                _onAwaitLoadContentEvent.Execute(AwaitLoadContentPanel.Show);
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
            Observable.Timer(TimeSpan.FromSeconds(_delay)).Subscribe(_ =>
            {
                _onAwaitLoadContentEvent.Execute(AwaitLoadContentPanel.Hide);
                InitOnSwitchSeria();
                _compositeDisposable.Clear();
            }).AddTo(_compositeDisposable);
        }
    }

    private void InitOnSwitchSeria()
    {
        CurrentSeriaIndexReactiveProperty.Value++;
        InitSeria(CurrentSeriaIndexReactiveProperty.Value);
    }
}