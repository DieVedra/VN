using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

public class CustomizationPanelResourceHandler
{
    private readonly LevelResourceHandlerValues _levelResourceHandlerValues;
    private readonly CalculateBalanceHandler _calculateBalanceHandler;
    private readonly ResourcePanelWithCanvasGroupView _monetResourcePanelWithCanvasGroupView;
    private readonly ResourcePanelWithCanvasGroupView _heartsResourcePanelWithCanvasGroupView;
    private readonly float _duration;
    private ResourcesViewMode _currentResourcesViewMode;
    private CompositeDisposable _compositeDisposable;
    private CancellationTokenSource _cancellationTokenSource;
    public CustomizationPanelResourceHandler(CalculateBalanceHandler calculateBalanceHandler, 
        ResourcePanelWithCanvasGroupView monetResourcePanelWithCanvasGroupView, ResourcePanelWithCanvasGroupView heartsResourcePanelWithCanvasGroupView,
        float duration)
    {
        _calculateBalanceHandler = calculateBalanceHandler;
        _monetResourcePanelWithCanvasGroupView = monetResourcePanelWithCanvasGroupView;
        _heartsResourcePanelWithCanvasGroupView = heartsResourcePanelWithCanvasGroupView;
        _duration = duration;
        _levelResourceHandlerValues = new LevelResourceHandlerValues(monetResourcePanelWithCanvasGroupView.RectTransform, heartsResourcePanelWithCanvasGroupView.RectTransform);
        _compositeDisposable = new CompositeDisposable();
        calculateBalanceHandler.MonetsToShowReactiveProperty.Subscribe(_ =>
        {
            _monetResourcePanelWithCanvasGroupView.Text.text = _.ToString();
        }).AddTo(_compositeDisposable);
        
        calculateBalanceHandler.HeartsToShowReactiveProperty.Subscribe(_ =>
        {
            _heartsResourcePanelWithCanvasGroupView.Text.text = _.ToString();
        }).AddTo(_compositeDisposable);
    }

    public void Dispose()
    {
        _compositeDisposable?.Clear();
        _cancellationTokenSource?.Cancel();
    }
    public void TryShowOrHidePanelOnButtonsSwitch(ResourcesViewMode resourcesViewMode)
    {
        switch (resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetsMode();
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MaxValue, true);
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode();
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MaxValue, true);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetsAndHeartsMode();
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MaxValue, true);
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MaxValue, true);
                break;
            case ResourcesViewMode.Hide:
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                break;
        }
        _currentResourcesViewMode = resourcesViewMode;
    }

    public async UniTask TryShowOrHideOnArrowsSwitch(ResourcesViewMode resourcesViewMode)
    {
        await TryHidePanel();
        _calculateBalanceHandler.PreliminaryBalanceCalculation();
        switch (resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetsMode();
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
                await AnimShowPanelMonets();
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode();
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
                await AnimShowPanelHearts();
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetsAndHeartsMode();
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
                await UniTask.WhenAll(AnimShowPanelMonets(), AnimShowPanelHearts());
                break;
        }
        _currentResourcesViewMode = resourcesViewMode;
    }

    private void SetHeartsMode()
    {
        var rectTransform = _heartsResourcePanelWithCanvasGroupView.RectTransform;
        rectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin;
        rectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;
        rectTransform.offsetMin = _levelResourceHandlerValues.MonetOffsetMin;
        rectTransform.offsetMax = _levelResourceHandlerValues.MonetOffsetMax;
    }

    private void SetMonetsMode()
    {
        var monetsRectTransform = _monetResourcePanelWithCanvasGroupView.RectTransform;
        monetsRectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin;
        monetsRectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;
        monetsRectTransform.offsetMin = _levelResourceHandlerValues.MonetOffsetMin;
        monetsRectTransform.offsetMax = _levelResourceHandlerValues.MonetOffsetMax;
    }

    private void SetMonetsAndHeartsMode()
    {
        var heartsRectTransform = _heartsResourcePanelWithCanvasGroupView.RectTransform;
        heartsRectTransform.anchorMin = _levelResourceHandlerValues.HeartsAnchorsMin;
        heartsRectTransform.anchorMax = _levelResourceHandlerValues.HeartsAnchorsMax;
        heartsRectTransform.offsetMin = _levelResourceHandlerValues.HeartsOffsetMin;
        heartsRectTransform.offsetMax = _levelResourceHandlerValues.HeartsOffsetMax;
        
        var monetsRectTransform = _monetResourcePanelWithCanvasGroupView.RectTransform;
        monetsRectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin;
        monetsRectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;
        monetsRectTransform.offsetMin = _levelResourceHandlerValues.MonetOffsetMin;
        monetsRectTransform.offsetMax = _levelResourceHandlerValues.MonetOffsetMax;
    }

    private void DoPanel(ResourcePanelWithCanvasGroupView view, float alpha, bool key)
    {
        view.CanvasGroup.alpha = alpha;
        view.gameObject.SetActive(key);
    }

    private async UniTask DoAnimPanel(ResourcePanelWithCanvasGroupView view, CancellationToken cancellationToken, 
        float duration, float alpha, float endValue)
    {
        view.CanvasGroup.alpha = alpha;
        await view.CanvasGroup.DOFade(endValue, duration)
            .WithCancellation(cancellationToken);
    }

    public async UniTask TryHidePanel()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        switch (_currentResourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                await AnimHidePanelMonets();
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                break;
            case ResourcesViewMode.HeartsMode:
                await AnimHidePanelHearts();
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                await UniTask.WhenAll(AnimHidePanelMonets(), AnimHidePanelHearts());
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                break;
        }
    }

    private async UniTask AnimShowPanelMonets()
    {
        await DoAnimPanel(_monetResourcePanelWithCanvasGroupView, _cancellationTokenSource.Token, _duration, LevelResourceHandlerValues.MinValue, LevelResourceHandlerValues.MaxValue);
    }

    private async UniTask AnimHidePanelMonets()
    {
        await DoAnimPanel(_monetResourcePanelWithCanvasGroupView, _cancellationTokenSource.Token, _duration, LevelResourceHandlerValues.MaxValue, LevelResourceHandlerValues.MinValue);
    }

    private async UniTask AnimShowPanelHearts()
    {
        await DoAnimPanel(_heartsResourcePanelWithCanvasGroupView, _cancellationTokenSource.Token, _duration, LevelResourceHandlerValues.MinValue, LevelResourceHandlerValues.MaxValue);
    }

    private async UniTask AnimHidePanelHearts()
    {
        await DoAnimPanel(_heartsResourcePanelWithCanvasGroupView, _cancellationTokenSource.Token, _duration, LevelResourceHandlerValues.MaxValue, LevelResourceHandlerValues.MinValue);
    }
}