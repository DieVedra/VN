using UniRx;

public class CustomizationPanelResourceAndPricePanelBroker
{
    private readonly ReactiveProperty<ResourcesViewMode> _resourcesViewModeReactiveProperty = new ReactiveProperty<ResourcesViewMode>();
    
    public ReactiveProperty<ResourcesViewMode> ResourcesViewModeReactiveProperty => _resourcesViewModeReactiveProperty;
    
    public ResourcesViewMode CurrentResourcesViewMode => _resourcesViewModeReactiveProperty.Value;


    public void CalculateAndSetMode(SwitchInfo[] switchInfos)
    {
        int allMonetPrice = 0;
        int allHeartsPriceAdditional = 0;
        foreach (var switchInfo in switchInfos)
        {
            allMonetPrice += switchInfo.Price;
            allHeartsPriceAdditional += switchInfo.AdditionalPrice;
        }
        
        if (allMonetPrice > 0 && allHeartsPriceAdditional > 0)
        {
            _resourcesViewModeReactiveProperty.Value = ResourcesViewMode.MonetsAndHeartsMode;
        }
        else if (allMonetPrice > 0)
        {
            _resourcesViewModeReactiveProperty.Value = ResourcesViewMode.MonetMode;
        }
        else if(allHeartsPriceAdditional > 0)
        {
            _resourcesViewModeReactiveProperty.Value = ResourcesViewMode.HeartsMode;
        }
        else
        {
            _resourcesViewModeReactiveProperty.Value = ResourcesViewMode.Hide;
        }
    }
}