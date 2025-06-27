
using System.Collections.Generic;

public class SwitchNodeInitializer : MyNodeInitializer
{
    public SwitchNodeInitializer(List<Stat> stats) : base(stats) { }
    
    private void TryReInitCases(List<CaseForStats> casesForStats)
    {
        if (casesForStats != null)
        {
            List<CaseBaseStat> newStats = null;
            for (int i = 0; i < casesForStats.Count; i++)
            {
                newStats = CreateCaseBaseStat();

                if (casesForStats[i].CaseStats.Count > newStats.Count)
                {
                    for (int j = 0; j < newStats.Count; j++)
                    {
                        ReInitStat(ref newStats, j, casesForStats[i].CaseStats[j]);
                    }
                }
                else if (casesForStats[i].CaseStats.Count < newStats.Count)
                {
                    for (int j = 0; j < casesForStats[i].CaseStats.Count; j++)
                    {
                        ReInitStat(ref newStats, j, casesForStats[i].CaseStats[j]);
                    }
                }
                casesForStats[i] = new CaseForStats(newStats, casesForStats[i].Name);
            }
        }
    }
    public List<CaseBaseStat> CreateCaseBaseStat()
    {
        List<BaseStat> stats = _gameStatsHandler.GetGameBaseStatsForm();
        List<CaseBaseStat> caseStats = new List<CaseBaseStat>(stats.Count);
        for (int i = 0; i < stats.Count; i++)
        {
            caseStats.Add(new CaseBaseStat(stats[i].Name, stats[i].Value, 0, false));
        }
        return caseStats;
    }
    private void ReInitStat(ref List<CaseBaseStat> newStats, int index, CaseBaseStat oldStat)
    {
        if (oldStat.Name == newStats[index].Name)
        {
            newStats[index] = new CaseBaseStat(oldStat.Name,
                oldStat.Value,
                oldStat.IndexCurrentOperator,
                oldStat.IncludeKey);
        }
    }
}