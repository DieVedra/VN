using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;

public class PanelResourceHandler
{
    private const float _duration = 0.2f;
    private const float _delayDefault = 0f;
    private readonly LevelResourceHandlerValues _levelResourceHandlerValues;
    private readonly ResourcePanelHandler _monetResourcePanelHandler, _heartsResourcePanelHandler;
    
    private ResourcesViewMode _resourcesViewMode;
    private CancellationTokenSource _cancellationTokenSource;
    public PanelResourceHandler(ResourcePanelHandler monetResourcePanelHandler, ResourcePanelHandler heartsResourcePanelHandler)
    {
        _levelResourceHandlerValues = new LevelResourceHandlerValues(monetResourcePanelHandler.PanelTransform, heartsResourcePanelHandler.PanelTransform);
        _monetResourcePanelHandler = monetResourcePanelHandler;
        _heartsResourcePanelHandler = heartsResourcePanelHandler;
    }
    public void Init(ResourcesViewMode resourcesViewMode)
    {
        _resourcesViewMode = resourcesViewMode;
        TryShowAndHidePanelOnButtonsSwitch();
    }
    public void Shutdown()
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
                _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, false);
                break;
            case ResourcesViewMode.HeartsMode:
                await AnimHidePanelHearts(duration);
                _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, false);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                await UniTask.WhenAll(AnimHidePanelMonets(duration), AnimHidePanelHearts(duration));
                _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, false);
                _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, false);
                break;
        }
    }

    public async UniTask Show(float duration = _duration)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, true);
        _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, true);
        switch (_resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetsMode();
                _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, true);
                await AnimShowPanelMonets(duration);
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode();
                _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, true);
                await AnimShowPanelHearts(duration);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetsAndHeartsMode();
                _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, true);
                _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, true);
                await UniTask.WhenAll(AnimShowPanelMonets(duration), AnimShowPanelHearts(duration));
                break;
        }
    }

    private void TryShowAndHidePanelOnButtonsSwitch()
    {
        switch (_resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetsMode();
                _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MaxValue, true);
                _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, false);
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode();
                _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, false);
                _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MaxValue, true);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetsAndHeartsMode();
                _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MaxValue, true);
                _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MaxValue, true);
                break;
            case ResourcesViewMode.Hide:
                _heartsResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, false);
                _monetResourcePanelHandler.DoPanel(LevelResourceHandlerValues.MinValue, false);
                break;
        }
    }

    private void SetHeartsMode()
    {
        var rectTransform = _heartsResourcePanelHandler.PanelTransform;
        rectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin;
        rectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;
        rectTransform.offsetMin = _levelResourceHandlerValues.MonetOffsetMin;
        rectTransform.offsetMax = _levelResourceHandlerValues.MonetOffsetMax;
    }

    private void SetMonetsMode()
    {
        var monetsRectTransform = _monetResourcePanelHandler.PanelTransform;
        monetsRectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin;
        monetsRectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;
        monetsRectTransform.offsetMin = _levelResourceHandlerValues.MonetOffsetMin;
        monetsRectTransform.offsetMax = _levelResourceHandlerValues.MonetOffsetMax;
    }

    private void SetMonetsAndHeartsMode()
    {
        var heartsRectTransform = _heartsResourcePanelHandler.PanelTransform;
        heartsRectTransform.anchorMin = _levelResourceHandlerValues.HeartsAnchorsMin;
        heartsRectTransform.anchorMax = _levelResourceHandlerValues.HeartsAnchorsMax;
        heartsRectTransform.offsetMin = _levelResourceHandlerValues.HeartsOffsetMin;
        heartsRectTransform.offsetMax = _levelResourceHandlerValues.HeartsOffsetMax;
        
        var monetsRectTransform = _monetResourcePanelHandler.PanelTransform;;
        monetsRectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin;
        monetsRectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;
        monetsRectTransform.offsetMin = _levelResourceHandlerValues.MonetOffsetMin;
        monetsRectTransform.offsetMax = _levelResourceHandlerValues.MonetOffsetMax;
    }

    private async UniTask AnimShowPanelMonets(float duration)
    {
        await _monetResourcePanelHandler.DoAnimPanel( _cancellationTokenSource.Token, duration, LevelResourceHandlerValues.MinValue, LevelResourceHandlerValues.MaxValue);
    }

    private async UniTask AnimHidePanelMonets(float duration)
    {
        await _monetResourcePanelHandler.DoAnimPanel(_cancellationTokenSource.Token, duration, LevelResourceHandlerValues.MaxValue, LevelResourceHandlerValues.MinValue);
    }

    private async UniTask AnimShowPanelHearts(float duration)
    {
        await _heartsResourcePanelHandler.DoAnimPanel(_cancellationTokenSource.Token, duration, LevelResourceHandlerValues.MinValue, LevelResourceHandlerValues.MaxValue);
    }

    private async UniTask AnimHidePanelHearts(float duration)
    {
        await _heartsResourcePanelHandler.DoAnimPanel(_cancellationTokenSource.Token, duration, LevelResourceHandlerValues.MaxValue, LevelResourceHandlerValues.MinValue);
    }
}