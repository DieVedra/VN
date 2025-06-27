using System;
using System.Collections.Generic;

public class SeriaGameStatsProviderBuild : IGameStatsProvider
{
    public SeriaGameStatsProviderBuild()
    {
    }

    public List<Stat> GetStatsFromCurrentSeria(int seriaIndex)
    {
        return null;
    }

    public GameStatsHandler GameStatsHandler { get; }
    public event Action<List<Stat>> OnAddStats;
}