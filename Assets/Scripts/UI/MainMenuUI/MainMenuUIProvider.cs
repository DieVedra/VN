
using UnityEngine;

public class MainMenuUIProvider
{
    public readonly MainMenuUIView MainMenuUIView;
    private readonly Transform _parent;
    private readonly Wallet _wallet;
    private readonly AdvertisingHandler _advertisingHandler;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private LoadScreenUIHandler _loadScreenUIHandler;
    private PlayStoryPanelHandler _playStoryPanelHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private SettingsPanelButtonUIHandler _settingsPanelButtonUIHandler;
    private SettingsPanelUIHandler _settingsPanelUIHandler;
    private ShopMoneyPanelUIHandler _shopMoneyPanelUIHandler;
    private ShopMoneyButtonsUIHandler _shopMoneyButtonsUIHandler;
    private ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private BottomPanelUIHandler _bottomPanelUIHandler;
    public MainMenuUIProvider(MainMenuUIView  mainMenuUIView, Wallet wallet, AdvertisingHandler advertisingHandler)
    {
        MainMenuUIView = mainMenuUIView;
        _wallet = wallet;
        _advertisingHandler = advertisingHandler;
        _parent = MainMenuUIView.transform;
    }

    public BlackFrameUIHandler GetBlackFrameUIHandler()
    {
        if (_blackFrameUIHandler == null)
        {
            _blackFrameUIHandler = new BlackFrameUIHandler(MainMenuUIView.BlackFrameView);
        }
        return _blackFrameUIHandler;
    }

    public LoadScreenUIHandler GetLoadScreenHandler()
    {
        if (_loadScreenUIHandler == null)
        {
            _loadScreenUIHandler = new LoadScreenUIHandler();
        }
        return _loadScreenUIHandler;
    }

    public PlayStoryPanelHandler GetPlayStoryPanelHandler()
    {
        if (_playStoryPanelHandler == null)
        {
            _playStoryPanelHandler = new PlayStoryPanelHandler(_parent,GetBlackFrameUIHandler());
        }
        return _playStoryPanelHandler;
    }

    public LoadIndicatorUIHandler GetLoadIndicatorUIHandler()
    {
        if (_loadIndicatorUIHandler == null)
        {
            _loadIndicatorUIHandler = new LoadIndicatorUIHandler();
        }
        return _loadIndicatorUIHandler;
    }

    public SettingsPanelButtonUIHandler GetSettingsPanelButtonUIHandler()
    {
        if (_settingsPanelButtonUIHandler == null)
        {
            _settingsPanelButtonUIHandler = new SettingsPanelButtonUIHandler(_parent,
                GetSettingsPanelUIHandler(), GetBlackFrameUIHandler(), GetLoadIndicatorUIHandler());
        }
        return _settingsPanelButtonUIHandler;
    }

    public SettingsPanelUIHandler GetSettingsPanelUIHandler()
    {
        if (_settingsPanelUIHandler == null)
        {
            _settingsPanelUIHandler = new SettingsPanelUIHandler();
        }
        return _settingsPanelUIHandler;
    }

    public ShopMoneyPanelUIHandler GetShopMoneyPanelUIHandler()
    {
        if (_shopMoneyPanelUIHandler == null)
        {
            _shopMoneyPanelUIHandler = new ShopMoneyPanelUIHandler(GetLoadIndicatorUIHandler(), GetBlackFrameUIHandler(), _wallet, _parent);
        }
        return _shopMoneyPanelUIHandler;
    }

    public ShopMoneyButtonsUIHandler GetShopMoneyButtonsUIHandler()
    {
        if (_shopMoneyButtonsUIHandler == null)
        {
            _shopMoneyButtonsUIHandler = new ShopMoneyButtonsUIHandler(_wallet, GetShopMoneyPanelUIHandler(), _parent);
        }

        return _shopMoneyButtonsUIHandler;
    }

    public ConfirmedPanelUIHandler GetConfirmedPanelUIHandler()
    {
        if (_confirmedPanelUIHandler == null)
        {
            _confirmedPanelUIHandler = new ConfirmedPanelUIHandler(GetLoadIndicatorUIHandler(), GetBlackFrameUIHandler(), _parent);
        }

        return _confirmedPanelUIHandler;
    }

    public BottomPanelUIHandler GetBottomPanelUIHandler()
    {
        if (_bottomPanelUIHandler == null)
        {
            _bottomPanelUIHandler = new BottomPanelUIHandler(GetConfirmedPanelUIHandler(),
                new AdvertisingButtonUIHandler(GetLoadIndicatorUIHandler(), GetBlackFrameUIHandler(), _advertisingHandler, _parent),
                _parent);
        }

        return _bottomPanelUIHandler;
    }
}