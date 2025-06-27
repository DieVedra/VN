

using System.Collections.Generic;

public class MyNodeInitializer
{
    protected GameStatsHandler _gameStatsHandler;
    public GameStatsHandler GameStatsHandler => _gameStatsHandler;

    protected MyNodeInitializer(List<Stat> stats)
    {
        _gameStatsHandler = new GameStatsHandler(stats);
    }
    protected MyNodeInitializer(GameStatsHandler gameStatsHandler)
    {
        _gameStatsHandler = gameStatsHandler;
    }
}