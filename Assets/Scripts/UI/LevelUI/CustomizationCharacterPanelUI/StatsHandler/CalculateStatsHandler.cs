
using System.Collections.Generic;
using System.Linq;

public class CalculateStatsHandler
{
    public List<BaseStat> PreliminaryStats;
    private List<BaseStat> _stats;
    
    public CalculateStatsHandler(List<BaseStat> preliminaryStats)
    {
        _stats = preliminaryStats;
        PreliminaryStats = preliminaryStats.ToList();
    }
    public void PreliminaryStatsCalculation(params SwitchInfo[] switchInfo)
    {
        PreliminaryStats = _stats.ToList();
        for (int i = 0; i < switchInfo.Length; i++)
        {
            for (int j = 0; j < switchInfo[i].Stats.Count; j++)
            {
                PreliminaryStats[j] = new BaseStat(PreliminaryStats[j].Name, PreliminaryStats[j].Value + switchInfo[i].Stats[j].Value);
            }
        }
    }
}