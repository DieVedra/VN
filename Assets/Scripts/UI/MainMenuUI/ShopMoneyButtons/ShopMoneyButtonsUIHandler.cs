using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ShopMoneyButtonsUIHandler
{

    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly Wallet _wallet;
    private readonly ShopMoneyPanelUIHandler _shopMoneyPanelUIHandler;
    private readonly Transform _parent;
    private ResourcePanelHandler _monetPanelHandler, _heartsPanelHandler;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    public bool IsInited { get; private set; }

    public ShopMoneyButtonsUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, Wallet wallet, ShopMoneyPanelUIHandler shopMoneyPanelUIHandler, Transform parent)
    {
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _wallet = wallet;
        _shopMoneyPanelUIHandler = shopMoneyPanelUIHandler;
        _parent = parent;
        IsInited = false;
    }

    public void Init(BlackFrameUIHandler darkeningBackgroundFrameUIHandler, Button shopButtonOnGameControlPanel, ResourcePanelHandler monetPanelHandler, ResourcePanelHandler heartsPanelHandler)
    {
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;

        if (IsInited == false)
        {
            // shopButtonOnGameControlPanel.Button.onClick.AddListener(() =>
            // {
            //     _shopMoneyPanelUIHandler.Show(_darkeningBackgroundFrameUIHandler, _parent, ShopMoneyMode.Monets).Forget();
            // });
            IsInited = true;
        }
    }
    public void Init(BlackFrameUIHandler darkeningBackgroundFrameUIHandler, ResourcePanelHandler monetPanelHandler, ResourcePanelHandler heartsPanelHandler)
    {
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        if (IsInited == false)
        {
            _monetPanelHandler = monetPanelHandler;
            _heartsPanelHandler = heartsPanelHandler;
            IsInited = true;
            SubscribeButtons();
        }
    }

    private void SubscribeButtons()
    {
        _monetPanelHandler.SubscribeButton(() =>
        {
            _shopMoneyPanelUIHandler.Show(ShopMoneyMode.Monets, ShowOperation, HideOperation).Forget();
        });
        _heartsPanelHandler.SubscribeButton(() =>
        {
            _shopMoneyPanelUIHandler.Show(ShopMoneyMode.Hearts, ShowOperation, HideOperation).Forget();
        });
    }

    private void ShowOperation()
    {
        _monetPanelHandler.SetParent(_shopMoneyPanelUIHandler.MonetIndicatorPanel);
        _heartsPanelHandler.SetParent(_shopMoneyPanelUIHandler.HeartsIndicatorPanel);
    }
    private void HideOperation()
    {
        _monetPanelHandler.SetParentDefault();
        _heartsPanelHandler.SetParentDefault();
        SubscribeButtons();
    }
}