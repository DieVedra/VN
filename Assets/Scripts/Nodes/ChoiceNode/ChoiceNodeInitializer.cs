
using System.Collections.Generic;

public class ChoiceNodeInitializer : MyNodeInitializer
{
    public ChoiceNodeInitializer(List<Stat> stats) : base(stats) { }
    
    public void TryInitStats(ref List<BaseStat> oldBaseStatsChoice)
    {
        if (oldBaseStatsChoice == null || oldBaseStatsChoice.Count == 0)
        {
            oldBaseStatsChoice = _gameStatsHandler.GetGameBaseStatsForm();
        }
        else
        {
            oldBaseStatsChoice = _gameStatsHandler.ReinitBaseStats(oldBaseStatsChoice);
        }
    }
}