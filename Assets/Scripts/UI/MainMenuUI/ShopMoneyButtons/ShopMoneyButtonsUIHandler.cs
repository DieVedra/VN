
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class ShopMoneyButtonsUIHandler
{
    private readonly int _sublingIndexMonet = 2;
    private readonly int _sublingIndexHearts = 3;

    private readonly Wallet _wallet;
    private readonly ShopMoneyPanelUIHandler _shopMoneyPanelUIHandler;
    private readonly Transform _parent;
    private readonly int _monetIndex = 0;
    private readonly int _heartsIndex = 1;
    private ShopMoneyButtonsAssetProvider _shopMoneyButtonsAssetProvider;
    private ResourcePanelButtonView _monetPanel, _heartsPanel;
    public bool AssetIsLoaded { get; private set; }

    public ShopMoneyButtonsUIHandler(Wallet wallet, ShopMoneyPanelUIHandler shopMoneyPanelUIHandler, Transform parent)
    {
        _wallet = wallet;
        _shopMoneyPanelUIHandler = shopMoneyPanelUIHandler;
        _parent = parent;
        _shopMoneyButtonsAssetProvider = new ShopMoneyButtonsAssetProvider();
        AssetIsLoaded = false;
    }

    public async UniTask Init()
    {
        if (AssetIsLoaded == false)
        {
            _monetPanel = await _shopMoneyButtonsAssetProvider.LoadAssetMonet(_parent);
            _heartsPanel = await _shopMoneyButtonsAssetProvider.LoadAssetHearth(_parent);
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
            
            AssetIsLoaded = true;
        }
    }
    public void SubscribeButtonsAndSetResourcesIndicate()
    {
        _monetPanel.Text.text = _wallet.Monets.ToString();
        _heartsPanel.Text.text = _wallet.Hearts.ToString();
        _monetPanel.gameObject.SetActive(true);
        _heartsPanel.gameObject.SetActive(true);
        _shopMoneyPanelUIHandler.OnHide -= SubscribeButtonsAndSetResourcesIndicate;
        
        _heartsPanel.Button.onClick.AddListener(()=>
        {
            _shopMoneyPanelUIHandler.OnHide += SubscribeButtonsAndSetResourcesIndicate;
            _shopMoneyPanelUIHandler.Show(_heartsIndex).Forget();
            OffPanels();
        });
        _monetPanel.Button.onClick.AddListener(()=>
        {
            _shopMoneyPanelUIHandler.OnHide += SubscribeButtonsAndSetResourcesIndicate;
            _shopMoneyPanelUIHandler.Show(_monetIndex).Forget();
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