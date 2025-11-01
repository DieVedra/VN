using System.Collections.Generic;

public class CalculateStatsHandler
{
    private const int _defaultValue = 0;
    public readonly List<CustomizationStat> PreliminaryStats;
    
    public CalculateStatsHandler(List<CustomizationStat> preliminaryStats)
    {
        PreliminaryStats = preliminaryStats;
    }
    public void PreliminaryStatsCalculation(IReadOnlyList<SwitchInfo> switchInfo)
    {
        SkipValues();
        List<CustomizationStat> stats;
        int count;
        for (int i = 0; i < switchInfo.Count; i++)
        {
            stats = switchInfo[i].Stats;
            count = stats.Count;
            for (int j = 0; j < count; j++)
            {
                PreliminaryStats[j].CustomizationStatValue = PreliminaryStats[j].CustomizationStatValue + stats[j].Value;
            }
        }
    }

    private void SkipValues()
    {
        foreach (var stat in PreliminaryStats)
        {
            stat.CustomizationStatValue = _defaultValue;
        }
    }
}