using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ShopMoneyPanelUIHandler : ILocalizable
{
    private const int _monetIndex = 0;
    private readonly LocalizationString _monetButtonText = "Монеты";
    private readonly LocalizationString _heartsButtonText = "Сердца";
    private readonly ShopMoneyAssetLoader _shopMoneyAssetLoader;
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly Wallet _wallet;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private ShopMoneyPanelView _shopMoneyPanelView;

    public event Action OnHide;
    public ReactiveCommand<bool> SwipeDetectorOff { get; private set; }

    public bool PanelIsLoaded { get; private set; }
    public ShopMoneyPanelUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, Wallet wallet, ReactiveCommand<bool> swipeDetectorOff)
    {
        PanelIsLoaded = false;
        _shopMoneyAssetLoader = new ShopMoneyAssetLoader();
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _wallet = wallet;
        SwipeDetectorOff = swipeDetectorOff;
    }

    public void Dispose()
    {
        if (_shopMoneyPanelView != null)
        {
            Addressables.ReleaseInstance(_shopMoneyPanelView.gameObject);
        }
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new LocalizationString[] {_monetButtonText, _heartsButtonText};
    }
    public async UniTask Show(BlackFrameUIHandler darkeningBackgroundFrameUIHandler, Transform parent, int index = 0)
    {
        SwipeDetectorOff?.Execute(true);
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
        if (PanelIsLoaded == false)
        {
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            await LoadPanel(parent);
            _loadIndicatorUIHandler.StopIndicate();
        }
        else
        {
            _shopMoneyPanelView.transform.SetAsLastSibling();
        }

        _shopMoneyPanelView.transform.parent.gameObject.SetActive(true);
        _shopMoneyPanelView.MonetButtonText.text = _monetButtonText;
        _shopMoneyPanelView.HeartsButtonText.text = _heartsButtonText;
        
        
        InitPanel(index);
        _shopMoneyPanelView.gameObject.SetActive(true);
    }

    private async UniTask Hide()
    {
        SwipeDetectorOff.Execute(false);
        _shopMoneyPanelView.gameObject.SetActive(false);
        _shopMoneyPanelView.ButtonMonet.onClick.RemoveAllListeners();
        _shopMoneyPanelView.ButtonHearts.onClick.RemoveAllListeners();
        await _darkeningBackgroundFrameUIHandler.OpenTranslucent();
        _shopMoneyPanelView.transform.parent.gameObject.SetActive(false);
    }

    private async UniTask LoadPanel(Transform parent)
    {
        if (PanelIsLoaded == false)
        {
            _shopMoneyPanelView = await _shopMoneyAssetLoader.CreateShopMoneyPanel(parent);
            _shopMoneyPanelView.transform.SetAsLastSibling();

            _shopMoneyPanelView.TextMoney.text = _wallet.GetMonetsCount.ToString();
            
            _wallet.MonetsCountChanged.Subscribe(_ =>
            {
                _shopMoneyPanelView.TextMoney.text = _.ToString();
            });
            _wallet.HeartsCountChanged.Subscribe(_ =>
            {
                _shopMoneyPanelView.TextHearts.text = _.ToString();
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