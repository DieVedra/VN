using System.Collections.Generic;

public class ChoiceNodeInitializer : MyNodeInitializer
{
    public ChoiceNodeInitializer(List<Stat> stats) : base(stats) { }
    
    public void TryInitReInitStatsInCases(List<ChoiceCase> oldChoiceCase)
    {
        for (int i = 0; i < oldChoiceCase.Count; i++)
        {
            var newStats = GameStatsHandlerNodeInitializer.ReinitBaseStats(oldChoiceCase[i].BaseStatsChoiceIReadOnly);
            var newCase = new ChoiceCase(newStats, oldChoiceCase[i].GetLocalizationString(), oldChoiceCase[i].ChoicePrice, oldChoiceCase[i].ChoiceAdditionaryPrice,
                oldChoiceCase[i].ShowStatsChoiceKey, oldChoiceCase[i].ShowNotificationChoice);
            newCase.InitLocalizationString();
            oldChoiceCase[i] = newCase;
        }
    }
    public List<BaseStat> GetBaseStatsChoice()
    {
        return GameStatsHandlerNodeInitializer.GetGameBaseStatsForm();
    }
}