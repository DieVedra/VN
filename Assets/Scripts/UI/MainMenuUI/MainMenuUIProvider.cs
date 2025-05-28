

public class MainMenuUIProvider
{
    public readonly LoadScreenUIHandler LoadScreenUIHandler;

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
    public MainMenuUIProvider(BlackFrameUIHandler blackFrameUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        LoadIndicatorUIHandler loadIndicatorUIHandler,
        PlayStoryPanelHandler playStoryPanelHandler, SettingsPanelButtonUIHandler settingsButtonUIHandler,
        SettingsPanelUIHandler settingsPanelUIHandler, ShopMoneyPanelUIHandler shopMoneyPanelUIHandler,
        ShopMoneyButtonsUIHandler shopButtonsUIHandler, ConfirmedPanelUIHandler confirmedPanelUIHandler,
        LoadScreenUIHandler loadScreenUIHandler, BottomPanelUIHandler bottomPanelUIHandler, MyScrollHandler myScrollHandler)
    {
        BlackFrameUIHandler = blackFrameUIHandler;
        DarkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        LoadIndicatorUIHandler = loadIndicatorUIHandler;
        PlayStoryPanelHandler = playStoryPanelHandler;
        SettingsButtonUIHandler = settingsButtonUIHandler;
        SettingsPanelUIHandler = settingsPanelUIHandler;
        ShopMoneyPanelUIHandler = shopMoneyPanelUIHandler;
        ShopButtonsUIHandler = shopButtonsUIHandler;
        ConfirmedPanelUIHandler = confirmedPanelUIHandler;
        BottomPanelUIHandler = bottomPanelUIHandler;
        LoadScreenUIHandler = loadScreenUIHandler;
        MyScrollHandler = myScrollHandler;
    }

    public void Dispose()
    {
        DarkeningBackgroundFrameUIHandler.Dispose();
        PlayStoryPanelHandler.Dispose();
        // SettingsButtonUIHandler.Dispose();
        SettingsPanelUIHandler.Dispose();
        ShopMoneyPanelUIHandler.Dispose();
        // ShopButtonsUIHandler.Dispose();
        ConfirmedPanelUIHandler.Dispose();
        BottomPanelUIHandler.Dispose();
        BottomPanelUIHandler.Dispose();
        MyScrollHandler.Dispose();
    }
}