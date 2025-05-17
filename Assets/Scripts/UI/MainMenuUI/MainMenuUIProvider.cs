

public class MainMenuUIProvider
{
    public readonly LoadScreenUIHandler LoadScreenUIHandler;

    public readonly BlackFrameUIHandler DarkeningBackgroundFrameUIHandler;
    public readonly BlackFrameUIHandler BlackFrameUIHandler;
    public readonly LoadIndicatorUIHandler LoadIndicatorUIHandler;
    
    public readonly PlayStoryPanelHandler PlayStoryPanelHandler;
    public readonly SettingsPanelButtonUIHandler SettingsPanelButtonUIHandler;
    
    public readonly SettingsPanelUIHandler SettingsPanelUIHandler;
    public readonly ShopMoneyPanelUIHandler ShopMoneyPanelUIHandler;
    
    public readonly ShopMoneyButtonsUIHandler ShopMoneyButtonsUIHandler;
    public readonly ConfirmedPanelUIHandler ConfirmedPanelUIHandler;
    
    public readonly BottomPanelUIHandler BottomPanelUIHandler;
    public MainMenuUIProvider(BlackFrameUIHandler blackFrameUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        LoadIndicatorUIHandler loadIndicatorUIHandler,
        PlayStoryPanelHandler playStoryPanelHandler, SettingsPanelButtonUIHandler settingsPanelButtonUIHandler,
        SettingsPanelUIHandler settingsPanelUIHandler, ShopMoneyPanelUIHandler shopMoneyPanelUIHandler,
        ShopMoneyButtonsUIHandler shopMoneyButtonsUIHandler, ConfirmedPanelUIHandler confirmedPanelUIHandler,
        LoadScreenUIHandler loadScreenUIHandler, BottomPanelUIHandler bottomPanelUIHandler)
    {
        BlackFrameUIHandler = blackFrameUIHandler;
        DarkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        LoadIndicatorUIHandler = loadIndicatorUIHandler;
        PlayStoryPanelHandler = playStoryPanelHandler;
        SettingsPanelButtonUIHandler = settingsPanelButtonUIHandler;
        SettingsPanelUIHandler = settingsPanelUIHandler;
        ShopMoneyPanelUIHandler = shopMoneyPanelUIHandler;
        ShopMoneyButtonsUIHandler = shopMoneyButtonsUIHandler;
        ConfirmedPanelUIHandler = confirmedPanelUIHandler;
        BottomPanelUIHandler = bottomPanelUIHandler;
        LoadScreenUIHandler = loadScreenUIHandler;
    }

    public void Dispose()
    {
        BlackFrameUIHandler.Dispose();
        DarkeningBackgroundFrameUIHandler.Dispose();
        LoadIndicatorUIHandler.Dispose();
        PlayStoryPanelHandler.Dispose();
        // SettingsPanelButtonUIHandler.Dispose();
        SettingsPanelUIHandler.Dispose();
        ShopMoneyPanelUIHandler.Dispose();
        // ShopMoneyButtonsUIHandler.Dispose();
        ConfirmedPanelUIHandler.Dispose();
        BottomPanelUIHandler.Dispose();
        BottomPanelUIHandler.Dispose();
    }
}