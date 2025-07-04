using UniRx;

public class GameSeriesHandlerBuildMode : GameSeriesHandler
{
    private LevelLocalizationHandler _levelLocalizationHandler;
    private GameStatsHandler _gameStatsHandler;
    public void Construct(GameStatsHandler gameStatsHandler, LevelLocalizationHandler levelLocalizationHandler, GameSeriesProvider gameSeriesProvider, NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,
        int currentSeriaIndex = 0, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _gameStatsHandler = gameStatsHandler;
        _levelLocalizationHandler = levelLocalizationHandler;
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
        SwitchToNextSeriaEvent.Subscribe(SwitchSeria);
        CurrentSeriaIndex = currentSeriaIndex;
        NodeGraphInitializer = nodeGraphInitializer;
        AddSeria(gameSeriesProvider.LastLoaded);
        gameSeriesProvider.OnLoad.Subscribe(AddSeria);
        InitSeria(CurrentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }

    protected override void InitSeria(int currentSeriaIndex, int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _levelLocalizationHandler.TrySetCurrentLocalization(SeriaNodeGraphsHandlers[currentSeriaIndex], _gameStatsHandler);
        base.InitSeria(currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }
    private void SwitchSeria(bool putSwimsuits = false)
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
    private void AddSeria(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        SeriaNodeGraphsHandlers.Add(seriaNodeGraphsHandler);
    }
}