using System;
using System.Collections.Generic;

public interface IGameStatsProvider
{
    public List<Stat> GetStatsFromCurrentSeria(int seriaIndex);
    public GameStatsHandler GameStatsHandler { get; }
}