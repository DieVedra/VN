using System.Collections.Generic;
using UnityEngine;

public class SwitchNodeInitializer : MyNodeInitializer
{
    public SwitchNodeInitializer(List<Stat> stats) : base(stats) { }

    public void TryReinitAllCases(List<CaseForStats> casesForStats)
    {
        if (casesForStats != null)
        {
            Dictionary<string, CaseBaseStat> oldStatsDictionary;
            IReadOnlyList<AdditionalCaseStats> additionalCaseStats;
            List<CaseBaseStat> newStats;
            AdditionalCaseStats stat;
            for (int i = 0; i < casesForStats.Count; i++)
            {
                newStats = GameStatsHandlerNodeInitializer.CreateCaseBaseStatForm();
                oldStatsDictionary = casesForStats[i].CaseStats.ToDictionaryDistinct(caseStat => caseStat.NameText);
                for (int j = 0; j < newStats.Count; j++)
                {
                    if (oldStatsDictionary.TryGetValue(newStats[j].NameText, out CaseBaseStat oldStat))
                    {
                        newStats[j] = oldStat;
                    }
                }

                additionalCaseStats = casesForStats[i].AdditionalCaseStats;
                for (int j = additionalCaseStats.Count - 1; j >= 0; j--)
                {
                    stat = additionalCaseStats[j];
                    if ((GameStatsHandlerNodeInitializer.StatsDictionary.ContainsKey(stat.Stat1Key) && GameStatsHandlerNodeInitializer.StatsDictionary.ContainsKey(stat.Stat2Key)) == false)
                    {
                        casesForStats[i].RemoveAdditionalElement(j);
                    }
                }
                casesForStats[i] = new CaseForStats(newStats, casesForStats[i].Name, casesForStats[i].FoldoutKey, additionalCaseStats);
            }
        }
    }
}