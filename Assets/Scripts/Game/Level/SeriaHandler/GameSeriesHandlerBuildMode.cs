using System;
using UniRx;

public class GameSeriesHandlerBuildMode : GameSeriesHandler, ICurrentSeriaNodeGraphsProvider
{
    private const float _delay = 1f;
    private LevelLocalizationHandler _levelLocalizationHandler;
    private OnEndGameEvent _onEndGameEvent;
    private OnContentIsLoadProperty<bool> _onContentIsLoadProperty;
    private OnAwaitLoadContentEvent<AwaitLoadContentPanel> _onAwaitLoadContentEvent;
    private CurrentSeriaLoadedNumberProperty<int> _currentSeriaLoadedNumberProperty;
    private CompositeDisposable _compositeDisposable;
    public SeriaNodeGraphsHandler GetCurrentSeriaNodeGraphsHandler() => SeriaNodeGraphsHandlers[CurrentSeriaIndexReactiveProperty.Value];

    public void Construct(LevelLocalizationHandler levelLocalizationHandler, GameSeriesProvider gameSeriesProvider,
        NodeGraphInitializer nodeGraphInitializer,
        ReactiveProperty<int> currentSeriaIndexReactiveProperty, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        OnContentIsLoadProperty<bool> onContentIsLoadProperty, OnAwaitLoadContentEvent<AwaitLoadContentPanel> onAwaitLoadContentEvent,
        CurrentSeriaLoadedNumberProperty<int> currentSeriaLoadedNumberProperty, OnEndGameEvent onEndGameEvent,
        int currentNodeGraphIndex = 0, int currentNodeIndex = 0, bool putOnSwimsuitKey = false)
    {
        _levelLocalizationHandler = levelLocalizationHandler;
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
        _onEndGameEvent = onEndGameEvent;
        _onContentIsLoadProperty = onContentIsLoadProperty;
        _onAwaitLoadContentEvent = onAwaitLoadContentEvent;
        _currentSeriaLoadedNumberProperty = currentSeriaLoadedNumberProperty;
        SwitchToNextSeriaEvent.Subscribe(SwitchSeria);
        CurrentSeriaIndexReactiveProperty = currentSeriaIndexReactiveProperty;
        NodeGraphInitializer = nodeGraphInitializer;
        SeriaNodeGraphsHandlers[currentSeriaLoadedNumberProperty.GetValue].SeriaPartNodeGraphs[currentNodeGraphIndex].SetKeyPutOnSwimsuit(putOnSwimsuitKey);

        foreach (var seria in gameSeriesProvider.GetDatas)
        {
            AddSeria(seria);
        }
        gameSeriesProvider.OnLoad.Subscribe(AddSeria);
        
        InitSeria(CurrentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }

    protected override void InitSeria(int currentSeriaIndex, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _levelLocalizationHandler.TrySetLocalizationToCurrentLevelContent(SeriaNodeGraphsHandlers[currentSeriaIndex]);
        base.InitSeria(currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }

    private void AddSeria(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        SeriaNodeGraphsHandlers.Add(seriaNodeGraphsHandler);
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
                NodeGraphInitializer.SwitchToNextNodeEvent.Shutdown();
                NodeGraphInitializer.SwitchToNextNodeEvent.Shutdown();
                NodeGraphInitializer.SwitchToAnotherNodeGraphEvent.Shutdown();
                NodeGraphInitializer.DisableNodesContentEvent.Shutdown();
                NodeGraphInitializer.SwitchToNextSeriaEvent.Shutdown();
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