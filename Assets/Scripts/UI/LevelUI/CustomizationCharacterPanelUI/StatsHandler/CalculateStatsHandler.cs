﻿
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CalculateStatsHandler
{
    public List<BaseStat> PreliminaryStats;
    private List<BaseStat> _stats;
    
    public CalculateStatsHandler(List<Stat> preliminaryStats)
    {
        _stats = preliminaryStats.Cast<BaseStat>().ToList();
        PreliminaryStats = _stats;
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