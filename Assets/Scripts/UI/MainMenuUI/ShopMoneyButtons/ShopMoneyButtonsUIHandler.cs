
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ShopMoneyButtonsUIHandler
{
    private const int _sublingIndexMonet = 2;
    private const int _sublingIndexHearts = 3;
    private const int _monetIndex = 0;
    private const int _heartsIndex = 1;

    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly Wallet _wallet;
    private readonly ShopMoneyPanelUIHandler _shopMoneyPanelUIHandler;
    private readonly Transform _parent;
    private ResourcePanelButtonView _monetPanel, _heartsPanel;
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

    public void Init(BlackFrameUIHandler darkeningBackgroundFrameUIHandler, ResourcePanelButtonView shopButtonOnGameControlPanel)
    {
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;

        if (IsInited == false)
        {
            shopButtonOnGameControlPanel.Button.onClick.AddListener(() =>
            {
                _shopMoneyPanelUIHandler.Show(_darkeningBackgroundFrameUIHandler, _parent).Forget();
            });
            IsInited = true;
        }
    }
    public void Init(BlackFrameUIHandler darkeningBackgroundFrameUIHandler, ResourcePanelButtonView monetPanel, ResourcePanelButtonView heartsPanel)
    {
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        if (IsInited == false)
        {
            _monetPanel = monetPanel;
            _heartsPanel = heartsPanel;
            _monetPanel.transform.SetSiblingIndex(_sublingIndexMonet);
            _heartsPanel.transform.SetSiblingIndex(_sublingIndexHearts);

            _wallet.MonetsReactiveProperty.Subscribe(_ =>
            {
                _monetPanel.Text.text = _wallet.Monets.ToString();
            });
            _wallet.HeartsReactiveProperty.Subscribe(_ =>
            {
                _heartsPanel.Text.text = _wallet.Hearts.ToString();
            });
            
            IsInited = true;
        }

        SubscribeButtonsAndSetResourcesIndicate();
    }

    private void SubscribeButtonsAndSetResourcesIndicate()
    {
        _monetPanel.Text.text = _wallet.Monets.ToString();
        _heartsPanel.Text.text = _wallet.Hearts.ToString();
        _monetPanel.gameObject.SetActive(true);
        _heartsPanel.gameObject.SetActive(true);
        _shopMoneyPanelUIHandler.OnHide -= SubscribeButtonsAndSetResourcesIndicate;
        
        _heartsPanel.Button.onClick.AddListener(()=>
        {
            _shopMoneyPanelUIHandler.OnHide += SubscribeButtonsAndSetResourcesIndicate;
            _shopMoneyPanelUIHandler.Show(_darkeningBackgroundFrameUIHandler, _parent, _heartsIndex).Forget();
            OffPanels();
        });
        _monetPanel.Button.onClick.AddListener(()=>
        {
            _shopMoneyPanelUIHandler.OnHide += SubscribeButtonsAndSetResourcesIndicate;
            _shopMoneyPanelUIHandler.Show(_darkeningBackgroundFrameUIHandler, _parent, _monetIndex).Forget();
            OffPanels();
        });
    }

    private void OffPanels()
    {
        _monetPanel.gameObject.SetActive(false);
        _heartsPanel.gameObject.SetActive(false);
        _monetPanel.Button.onClick.RemoveAllListeners();
        _heartsPanel.Button.onClick.RemoveAllListeners();
    }
}