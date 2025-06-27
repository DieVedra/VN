

using System.Collections.Generic;

public class MyNodeInitializer
{
    protected GameStatsHandler _gameStatsHandler;
    public GameStatsHandler GameStatsHandler => _gameStatsHandler;

    public MyNodeInitializer(List<Stat> stats)
    {
        _gameStatsHandler = new GameStatsHandler(stats);
    }
}