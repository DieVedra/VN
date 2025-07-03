using UniRx;
using UnityEngine;

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

    private void AddSeria(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        _levelLocalizationHandler.TrySetCurrentLocalization(seriaNodeGraphsHandler, _gameStatsHandler);
        Debug.Log(555);
        SeriaNodeGraphsHandlers.Add(seriaNodeGraphsHandler);
    }
}