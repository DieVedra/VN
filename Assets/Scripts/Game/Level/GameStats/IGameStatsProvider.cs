using System.Collections.Generic;

public interface IGameStatsProvider
{
    public List<Stat> GetEmptyStatsFromCurrentSeria(int seriaIndex);
    public GameStatsHandler GameStatsHandler { get; }
}