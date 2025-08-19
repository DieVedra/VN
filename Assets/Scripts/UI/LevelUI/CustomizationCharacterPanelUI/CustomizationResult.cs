using System.Collections.Generic;

public class CustomizationResult
{
    public readonly List<BaseStat> Stats;
    public readonly int MonetsToRemove;
    public readonly int HeartsToRemove;

    public CustomizationResult(List<BaseStat> stats, (int price, int additionalPrice) prices)
    {
        Stats = stats;
        MonetsToRemove = prices.price;
        HeartsToRemove = prices.additionalPrice;
    }
}