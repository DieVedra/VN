using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

public class PriceViewHandler
{
    private readonly float _duration;
    private readonly PriceViewHandlerValues _values;
    private readonly PriceUIView _priceUIView;
    private readonly CalculatePriceHandler _calculatePriceHandler;
    
    private CancellationTokenSource _cancellationTokenSource;
    private CompositeDisposable _compositeDisposable;

    private bool _panelIsShowed;
    public bool PanelIsShowed => _panelIsShowed;
    public CalculatePriceHandler CalculatePriceHandler => _calculatePriceHandler;
    public PriceViewHandler(PriceUIView priceUIView,
        CalculatePriceHandler calculatePriceHandler, float duration)
    {
        _priceUIView = priceUIView;
        _calculatePriceHandler = calculatePriceHandler;
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
            // await UniTask.WhenAll(_levelResourceHandler.AnimHidePanelMonets(_cancellationTokenSource.Token, _duration),
            //     _levelResourceHandler.TryAnimHidePanelHearts(_cancellationTokenSource.Token, _duration),
            //     _priceUIView.CanvasGroup.DOFade(PriceViewHandlerValues.MinValue, _duration).WithCancellation(_cancellationTokenSource.Token));

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
        // var taskRunner = new TaskRunner();
        // taskRunner.AddOperationToList(() => _priceUIView.CanvasGroup.DOFade(PriceViewHandlerValues.MaxValue, _duration).WithCancellation(_cancellationTokenSource.Token));
        // if (price > 0)
        // {
        //     taskRunner.AddOperationToList(() => _levelResourceHandler.AnimShowPanelMonets(_cancellationTokenSource.Token, _duration));
        // }
        // if (additionalPrice > 0)
        // {
        //     taskRunner.AddOperationToList(() => _levelResourceHandler.AnimShowPanelHearts(_cancellationTokenSource.Token, _duration));
        // }
        // await taskRunner.TryRunTasks();
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
        // _levelResourceHandler.OffAll();
        _panelIsShowed = false;
    }
    private void PanelOn(int price, int additionalPrice)
    {
        _priceUIView.gameObject.SetActive(true);
        _compositeDisposable?.Clear();
        _compositeDisposable = new CompositeDisposable();
        if (price > 0 && additionalPrice > 0)
        {
            SetMonetAndHeartsMode(price, additionalPrice);
        }
        else if (price > 0)
        {
            SetMonetMode(price);
        }
        else if(additionalPrice > 0)
        {
            SetHeartsMode(additionalPrice);
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
        // _levelResourceHandler.HideHeartsPanel();
        // _levelResourceHandler.SetMonetsMode();
        // _levelResourceHandler.ShowMonetPanel();
        // _calculatePriceHandler.MonetsToShowReactiveProperty.Subscribe(_ =>
        // {
        //     _levelResourceHandler.SetMonet(_.ToString());
        // }).AddTo(_compositeDisposable);
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
        // _levelResourceHandler.HideMonetPanel();
        // _levelResourceHandler.SetHeartsMode();
        // _levelResourceHandler.ShowHeartsPanel();
        // _calculatePriceHandler.HeartsToShowReactiveProperty.Subscribe(_ =>
        // {
        //     _levelResourceHandler.SetHearts(_.ToString());
        // }).AddTo(_compositeDisposable);
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
        // _levelResourceHandler.SetMonetsAndHeartsMode();
        // _levelResourceHandler.ShowMonetPanel();
        // _levelResourceHandler.ShowHeartsPanel();
        // _calculatePriceHandler.MonetsToShowReactiveProperty.Subscribe(_ =>
        // {
        //     _levelResourceHandler.SetMonet(_.ToString());
        // }).AddTo(_compositeDisposable);
        //
        // _calculatePriceHandler.HeartsToShowReactiveProperty.Subscribe(_ =>
        // {
        //     _levelResourceHandler.SetHearts(_.ToString());
        // }).AddTo(_compositeDisposable);
    }
}