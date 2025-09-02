using System.Collections.Generic;

public class SwitchNodeInitializer : MyNodeInitializer
{
    public SwitchNodeInitializer(List<Stat> stats) : base(stats) { }

    public void TryReinitAllCases(List<CaseForStats> casesForStats)
    {
        if (casesForStats != null)
        {
            for (int i = 0; i < casesForStats.Count; i++)
            {
                var newStats = CreateCaseBaseStat();
                var oldStatsDictionary = casesForStats[i].CaseStats.ToDictionaryDistinct(caseStat => caseStat.NameText);
                var result = new List<CaseBaseStat>();
                for (int j = 0; j < newStats.Count; j++)
                {
                    if (oldStatsDictionary.TryGetValue(newStats[j].NameText, out CaseBaseStat oldStat))
                    {
                        result.Add(oldStat);
                    }
                    else
                    {
                        result.Add(newStats[j]);
                    }
                }
                casesForStats[i] = new CaseForStats(result, casesForStats[i].Name);
            }
        }
    }
    public List<CaseBaseStat> CreateCaseBaseStat()
    {
        List<BaseStat> stats = GameStatsHandler.GetGameBaseStatsForm();
        List<CaseBaseStat> caseStats = new List<CaseBaseStat>(stats.Count);
        for (int i = 0; i < stats.Count; i++)
        {
            caseStats.Add(new CaseBaseStat(stats[i].NameText, stats[i].Value, 0, false));
        }
        return caseStats;
    }
}