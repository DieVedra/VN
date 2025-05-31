

public class MainMenuUIProvider
{
    public readonly LoadScreenUIHandler LoadScreenUIHandler;
    public readonly GlobalUIHandler _globalUIHandler;

    public readonly BlackFrameUIHandler DarkeningBackgroundFrameUIHandler;
    public readonly BlackFrameUIHandler BlackFrameUIHandler;
    public readonly LoadIndicatorUIHandler LoadIndicatorUIHandler;
    
    public readonly PlayStoryPanelHandler PlayStoryPanelHandler;
    public readonly SettingsPanelButtonUIHandler SettingsButtonUIHandler;
    
    public readonly SettingsPanelUIHandler SettingsPanelUIHandler;
    public readonly ShopMoneyPanelUIHandler ShopMoneyPanelUIHandler;
    
    public readonly ShopMoneyButtonsUIHandler ShopButtonsUIHandler;
    public readonly ConfirmedPanelUIHandler ConfirmedPanelUIHandler;
    
    public readonly BottomPanelUIHandler BottomPanelUIHandler;
    public readonly MyScrollHandler MyScrollHandler;
    public MainMenuUIProvider(BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        PlayStoryPanelHandler playStoryPanelHandler, SettingsPanelButtonUIHandler settingsButtonUIHandler,
        SettingsPanelUIHandler settingsPanelUIHandler, ShopMoneyPanelUIHandler shopMoneyPanelUIHandler,
        ShopMoneyButtonsUIHandler shopButtonsUIHandler, ConfirmedPanelUIHandler confirmedPanelUIHandler,
        GlobalUIHandler globalUIHandler, BottomPanelUIHandler bottomPanelUIHandler, MyScrollHandler myScrollHandler)
    {
        BlackFrameUIHandler = globalUIHandler.BlackFrameUIHandler;
        DarkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        LoadIndicatorUIHandler = globalUIHandler.LoadIndicatorUIHandler;
        PlayStoryPanelHandler = playStoryPanelHandler;
        SettingsButtonUIHandler = settingsButtonUIHandler;
        SettingsPanelUIHandler = settingsPanelUIHandler;
        ShopMoneyPanelUIHandler = shopMoneyPanelUIHandler;
        ShopButtonsUIHandler = shopButtonsUIHandler;
        ConfirmedPanelUIHandler = confirmedPanelUIHandler;
        BottomPanelUIHandler = bottomPanelUIHandler;
        _globalUIHandler = globalUIHandler;
        LoadScreenUIHandler = globalUIHandler.LoadScreenUIHandler;
        MyScrollHandler = myScrollHandler;
    }

    public void Dispose()
    {
        DarkeningBackgroundFrameUIHandler.Dispose();
        PlayStoryPanelHandler.Dispose();
        ConfirmedPanelUIHandler.Dispose();
        BottomPanelUIHandler.Dispose();
        BottomPanelUIHandler.Dispose();
        MyScrollHandler.Dispose();
    }
}