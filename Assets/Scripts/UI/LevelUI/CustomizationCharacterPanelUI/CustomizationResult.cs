
using System.Collections.Generic;

public class CustomizationResult
{
    public readonly List<BaseStat> Stats;
    public readonly int PreliminaryBalance;

    public CustomizationResult(List<BaseStat> stats, int preliminaryBalance)
    {
        Stats = stats;
        PreliminaryBalance = preliminaryBalance;
    }
}