using System;
using System.Collections.Generic;

public class SeriaGameStatsProviderBuild : IGameStatsProvider
{
    public SeriaGameStatsProviderBuild()
    {
    }

    public List<Stat> GetStatsFromCurrentSeria(int seriaIndex)
    {
        throw new NotImplementedException();
    }

    public GameStatsHandler GameStatsHandler { get; }
    public event Action<List<Stat>> OnAddStats;
}