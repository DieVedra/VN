using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class PriceViewHandler
{
    private const float _height1ImagePanel = 230;
    private const float _height2ImagePanel = 330;
    private const float _posYMonetBlock = -156;
    private const float _posYHeartsBlock = -258;
    private const float _minValue = 0f;
    private const float _maxValue = 1f;
    private readonly float _duration;
    private readonly PriceUIView _priceUIView;
    private readonly ResourcesViewMode _resourcesViewMode;
    private Vector2 _values = new Vector2();
    private CancellationTokenSource _cancellationTokenSource;

    private bool _panelIsShowed;
    public bool PanelIsShowed => _panelIsShowed;
    public PriceViewHandler(PriceUIView priceUIView, ResourcesViewMode resourcesViewMode, float duration)
    {
        _priceUIView = priceUIView;
        _resourcesViewMode = resourcesViewMode;
        _panelIsShowed = false;
        _duration = duration;
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
            await _priceUIView.CanvasGroup.DOFade(_minValue, _duration)
                .WithCancellation(_cancellationTokenSource.Token);
            PanelOff();
        }
    }

    public async UniTask ShowAnim(int price, int additionalPrice)
    {
        _priceUIView.CanvasGroup.alpha = _minValue;
        PanelOn(price, additionalPrice);
        _cancellationTokenSource = new CancellationTokenSource();
        await _priceUIView.CanvasGroup.DOFade(_maxValue, _duration)
            .WithCancellation(_cancellationTokenSource.Token);
    }
    public void Show(int price, int additionalPrice)
    {
        _priceUIView.CanvasGroup.alpha = _maxValue;
        PanelOn(price, additionalPrice);
    }

    public void Hide()
    {
        _priceUIView.CanvasGroup.alpha = _minValue;
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
        _values.x = _priceUIView.ImageBackgroundRectTransform.sizeDelta.x;
        _values.y = _height1ImagePanel;
        _priceUIView.ImageBackgroundRectTransform.sizeDelta = _values;
        _values.x = _priceUIView.MonetsBlock.anchoredPosition.x;
        _values.y = _posYMonetBlock;
        _priceUIView.MonetsBlock.anchoredPosition = _values;
        _priceUIView.MonetsPriceText.text = price.ToString();
    }

    private void SetHeartsMode(int additionalPrice)
    {
        _priceUIView.MonetsBlock.gameObject.SetActive(false);
        _priceUIView.HeartsBlock.gameObject.SetActive(true);
        _values.x = _priceUIView.ImageBackgroundRectTransform.sizeDelta.x;
        _values.y = _height1ImagePanel;
        _priceUIView.ImageBackgroundRectTransform.sizeDelta = _values;
        
        _values.x = _priceUIView.MonetsBlock.anchoredPosition.x;
        _values.y = _posYMonetBlock;
        _priceUIView.HeartsBlock.anchoredPosition = _values;
        _priceUIView.HeartsPriceText.text = additionalPrice.ToString();
    }

    private void SetMonetAndHeartsMode(int price, int additionalPrice)
    {
        _priceUIView.MonetsBlock.gameObject.SetActive(true);
        _priceUIView.HeartsBlock.gameObject.SetActive(true);
        _values.x = _priceUIView.ImageBackgroundRectTransform.sizeDelta.x;
        _values.y = _height2ImagePanel;
        _priceUIView.ImageBackgroundRectTransform.anchoredPosition = _values;

        _values.x = _priceUIView.MonetsBlock.anchoredPosition.x;
        _values.y = _posYMonetBlock;
        _priceUIView.MonetsBlock.anchoredPosition = _values;
        
        _values.x = _priceUIView.HeartsBlock.anchoredPosition.x;
        _values.y = _posYHeartsBlock;
        _priceUIView.HeartsBlock.anchoredPosition = _values;
        
        _priceUIView.MonetsPriceText.text = price.ToString();
        _priceUIView.HeartsPriceText.text = additionalPrice.ToString();
    }
}