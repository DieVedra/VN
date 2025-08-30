

using System.Collections.Generic;

public class MainMenuUIProvider : ILocalizable
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

    private List<LocalizationString> _localizableContent;
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

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        if (_localizableContent == null)
        {
            _localizableContent = new List<LocalizationString>();
            _localizableContent.AddRange(LoadScreenUIHandler.GetLocalizableContent());
            _localizableContent.AddRange(LoadIndicatorUIHandler.GetLocalizableContent());
            _localizableContent.AddRange(PlayStoryPanelHandler.GetLocalizableContent());
            _localizableContent.AddRange(SettingsPanelUIHandler.GetLocalizableContent());
            _localizableContent.AddRange(ShopMoneyPanelUIHandler.GetLocalizableContent());
            _localizableContent.AddRange(BottomPanelUIHandler.GetLocalizableContent());
            _localizableContent.AddRange(MyScrollHandler.GetLocalizableContent());
        }

        return _localizableContent;
    }
}