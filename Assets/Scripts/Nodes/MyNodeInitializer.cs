

using System.Collections.Generic;

public class MyNodeInitializer
{
    protected readonly GameStatsHandler GameStatsHandler;

    protected MyNodeInitializer(List<Stat> stats)
    {
        GameStatsHandler = new GameStatsHandler(stats);
    }
    protected MyNodeInitializer(GameStatsHandler gameStatsHandler)
    {
        GameStatsHandler = gameStatsHandler;
    }
}