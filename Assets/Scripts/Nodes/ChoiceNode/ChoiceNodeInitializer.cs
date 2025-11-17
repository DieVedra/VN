
using System.Collections.Generic;

public class ChoiceNodeInitializer : MyNodeInitializer
{
    public ChoiceNodeInitializer(List<Stat> stats) : base(stats) { }
    
    public void TryInitStats(ref List<BaseStat> oldBaseStatsChoice)
    {
        if (oldBaseStatsChoice == null || oldBaseStatsChoice.Count == 0)
        {
            oldBaseStatsChoice = GetBaseStatsChoice();
        }
        else
        {
            oldBaseStatsChoice = GameStatsHandlerNodeInitializer.ReinitBaseStats(oldBaseStatsChoice);
        }
    }

    public List<BaseStat> GetBaseStatsChoice()
    {
        return GameStatsHandlerNodeInitializer.GetGameBaseStatsForm();
    }
}