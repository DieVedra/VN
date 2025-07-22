using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class PriceViewHandler
{
    private readonly float _duration;
    private readonly PriceViewHandlerValues _values;
    private readonly PriceUIView _priceUIView;
    private readonly CalculateBalanceHandler _calculateBalanceHandler;
    private readonly ResourcesViewMode _resourcesViewMode;

    private CancellationTokenSource _cancellationTokenSource;

    private bool _panelIsShowed;
    public bool PanelIsShowed => _panelIsShowed;
    public CalculateBalanceHandler CalculateBalanceHandler => _calculateBalanceHandler;
    public PriceViewHandler(PriceUIView priceUIView,
        CalculateBalanceHandler calculateBalanceHandler,
        ResourcesViewMode resourcesViewMode, float duration)
    {
        _priceUIView = priceUIView;
        _calculateBalanceHandler = calculateBalanceHandler;
        _resourcesViewMode = resourcesViewMode;
        _panelIsShowed = false;
        _duration = duration;
        _values = new PriceViewHandlerValues();
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }
    public async UniTask HideToShowAnim(int price, int additionalPrice)
    {
        await HideAnim();
        await ShowAnim(price, additionalPrice);
    }
    public async UniTask HideAnim()
    {
        if (_panelIsShowed == true)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await _priceUIView.CanvasGroup.DOFade(PriceViewHandlerValues.MinValue, _duration)
                .WithCancellation(_cancellationTokenSource.Token);
            PanelOff();
        }
    }

    public async UniTask ShowAnim(int price, int additionalPrice)
    {
        _priceUIView.CanvasGroup.alpha = PriceViewHandlerValues.MinValue;
        PanelOn(price, additionalPrice);
        _cancellationTokenSource = new CancellationTokenSource();
        await _priceUIView.CanvasGroup.DOFade(PriceViewHandlerValues.MaxValue, _duration)
            .WithCancellation(_cancellationTokenSource.Token);
    }
    public void Show(int price, int additionalPrice)
    {
        _priceUIView.CanvasGroup.alpha = PriceViewHandlerValues.MaxValue;
        PanelOn(price, additionalPrice);
    }

    public void Hide()
    {
        _priceUIView.CanvasGroup.alpha = PriceViewHandlerValues.MinValue;
        PanelOff();
    }
    private void PanelOff()
    {
        _priceUIView.gameObject.SetActive(false);
        _panelIsShowed = false;
    }
    private void PanelOn(int price, int additionalPrice)
    {
        _priceUIView.gameObject.SetActive(true);
        switch (_resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetMode(price);
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode(additionalPrice);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetAndHeartsMode(price, additionalPrice);
                break;
        }
        _panelIsShowed = true;
    }

    private void SetMonetMode(int price)
    {
        _priceUIView.MonetsBlock.gameObject.SetActive(true);
        _priceUIView.HeartsBlock.gameObject.SetActive(false);
        _priceUIView.RectTransform.anchoredPosition = _values.Mode1PosPricePanel;
        _priceUIView.BackgroundRectTransform.anchoredPosition = _values.Mode1PosImageBackground;
        _priceUIView.BackgroundRectTransform.sizeDelta = _values.Mode1SizeImageBackground;
        _priceUIView.MonetsBlock.anchoredPosition = _values.PosMonetBlock;
        _priceUIView.MonetsPriceText.text = price.ToString();
    }

    private void SetHeartsMode(int additionalPrice)
    {
        _priceUIView.MonetsBlock.gameObject.SetActive(false);
        _priceUIView.HeartsBlock.gameObject.SetActive(true);
        _priceUIView.RectTransform.anchoredPosition = _values.Mode1PosPricePanel;
        _priceUIView.BackgroundRectTransform.anchoredPosition = _values.Mode1PosImageBackground;
        _priceUIView.BackgroundRectTransform.sizeDelta = _values.Mode1SizeImageBackground;
        _priceUIView.HeartsBlock.anchoredPosition = _values.PosMonetBlock;
        _priceUIView.HeartsPriceText.text = additionalPrice.ToString();
    }

    private void SetMonetAndHeartsMode(int price, int additionalPrice)
    {
        _priceUIView.MonetsBlock.gameObject.SetActive(true);
        _priceUIView.HeartsBlock.gameObject.SetActive(true);
        _priceUIView.RectTransform.anchoredPosition = _values.Mode2PosPricePanel;
        _priceUIView.BackgroundRectTransform.anchoredPosition = _values.Mode2PosImageBackground;
        _priceUIView.BackgroundRectTransform.sizeDelta = _values.Mode2SizeImageBackground;
        _priceUIView.MonetsBlock.anchoredPosition = _values.PosMonetBlock;
        _priceUIView.HeartsBlock.anchoredPosition = _values.PosHeartsBlock;
        _priceUIView.MonetsPriceText.text = price.ToString();
        _priceUIView.HeartsPriceText.text = additionalPrice.ToString();
    }
}