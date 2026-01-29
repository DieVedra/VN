using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PanelResourceVisionHandler
{
    public const float MinValue = 0f;
    public const float MaxValue = 1f;
    private const float _duration = 0.2f;
    private const float _delayDefault = 0f;
    // private readonly LevelResourceVisionHandlerValues _levelResourceVisionHandlerValues;
    private readonly ResourcePanelHandler _monetResourcePanelHandler, _heartsResourcePanelHandler;
    
    private ResourcesViewMode _resourcesViewMode;
    private CancellationTokenSource _cancellationTokenSource;
    public PanelResourceVisionHandler(ResourcePanelHandler monetResourcePanelHandler, ResourcePanelHandler heartsResourcePanelHandler)
    {
        // _levelResourceVisionHandlerValues = new LevelResourceVisionHandlerValues(monetResourcePanelHandler.PanelTransform, heartsResourcePanelHandler.PanelTransform);
        _monetResourcePanelHandler = monetResourcePanelHandler;
        _heartsResourcePanelHandler = heartsResourcePanelHandler;
    }
    public void Init(RectTransform monetResourceParent, RectTransform heartResourceParent, ResourcesViewMode resourcesViewMode)
    {
        _resourcesViewMode = resourcesViewMode;
        switch (_resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetsMode(monetResourceParent, heartResourceParent);
                // _monetResourcePanelHandler.DoPanel(MaxValue, true);
                // _heartsResourcePanelHandler.DoPanel(MinValue, false);
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode(monetResourceParent, heartResourceParent);
                // _monetResourcePanelHandler.DoPanel(MinValue, false);
                // _heartsResourcePanelHandler.DoPanel(MaxValue, true);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetsAndHeartsMode(monetResourceParent, heartResourceParent);
                // _monetResourcePanelHandler.DoPanel(MaxValue, true);
                // _heartsResourcePanelHandler.DoPanel(MaxValue, true);
                break;
            case ResourcesViewMode.Hide:
                // _heartsResourcePanelHandler.DoPanel(MinValue, false);
                // _monetResourcePanelHandler.DoPanel(MinValue, false);
                break;
        }
    }
    public void Init(ResourcesViewMode resourcesViewMode)
    {
        _resourcesViewMode = resourcesViewMode;
        // TryShowAndHidePanelOnButtonsSwitch();
    }
    //
    // private void TryShowAndHidePanelOnButtonsSwitch()
    // {
    //     switch (_resourcesViewMode)
    //     {
    //         case ResourcesViewMode.MonetMode:
    //             SetMonetsMode();
    //             _monetResourcePanelHandler.DoPanel(MaxValue, true);
    //             _heartsResourcePanelHandler.DoPanel(MinValue, false);
    //             break;
    //         case ResourcesViewMode.HeartsMode:
    //             SetHeartsMode();
    //             _monetResourcePanelHandler.DoPanel(MinValue, false);
    //             _heartsResourcePanelHandler.DoPanel(MaxValue, true);
    //             break;
    //         case ResourcesViewMode.MonetsAndHeartsMode:
    //             SetMonetsAndHeartsMode();
    //             _monetResourcePanelHandler.DoPanel(MaxValue, true);
    //             _heartsResourcePanelHandler.DoPanel(MaxValue, true);
    //             break;
    //         case ResourcesViewMode.Hide:
    //             _heartsResourcePanelHandler.DoPanel(MinValue, false);
    //             _monetResourcePanelHandler.DoPanel(MinValue, false);
    //             break;
    //     }
    // }

    public async UniTask Show(RectTransform monetResourceParent, RectTransform heartResourceParent, ResourcesViewMode resourcesViewMode, float duration = _duration)
    {
        _resourcesViewMode = resourcesViewMode;
        switch (_resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetsMode(monetResourceParent, heartResourceParent);
                await _monetResourcePanelHandler.DoAnimPanel(duration, MinValue, MaxValue, true);
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode(monetResourceParent, heartResourceParent);
                await _heartsResourcePanelHandler.DoAnimPanel(duration, MinValue, MaxValue, true);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetsAndHeartsMode(monetResourceParent, heartResourceParent);
                await UniTask.WhenAll(
                    _monetResourcePanelHandler.DoAnimPanel(duration, MinValue, MaxValue, true),
                    _heartsResourcePanelHandler.DoAnimPanel(duration, MinValue, MaxValue, true));
                break;
        }
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
                await _monetResourcePanelHandler.DoAnimPanel(duration, MaxValue, MinValue, false);
                break;
            case ResourcesViewMode.HeartsMode:
                await _heartsResourcePanelHandler.DoAnimPanel(duration, MaxValue, MinValue, false);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                await UniTask.WhenAll(
                    _monetResourcePanelHandler.DoAnimPanel(duration, MaxValue, MinValue, false),
                    _heartsResourcePanelHandler.DoAnimPanel(duration, MaxValue, MinValue, false));
                break;
        }
    }

    private void SetHeartsMode(RectTransform monetResourceParent, RectTransform heartResourceParent)
    {
    }

    private void SetMonetsMode(RectTransform monetResourceParent, RectTransform heartResourceParent)
    {
    }

    private void SetMonetsAndHeartsMode(RectTransform monetResourceParent, RectTransform heartResourceParent)
    {
    }
}