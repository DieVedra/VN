using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class ShopMoneyPanelUIHandler : ILocalizable
{
    private readonly LocalizationString _monetButtonText = "Монеты";
    private readonly LocalizationString _heartsButtonText = "Сердца";
    private readonly ShopMoneyAssetLoader _shopMoneyAssetLoader;
    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly Wallet _wallet;
    private Transform _parent;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private ShopMoneyPanelView _shopMoneyPanelView;
    private GlobalCanvasCloser _globalCanvasCloser;
    private AdvertisingButtonUIHandler _advertisingButtonUIHandler;
    private ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private ShopMoneyMode _lastShopMode;
    private Action _hideOperation;
    private bool _isInited;

    public ReactiveCommand<bool> SwipeDetectorOff { get; private set; }

    public RectTransform MonetIndicatorPanel => _shopMoneyPanelView.MonetIndicatorPanel;
    public RectTransform HeartsIndicatorPanel => _shopMoneyPanelView.HeartsIndicatorPanel;
    public bool PanelIsLoaded { get; private set; }
    public ShopMoneyPanelUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        Wallet wallet, ReactiveCommand<bool> swipeDetectorOff)
    {
        PanelIsLoaded = false;
        _shopMoneyAssetLoader = new ShopMoneyAssetLoader();
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _wallet = wallet;
        SwipeDetectorOff = swipeDetectorOff;
        _lastShopMode = ShopMoneyMode.Monets;
    }

    public void Init(Transform parent, GlobalCanvasCloser globalCanvasCloser,
        AdvertisingButtonUIHandler advertisingButtonUIHandler, ConfirmedPanelUIHandler confirmedPanelUIHandler)
    {
        if (_isInited == false)
        {
            _isInited = true;
            _parent = parent;
            _globalCanvasCloser = globalCanvasCloser;
            _advertisingButtonUIHandler = advertisingButtonUIHandler;
            _confirmedPanelUIHandler = confirmedPanelUIHandler;
        }
    }
    public void Shutdown()
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
    public async UniTask Show(ShopMoneyMode mode, Action showOperation, Action hideOperation)
    {
        SwipeDetectorOff?.Execute(true);
        _darkeningBackgroundFrameUIHandler.CloseTranslucent(false).Forget();
        if (PanelIsLoaded == false)
        {
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            await LoadPanel(_parent);
            _loadIndicatorUIHandler.StopIndicate();
        }
        _shopMoneyPanelView.transform.SetAsLastSibling();
        _hideOperation = hideOperation;
        _globalCanvasCloser.TryEnable();
        _shopMoneyPanelView.MonetButtonText.text = _monetButtonText;
        _shopMoneyPanelView.HeartsButtonText.text = _heartsButtonText;


        InitPanel(mode);
        _shopMoneyPanelView.gameObject.SetActive(true);
        showOperation.Invoke();
    }

    private async UniTask Hide()
    {
        SwipeDetectorOff.Execute(false);
        _shopMoneyPanelView.gameObject.SetActive(false);
        _shopMoneyPanelView.ButtonMonet.onClick.RemoveAllListeners();
        _shopMoneyPanelView.ButtonHearts.onClick.RemoveAllListeners();
        _hideOperation?.Invoke();
        await _darkeningBackgroundFrameUIHandler.OpenTranslucent(() =>
        {
            _globalCanvasCloser.TryDisable();
        });
        _shopMoneyPanelView.gameObject.SetActive(false);
    }

    private async UniTask LoadPanel(Transform parent)
    {
        _shopMoneyPanelView = await _shopMoneyAssetLoader.CreateShopMoneyPanel(parent);
        PanelIsLoaded = true;
    }

    private void InitPanel(ShopMoneyMode mode)
    {
        SubscribeAdvertisingButton();
        _shopMoneyPanelView.ExitButton.onClick.AddListener(() =>
        {
            _shopMoneyPanelView.ExitButton.onClick.RemoveAllListeners();
            _shopMoneyPanelView.ButtonMonet.onClick.RemoveAllListeners();
            _shopMoneyPanelView.ButtonHearts.onClick.RemoveAllListeners();
            _shopMoneyPanelView.AdvertisingButton.onClick.RemoveAllListeners();
            Hide().Forget();
        });
        _shopMoneyPanelView.ButtonMonet.onClick.AddListener(SwitchToMonetPanel);
        _shopMoneyPanelView.ButtonHearts.onClick.AddListener(SwitchToHeartsPanel);
        if (TrySwitchMode(mode) == false)
        {
            TrySwitchMode(_lastShopMode);
        }
    }

    private void SubscribeAdvertisingButton()
    {
        _shopMoneyPanelView.AdvertisingButton.onClick.AddListener(() =>
        {
            _shopMoneyPanelView.AdvertisingButton.onClick.RemoveAllListeners();
            _confirmedPanelUIHandler.Show(
                _advertisingButtonUIHandler.LabelTextToConfirmedPanel,
                _advertisingButtonUIHandler.TranscriptionTextToConfirmedPanel,
                _advertisingButtonUIHandler.ButtonText, AdvertisingButtonUIHandler.HeightPanel,
                AdvertisingButtonUIHandler.FontSizeValue,
                () => { _advertisingButtonUIHandler.Show(SubscribeAdvertisingButton).Forget(); },
                SubscribeAdvertisingButton,
                true).Forget();
        });
    }

    private bool TrySwitchMode(ShopMoneyMode mode)
    {
        bool result = false;
        switch (mode)
        {
            case ShopMoneyMode.Monets:
                SwitchToMonetPanel();
                result = true;
                break;
            case ShopMoneyMode.Hearts:
                SwitchToHeartsPanel();
                result = true;
                break;
        }
        return result;
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
        _lastShopMode = ShopMoneyMode.Monets;
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
        _lastShopMode = ShopMoneyMode.Hearts;
    }

    private void InitLots(IReadOnlyList<LotView> lots, Action<int> operation)
    {
        for (int i = 0; i < lots.Count; i++)
        {
            SubscribeLotButton(lots[i].Button, operation, i);
        }
    }

    private void SubscribeLotButton(Button button, Action<int> operation, int i)
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