
using System.Collections.Generic;

public class ChoiceNodeInitializer : MyNodeInitializer
{
    public ChoiceNodeInitializer(List<Stat> stats) : base(stats) { }
    
    public void TryInitStats(ref List<BaseStat> oldBaseStatsChoice)
    {
        if (oldBaseStatsChoice == null || oldBaseStatsChoice.Count != _gameStatsHandler.Stats.Count)
        {
            List<BaseStat> newBaseStats = _gameStatsHandler.GetGameBaseStatsForm();
            if (oldBaseStatsChoice != null && oldBaseStatsChoice.Count > 0)
            {
                if (oldBaseStatsChoice.Count > newBaseStats.Count)
                {
                    for (int i = 0; i < newBaseStats.Count; i++)
                    {
                        ReInitStat(ref newBaseStats, i, oldBaseStatsChoice[i]);
                    }
                }
                else if (oldBaseStatsChoice.Count < newBaseStats.Count)
                {
                    for (int i = 0; i < oldBaseStatsChoice.Count; i++)
                    {
                        ReInitStat(ref newBaseStats, i, oldBaseStatsChoice[i]);
                    }
                }
            }
            oldBaseStatsChoice = newBaseStats;
        }
    }
    private void ReInitStat(ref List<BaseStat> newStats, int index, BaseStat oldStat)
    {
        if (newStats[index].Name == oldStat.Name)
        {
            newStats[index] = new BaseStat(oldStat.Name, oldStat.Value);
        }
    }
}