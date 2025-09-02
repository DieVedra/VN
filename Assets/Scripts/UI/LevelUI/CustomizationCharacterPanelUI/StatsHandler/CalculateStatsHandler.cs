using System.Collections.Generic;
using System.Linq;

public class CalculateStatsHandler
{
    public List<BaseStat> PreliminaryStats;
    private List<BaseStat> _stats;
    
    public CalculateStatsHandler(List<Stat> preliminaryStats)
    {
        _stats = preliminaryStats.Cast<BaseStat>().ToList();
        PreliminaryStats = _stats;
    }
    public void PreliminaryStatsCalculation(IReadOnlyList<SwitchInfo> switchInfo)
    {
        PreliminaryStats = _stats.ToList();
        for (int i = 0; i < switchInfo.Count; i++)
        {
            for (int j = 0; j < switchInfo[i].Stats.Count; j++)
            {
                PreliminaryStats[j] = new BaseStat(PreliminaryStats[j].NameText, PreliminaryStats[j].Value + switchInfo[i].Stats[j].Value);
            }
        }
    }
}