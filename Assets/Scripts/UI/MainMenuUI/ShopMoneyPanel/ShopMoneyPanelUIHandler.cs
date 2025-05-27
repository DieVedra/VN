
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ShopMoneyPanelUIHandler
{
    private const int _monetIndex = 0;
    private readonly LocalizationString _monetButtonText = "Монеты";
    private readonly LocalizationString _heartsButtonText = "Сердца";
    private readonly ShopMoneyAssetLoader _shopMoneyAssetLoader;
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private readonly Transform _parent;
    private readonly ReactiveCommand _languageChanged;
    private readonly Wallet _wallet;
    private ShopMoneyPanelView _shopMoneyPanelView;

    public event Action OnHide;
    public bool PanelIsLoaded { get; private set; }
    public ShopMoneyPanelUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        Wallet wallet, Transform parent, ReactiveCommand languageChanged)
    {
        PanelIsLoaded = false;
        _shopMoneyAssetLoader = new ShopMoneyAssetLoader();
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _wallet = wallet;
        _parent = parent;
        _languageChanged = languageChanged;
    }

    public void Dispose()
    {
        if (_shopMoneyPanelView != null)
        {
            Addressables.ReleaseInstance(_shopMoneyPanelView.gameObject);
        }
    }

    public async UniTask Show(int index)
    {
        _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
        if (PanelIsLoaded == false)
        {
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            await LoadPanel();
        }
        else
        {
            _shopMoneyPanelView.transform.SetAsLastSibling();
        }
        _shopMoneyPanelView.MonetButtonText.text = _monetButtonText;
        _shopMoneyPanelView.HeartsButtonText.text = _heartsButtonText;
        
        
        InitPanel(index);
        _shopMoneyPanelView.gameObject.SetActive(true);
        _loadIndicatorUIHandler.StopIndicate();
    }

    private async UniTask Hide()
    {
        _shopMoneyPanelView.gameObject.SetActive(false);
        _shopMoneyPanelView.ButtonMonet.onClick.RemoveAllListeners();
        _shopMoneyPanelView.ButtonHearts.onClick.RemoveAllListeners();
        await _darkeningBackgroundFrameUIHandler.OpenTranslucent();
    }

    private async UniTask LoadPanel()
    {
        if (PanelIsLoaded == false)
        {
            _shopMoneyPanelView = await _shopMoneyAssetLoader.CreateShopMoneyPanel(_parent);
            _shopMoneyPanelView.transform.SetAsLastSibling();

            _shopMoneyPanelView.TextMoney.text = _wallet.Monets.ToString();
            
            _wallet.MonetsReactiveProperty.Subscribe(_ =>
            {
                _shopMoneyPanelView.TextMoney.text = _wallet.Monets.ToString();
            });
            _wallet.HeartsReactiveProperty.Subscribe(_ =>
            {
                _shopMoneyPanelView.TextHearts.text = _wallet.Hearts.ToString();
            });
            
            PanelIsLoaded = true;
        }
    }

    private void InitPanel(int index)
    {
        _shopMoneyPanelView.ExitButton.onClick.AddListener(() =>
        {
            OnHide?.Invoke();
            Hide().Forget();
            _shopMoneyPanelView.ExitButton.onClick.RemoveAllListeners();
        });

        _shopMoneyPanelView.ButtonMonet.onClick.AddListener(SwitchToMonetPanel);
        _shopMoneyPanelView.ButtonHearts.onClick.AddListener(SwitchToHeartsPanel);
        
        if (index == _monetIndex)
        {
            SwitchToMonetPanel();
        }
        else
        {
            SwitchToHeartsPanel();
        }
    }

    private void SwitchToMonetPanel()
    {
        _shopMoneyPanelView.ButtonMonet.gameObject.SetActive(false);
        _shopMoneyPanelView.ButtonHearts.gameObject.SetActive(true);
        _shopMoneyPanelView.MonetPanel.gameObject.SetActive(true);
        _shopMoneyPanelView.HeartsPanel.gameObject.SetActive(false);
        DisposeLots(_shopMoneyPanelView.MonetLots);
        DisposeLots(_shopMoneyPanelView.HeartLots);
        InitLots(_shopMoneyPanelView.MonetLots, AddMonet);
    }
    private void SwitchToHeartsPanel()
    {
        _shopMoneyPanelView.ButtonMonet.gameObject.SetActive(true);
        _shopMoneyPanelView.ButtonHearts.gameObject.SetActive(false);
        _shopMoneyPanelView.MonetPanel.gameObject.SetActive(false);
        _shopMoneyPanelView.HeartsPanel.gameObject.SetActive(true);
        DisposeLots(_shopMoneyPanelView.MonetLots);
        DisposeLots(_shopMoneyPanelView.HeartLots);
        InitLots(_shopMoneyPanelView.HeartLots, AddHearts);
    }

    private void InitLots(IReadOnlyList<LotView> lots, Action<int> operation)
    {
        for (int i = 0; i < lots.Count; i++)
        {
            SubscribeButton(lots[i].Button, operation, i);
        }
    }

    private void SubscribeButton(Button button, Action<int> operation, int i)
    {
        button.onClick.AddListener(()=>
        {
            operation?.Invoke(i);
        });
    }

    private void UnsubscribeButton(Button button)
    {
        button.onClick.RemoveAllListeners();
    }
    private void AddMonet(int index)
    {
        _wallet.AddCash(_shopMoneyPanelView.MonetLots[index].Resource, false);
    }

    private void AddHearts(int index)
    {
        _wallet.AddHearts(_shopMoneyPanelView.HeartLots[index].Resource, false);
    }
    private void DisposeLots(IReadOnlyList<LotView> lots)
    {
        for (int i = 0; i < lots.Count; i++)
        {
            UnsubscribeButton(lots[i].Button);
        }
    }
}