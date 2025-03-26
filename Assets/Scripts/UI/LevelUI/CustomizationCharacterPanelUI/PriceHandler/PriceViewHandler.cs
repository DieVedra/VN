using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;

public class PriceViewHandler
{
    private readonly float _duration;
    private readonly TextMeshProUGUI _moneyPanelText;
    private readonly TextMeshProUGUI _pricePanelText;
    
    private readonly CanvasGroup _moneyPanelCanvasGroup;
    private readonly CanvasGroup _pricePanelCanvasGroup;
    private readonly CalculatePriceHandler _calculatePriceHandler;
    
    private CancellationTokenSource _cancellationTokenSource;

    private bool _isShowed;
    public bool IsShowed => _isShowed;
    public CalculatePriceHandler CalculatePriceHandler => _calculatePriceHandler;
    public PriceViewHandler(TextMeshProUGUI moneyPanelText, TextMeshProUGUI pricePanelText,
        CanvasGroup moneyPanelCanvasGroup, CanvasGroup pricePanelCanvasGroup, CalculatePriceHandler calculatePriceHandler, float duration)
    {
        _moneyPanelText = moneyPanelText;
        _pricePanelText = pricePanelText;
        _moneyPanelCanvasGroup = moneyPanelCanvasGroup;
        _pricePanelCanvasGroup = pricePanelCanvasGroup;
        _calculatePriceHandler = calculatePriceHandler;
        _isShowed = false;
        _duration = duration;
    }

    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }
    public async UniTask HideToShowAnim(int price)
    {
        await HideAnim();
        await ShowAnim(price);
    }
    public async UniTask HideAnim()
    {
        if (_isShowed == true)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            await UniTask.WhenAll(_moneyPanelCanvasGroup.DOFade(0f, _duration).WithCancellation(_cancellationTokenSource.Token),
                _pricePanelCanvasGroup.DOFade(0f, _duration).WithCancellation(_cancellationTokenSource.Token));
            PanelOff();
        }
    }

    public async UniTask ShowAnim(int price)
    {
        PanelOn(price);
        _moneyPanelCanvasGroup.alpha = 0f;
        _pricePanelCanvasGroup.alpha = 0f;
        _cancellationTokenSource = new CancellationTokenSource();
        await UniTask.WhenAll(_moneyPanelCanvasGroup.DOFade(1f, _duration).WithCancellation(_cancellationTokenSource.Token),
            _pricePanelCanvasGroup.DOFade(1f, _duration).WithCancellation(_cancellationTokenSource.Token));
    }
    public void Show(int price)
    {
        PanelOn(price);
        _moneyPanelCanvasGroup.alpha = 1f;
        _pricePanelCanvasGroup.alpha = 1f;
    }

    public void Hide()
    {
        _moneyPanelCanvasGroup.alpha = 0f;
        _pricePanelCanvasGroup.alpha = 0f;
        PanelOff();
    }
    private void PanelOff()
    {
        _moneyPanelCanvasGroup.gameObject.SetActive(false);
        _pricePanelCanvasGroup.gameObject.SetActive(false);
        _isShowed = false;
    }

    private void PanelOn(int price)
    {
        _moneyPanelCanvasGroup.gameObject.SetActive(true);
        _pricePanelCanvasGroup.gameObject.SetActive(true);
        _isShowed = true;
        _moneyPanelText.text = _calculatePriceHandler.MoneyToShow.ToString();
        _pricePanelText.text = price.ToString();
    }
}