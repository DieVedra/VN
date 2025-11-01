using System.Collections.Generic;

public struct CustomizationResult
{
    public readonly List<CustomizationStat> Stats;
    public readonly int MonetsToRemove;
    public readonly int HeartsToRemove;

    public CustomizationResult(List<CustomizationStat> stats, (int price, int additionalPrice) prices)
    {
        Stats = stats;
        MonetsToRemove = prices.price;
        HeartsToRemove = prices.additionalPrice;
    }
}