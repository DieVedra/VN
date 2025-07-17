using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class PriceViewHandler
{
    private const float _minValue = 0f;
    private const float _maxValue = 1f;
    private const float _posYMonetBlock = -156;
    private const float _posYHeartsBlock = -291;
    
    private const float _posY1ImageBackground = -115;
    private const float _height1ImageBackground = 243;
    
    private const float _posY2ImageBackground = -163;
    private const float _height2ImageBackground = 340;
    private const float _widhtImageBackground = 272;
    
    private readonly float _duration;
    private readonly PriceUIView _priceUIView;
    private readonly LevelResourceHandler _levelResourceHandler;
    private readonly CalculatePriceHandler _calculatePriceHandler;
    
    private CancellationTokenSource _cancellationTokenSource;
    private CompositeDisposable _compositeDisposable;

    private bool _panelIsShowed;
    private Vector2 _mode1PosImageBackground => new Vector2(_minValue, _posY1ImageBackground);
    private Vector2 _mode2PosImageBackground => new Vector2(_minValue, _posY2ImageBackground);
    private Vector2 _mode1SizeImageBackground => new Vector2(_widhtImageBackground, _height1ImageBackground);
    private Vector2 _mode2SizeImageBackground => new Vector2(_widhtImageBackground, _height2ImageBackground);
    private Vector2 _posMonetBlock => new Vector2(_minValue, _posYMonetBlock);
    private Vector2 _posHeartsBlock => new Vector2(_minValue, _posYHeartsBlock);
    public bool PanelIsShowed => _panelIsShowed;
    public CalculatePriceHandler CalculatePriceHandler => _calculatePriceHandler;
    public PriceViewHandler(PriceUIView priceUIView, LevelResourceHandler levelResourceHandler,
        CalculatePriceHandler calculatePriceHandler, float duration)
    {
        _priceUIView = priceUIView;
        _levelResourceHandler = levelResourceHandler;
        _calculatePriceHandler = calculatePriceHandler;
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
            await UniTask.WhenAll(_levelResourceHandler.AnimHidePanelMonets(_cancellationTokenSource.Token, _duration),
                _levelResourceHandler.TryAnimHidePanelHearts(_cancellationTokenSource.Token, _duration),
                _priceUIView.CanvasGroup.DOFade(_minValue, _duration).WithCancellation(_cancellationTokenSource.Token));
            PanelOff();
        }
    }

    public async UniTask ShowAnim(int price, int additionalPrice)
    {
        _priceUIView.CanvasGroup.alpha = _minValue;
        PanelOn(price, additionalPrice);
        _cancellationTokenSource = new CancellationTokenSource();
        List<UniTask> tasks = new List<UniTask>
        {
            _priceUIView.CanvasGroup.DOFade(_maxValue, _duration).WithCancellation(_cancellationTokenSource.Token)
        };
        if (price > 0)
        {
            tasks.Add(_levelResourceHandler.AnimShowPanelMonets(_cancellationTokenSource.Token, _duration));
        }
        if (additionalPrice > 0)
        {
            tasks.Add(_levelResourceHandler.AnimShowPanelHearts(_cancellationTokenSource.Token, _duration));
        }
        await UniTask.WhenAll(tasks);
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
        _levelResourceHandler.OffAll();
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
        _priceUIView.BackgroundRectTransform.anchoredPosition = _mode1PosImageBackground;
        _priceUIView.BackgroundRectTransform.sizeDelta = _mode1SizeImageBackground;
        // _priceUIView.MonetsBlock.anchoredPosition = _posMonetBlock;
        _priceUIView.MonetsPriceText.text = price.ToString();
        _levelResourceHandler.HideHeartsPanel();
        _levelResourceHandler.ShowMonetPanel();
        _calculatePriceHandler.MonetsToShowReactiveProperty.Subscribe(_ =>
        {
            _levelResourceHandler.SetMonet(_.ToString());
        }).AddTo(_compositeDisposable);
    }

    private void SetHeartsMode(int additionalPrice)
    {
        _priceUIView.MonetsBlock.gameObject.SetActive(false);
        _priceUIView.HeartsBlock.gameObject.SetActive(true);
        _priceUIView.BackgroundRectTransform.anchoredPosition = _mode1PosImageBackground;
        _priceUIView.BackgroundRectTransform.sizeDelta = _mode1SizeImageBackground;
        _priceUIView.HeartsBlock.anchoredPosition = _posMonetBlock;
        _priceUIView.HeartsPriceText.text = additionalPrice.ToString();
        _levelResourceHandler.HideMonetPanel();
        _levelResourceHandler.ShowHeartsPanel();
        _calculatePriceHandler.HeartsToShowReactiveProperty.Subscribe(_ =>
        {
            _levelResourceHandler.SetHearts(_.ToString());
        }).AddTo(_compositeDisposable);
    }

    private void SetMonetAndHeartsMode(int price, int additionalPrice)
    {
        _priceUIView.MonetsBlock.gameObject.SetActive(true);
        _priceUIView.HeartsBlock.gameObject.SetActive(true);
        _priceUIView.BackgroundRectTransform.anchoredPosition = _mode2PosImageBackground;
        _priceUIView.BackgroundRectTransform.sizeDelta = _mode2SizeImageBackground;
        _priceUIView.MonetsBlock.anchoredPosition = _posMonetBlock;
        _priceUIView.HeartsBlock.anchoredPosition = _posHeartsBlock;
        _priceUIView.MonetsPriceText.text = price.ToString();
        _priceUIView.HeartsPriceText.text = additionalPrice.ToString();
        _levelResourceHandler.ShowMonetPanel();
        _levelResourceHandler.ShowHeartsPanel();
        _calculatePriceHandler.MonetsToShowReactiveProperty.Subscribe(_ =>
        {
            _levelResourceHandler.SetMonet(_.ToString());
        }).AddTo(_compositeDisposable);

        _calculatePriceHandler.HeartsToShowReactiveProperty.Subscribe(_ =>
        {
            _levelResourceHandler.SetHearts(_.ToString());
        }).AddTo(_compositeDisposable);
    }
}