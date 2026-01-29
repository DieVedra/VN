using Cysharp.Threading.Tasks;
using UnityEngine.UI;

public class ShopMoneyButtonsUIHandler
{
    private readonly ShopMoneyPanelUIHandler _shopMoneyPanelUIHandler;
    private ResourcePanelHandler _monetPanelHandler, _heartsPanelHandler;

    private Button _shopButtonOnGameControlPanel;
    public bool IsInited { get; private set; }

    public ShopMoneyButtonsUIHandler(ShopMoneyPanelUIHandler shopMoneyPanelUIHandler)
    {
        _shopMoneyPanelUIHandler = shopMoneyPanelUIHandler;
        IsInited = false;
    }

    public void InitFromGameControlPanel(Button shopButtonOnGameControlPanel,
        ResourcePanelHandler monetPanelHandler, ResourcePanelHandler heartsPanelHandler)
    {
        if (IsInited == false)
        {
            IsInited = true;
            _monetPanelHandler = monetPanelHandler;
            _heartsPanelHandler = heartsPanelHandler;
            _shopButtonOnGameControlPanel = shopButtonOnGameControlPanel;
            SubscribeButtons();
        }
    }
    public void InitFromAppStarter(ResourcePanelHandler monetPanelHandler, ResourcePanelHandler heartsPanelHandler)
    {
        if (IsInited == false)
        {
            IsInited = true;
            _monetPanelHandler = monetPanelHandler;
            _heartsPanelHandler = heartsPanelHandler;
            SubscribeButtons();
        }
    }

    public void Shutdown()
    {
        _monetPanelHandler.Shutdown();
        _heartsPanelHandler.Shutdown();
        IsInited = false;
    }
    private void SubscribeButtons()
    {
        _monetPanelHandler.SubscribeButton(() =>
        {
            _shopMoneyPanelUIHandler.Show(ShopMoneyMode.Monets, ShowShopMoneyPanelOperation, HideShopMoneyPanelOperation).Forget();
        });
        _heartsPanelHandler.SubscribeButton(() =>
        {
            _shopMoneyPanelUIHandler.Show(ShopMoneyMode.Hearts, ShowShopMoneyPanelOperation, HideShopMoneyPanelOperation).Forget();
        });
        _shopButtonOnGameControlPanel?.onClick.AddListener(() =>
        {
            _shopMoneyPanelUIHandler.Show(ShopMoneyMode.LastMode, ShowShopMoneyPanelOperation, HideShopMoneyPanelOperation).Forget();
            _shopButtonOnGameControlPanel.onClick.RemoveAllListeners();
        });
    }
    private void ShowShopMoneyPanelOperation()
    {
        _monetPanelHandler.SetParent(_shopMoneyPanelUIHandler.MonetIndicatorPanel);
        _heartsPanelHandler.SetParent(_shopMoneyPanelUIHandler.HeartsIndicatorPanel);
    }
    private void HideShopMoneyPanelOperation()
    {
        _monetPanelHandler.SetParentDefault();
        _heartsPanelHandler.SetParentDefault();
        SubscribeButtons();
    }
}