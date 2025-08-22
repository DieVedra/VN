using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;

public class PanelResourceHandler
{
    private const float _duration = 0.2f;
    private const float _delayDefault = 0f;
    private readonly LevelResourceHandlerValues _levelResourceHandlerValues;
    private readonly Wallet _wallet;
    private readonly ResourcePanelWithCanvasGroupView _monetResourcePanelWithCanvasGroupView;
    private readonly ResourcePanelWithCanvasGroupView _heartsResourcePanelWithCanvasGroupView;
    private ResourcesViewMode _resourcesViewMode;
    private CancellationTokenSource _cancellationTokenSource;
    public PanelResourceHandler(Wallet wallet,
        ResourcePanelWithCanvasGroupView monetResourcePanelWithCanvasGroupView, ResourcePanelWithCanvasGroupView heartsResourcePanelWithCanvasGroupView)
    {
        _wallet = wallet;
        _monetResourcePanelWithCanvasGroupView = monetResourcePanelWithCanvasGroupView;
        _heartsResourcePanelWithCanvasGroupView = heartsResourcePanelWithCanvasGroupView;
        _levelResourceHandlerValues = new LevelResourceHandlerValues(monetResourcePanelWithCanvasGroupView.RectTransform, heartsResourcePanelWithCanvasGroupView.RectTransform);
        _wallet.MonetsCountChanged.Subscribe(_ =>
        {
            _monetResourcePanelWithCanvasGroupView.Text.text = _.ToString();
        });

        _wallet.HeartsCountChanged.Subscribe(_ =>
        {
            _heartsResourcePanelWithCanvasGroupView.Text.text = _.ToString();;
        });
    }

    public void Init(ResourcesViewMode resourcesViewMode)
    {
        _resourcesViewMode = resourcesViewMode;
        _monetResourcePanelWithCanvasGroupView.Text.text = _wallet.GetMonetsCount.ToString();
        _heartsResourcePanelWithCanvasGroupView.Text.text = _wallet.GetHeartsCount.ToString();
        TryShowOrHidePanelOnButtonsSwitch();
    }
    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
    }

    public async UniTask TryHidePanel(float delay = _delayDefault, float duration = _duration)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        if (delay > _delayDefault)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _cancellationTokenSource.Token);
        }
        switch (_resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                await AnimHidePanelMonets(duration);
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                break;
            case ResourcesViewMode.HeartsMode:
                await AnimHidePanelHearts(duration);
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                await UniTask.WhenAll(AnimHidePanelMonets(duration), AnimHidePanelHearts(duration));
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
                break;
        }
    }

    public async UniTask Show(float duration = _duration)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
        DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
        switch (_resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetsMode();
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
                await AnimShowPanelMonets(duration);
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode();
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
                await AnimShowPanelHearts(duration);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetsAndHeartsMode();
                DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
                DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, true);
                await UniTask.WhenAll(AnimShowPanelMonets(duration), AnimShowPanelHearts(duration));
                break;
        }
    }

    private void TryShowOrHidePanelOnButtonsSwitch()
    {
        switch (_resourcesViewMode)
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

    private async UniTask AnimShowPanelMonets(float duration)
    {
        await DoAnimPanel(_monetResourcePanelWithCanvasGroupView, _cancellationTokenSource.Token, duration, LevelResourceHandlerValues.MinValue, LevelResourceHandlerValues.MaxValue);
    }

    private async UniTask AnimHidePanelMonets(float duration)
    {
        await DoAnimPanel(_monetResourcePanelWithCanvasGroupView, _cancellationTokenSource.Token, duration, LevelResourceHandlerValues.MaxValue, LevelResourceHandlerValues.MinValue);
    }

    private async UniTask AnimShowPanelHearts(float duration)
    {
        await DoAnimPanel(_heartsResourcePanelWithCanvasGroupView, _cancellationTokenSource.Token, duration, LevelResourceHandlerValues.MinValue, LevelResourceHandlerValues.MaxValue);
    }

    private async UniTask AnimHidePanelHearts(float duration)
    {
        await DoAnimPanel(_heartsResourcePanelWithCanvasGroupView, _cancellationTokenSource.Token, duration, LevelResourceHandlerValues.MaxValue, LevelResourceHandlerValues.MinValue);
    }
}