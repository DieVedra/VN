﻿
using System.Collections.Generic;

public class CustomizationResult
{
    public readonly List<BaseStat> Stats;
    public readonly int PreliminaryMonet;
    public readonly int PreliminaryHearts;

    public CustomizationResult(List<BaseStat> stats, int preliminaryMonet, int preliminaryHearts)
    {
        Stats = stats;
        PreliminaryMonet = preliminaryMonet;
        PreliminaryHearts = preliminaryHearts;
    }

    public int GetRemovedValueMonets(int monets)
    {
        return monets -= PreliminaryMonet;
    }
    public int GetRemovedValueHearts(int hearts)
    {
        return hearts -= PreliminaryHearts;
    }
}