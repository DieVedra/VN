
public class ResourcesViewModeCalculator
{
    public ResourcesViewMode CalculateResourcesViewMode(params (int price, int additionalPrice)[] prices)
    {
        ResourcesViewMode resourcesViewMode1 = ResourcesViewMode.Hide;
        ResourcesViewMode resourcesViewMode2 = ResourcesViewMode.Hide;

        for (int i = 0; i < prices.Length; i++)
        {
            if (prices[i].price > 0)
            {
                resourcesViewMode1 = ResourcesViewMode.MonetMode;
            }
            if (prices[i].additionalPrice > 0)
            {
                resourcesViewMode2 = ResourcesViewMode.HeartsMode;
            }
        }

        if (resourcesViewMode1 == resourcesViewMode2)
        {
            return resourcesViewMode1;
        }
        if (resourcesViewMode1 == ResourcesViewMode.Hide && resourcesViewMode2 == ResourcesViewMode.HeartsMode)
        {
            resourcesViewMode1 = ResourcesViewMode.HeartsMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.Hide && resourcesViewMode2 == ResourcesViewMode.MonetMode)
        {
            resourcesViewMode1 = ResourcesViewMode.MonetMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.HeartsMode && resourcesViewMode2 == ResourcesViewMode.Hide)
        {
            resourcesViewMode1 = ResourcesViewMode.HeartsMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.HeartsMode && resourcesViewMode2 == ResourcesViewMode.MonetMode)
        {
            resourcesViewMode1 = ResourcesViewMode.MonetsAndHeartsMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.MonetMode && resourcesViewMode2 == ResourcesViewMode.Hide)
        {
            resourcesViewMode1 = ResourcesViewMode.MonetMode;
        }
        if (resourcesViewMode1 == ResourcesViewMode.MonetMode && resourcesViewMode2 == ResourcesViewMode.HeartsMode)
        {
            resourcesViewMode1 = ResourcesViewMode.MonetsAndHeartsMode;
        }
        return resourcesViewMode1;
    }
}