using System.Collections.Generic;

public interface IGameStatsProvider
{
    public List<Stat> GetEmptyStatsFromCurrentSeria(int seriaIndex);
    public List<T> GetEmptyTStat<T>(int seriaIndex) where T : BaseStat;
    public GameStatsHandler GameStatsHandler { get; }
}