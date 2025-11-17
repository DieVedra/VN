

using System.Collections.Generic;

public class MyNodeInitializer
{
    protected readonly GameStatsHandler GameStatsHandlerNodeInitializer;

    protected MyNodeInitializer(List<Stat> stats)
    {
        GameStatsHandlerNodeInitializer = new GameStatsHandler(stats);
    }
    protected MyNodeInitializer(GameStatsHandler gameStatsHandlerNodeInitializer)
    {
        GameStatsHandlerNodeInitializer = gameStatsHandlerNodeInitializer;
    }
}