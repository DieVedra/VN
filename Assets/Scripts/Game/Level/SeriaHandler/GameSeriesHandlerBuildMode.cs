﻿using UniRx;

public class GameSeriesHandlerBuildMode : GameSeriesHandler
{
    private LevelLocalizationHandler _levelLocalizationHandler;
    private GameStatsHandler _gameStatsHandler;
    public void Construct(GameStatsHandler gameStatsHandler, LevelLocalizationHandler levelLocalizationHandler, GameSeriesProvider gameSeriesProvider,
        NodeGraphInitializer nodeGraphInitializer, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent, ReactiveProperty<int> currentSeriaIndexReactiveProperty,
        int currentNodeGraphIndex = 0, int currentNodeIndex = 0)
    {
        _gameStatsHandler = gameStatsHandler;
        _levelLocalizationHandler = levelLocalizationHandler;
        SwitchToNextSeriaEvent = switchToNextSeriaEvent;
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
        TrySetCurrentLocalizationToCurrentSeria();
        // _levelLocalizationHandler.TrySetCurrentLocalization(SeriaNodeGraphsHandlers[currentSeriaIndex], _gameStatsHandler);
        base.InitSeria(currentSeriaIndex, currentNodeGraphIndex, currentNodeIndex);
    }
    private void SwitchSeria(bool putSwimsuits = false)
    {
        if (CurrentSeriaIndex < SeriaNodeGraphsHandlers.Count - 1)
        {
            CurrentSeriaIndexReactiveProperty.Value++;
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

    private void TrySetCurrentLocalizationToCurrentSeria()
    {
        _levelLocalizationHandler.TrySetCurrentLocalization(SeriaNodeGraphsHandlers[CurrentSeriaIndexReactiveProperty.Value], _gameStatsHandler);
    }
}