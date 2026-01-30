using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PanelResourceVisionHandler
{
    private const float _minValue = 0f;
    private const float _maxValue = 1f;
    private const float _duration = 0.2f;
    private const float _delayDefault = 0f;
    private readonly ResourcePanelHandler _monetResourcePanelHandler, _heartsResourcePanelHandler;

    private Transform _buferMonetsDefaultParent, _buferHeartsDefaultParent;
    private ResourcesViewMode _resourcesViewMode;
    private CancellationTokenSource _cancellationTokenSource;
    public PanelResourceVisionHandler(ResourcePanelHandler monetResourcePanelHandler, ResourcePanelHandler heartsResourcePanelHandler)
    {
        _monetResourcePanelHandler = monetResourcePanelHandler;
        _heartsResourcePanelHandler = heartsResourcePanelHandler;
    }

    public async UniTask Show(RectTransform monetResourceParent, RectTransform heartResourceParent, ResourcesViewMode resourcesViewMode, float duration = _duration)
    {
        _resourcesViewMode = resourcesViewMode;
        switch (_resourcesViewMode)
        {
            case ResourcesViewMode.MonetMode:
                SetMonetsMode(monetResourceParent, heartResourceParent);
                await _monetResourcePanelHandler.DoAnimPanel(duration, _minValue, _maxValue, true);
                break;
            case ResourcesViewMode.HeartsMode:
                SetHeartsMode(monetResourceParent, heartResourceParent);
                await _heartsResourcePanelHandler.DoAnimPanel(duration, _minValue, _maxValue, true);
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                SetMonetsAndHeartsMode(monetResourceParent, heartResourceParent);
                await UniTask.WhenAll(
                    _monetResourcePanelHandler.DoAnimPanel(duration, _minValue, _maxValue, true),
                    _heartsResourcePanelHandler.DoAnimPanel(duration, _minValue, _maxValue, true));
                break;
            case ResourcesViewMode.Hide:
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
                await _monetResourcePanelHandler.DoAnimPanel(duration, _maxValue, _minValue, false);
                SkipMonetMode();
                break;
            case ResourcesViewMode.HeartsMode:
                await _heartsResourcePanelHandler.DoAnimPanel(duration, _maxValue, _minValue, false);
                SkipHeartsMode();
                break;
            case ResourcesViewMode.MonetsAndHeartsMode:
                await UniTask.WhenAll(
                    _monetResourcePanelHandler.DoAnimPanel(duration, _maxValue, _minValue, false),
                    _heartsResourcePanelHandler.DoAnimPanel(duration, _maxValue, _minValue, false));
                SkipMonetMode();
                SkipHeartsMode();
                break;
        }
    }

    private void SkipHeartsMode()
    {
        _heartsResourcePanelHandler.SetDefaultParent(_buferHeartsDefaultParent);
        _heartsResourcePanelHandler.TransferToDefault(ResourcePanelMode.WithAddButton);
    }

    private void SkipMonetMode()
    {
        _monetResourcePanelHandler.SetDefaultParent(_buferMonetsDefaultParent);
        _monetResourcePanelHandler.TransferToDefault(ResourcePanelMode.WithAddButton);
    }

    private void SetHeartsMode(RectTransform monetResourceParent, RectTransform heartResourceParent)
    {
        monetResourceParent.gameObject.SetActive(true);
        heartResourceParent.gameObject.SetActive(false);
        _buferHeartsDefaultParent = _heartsResourcePanelHandler.ParentDefault;
        _heartsResourcePanelHandler.SetDefaultParent(monetResourceParent);
        _heartsResourcePanelHandler.TransferToTargetPanel(monetResourceParent, ResourcePanelMode.WithAddButton);
    }

    private void SetMonetsMode(RectTransform monetResourceParent, RectTransform heartResourceParent)
    {
        monetResourceParent.gameObject.SetActive(true);
        heartResourceParent.gameObject.SetActive(false);
        _buferMonetsDefaultParent = _monetResourcePanelHandler.ParentDefault;
        _monetResourcePanelHandler.SetDefaultParent(monetResourceParent);
        _monetResourcePanelHandler.TransferToTargetPanel(monetResourceParent, ResourcePanelMode.WithAddButton);
    }

    private void SetMonetsAndHeartsMode(RectTransform monetResourceParent, RectTransform heartResourceParent)
    {
        monetResourceParent.gameObject.SetActive(true);
        heartResourceParent.gameObject.SetActive(true);
        _buferMonetsDefaultParent = _monetResourcePanelHandler.ParentDefault;
        _buferHeartsDefaultParent = _heartsResourcePanelHandler.ParentDefault;
        _monetResourcePanelHandler.SetDefaultParent(monetResourceParent);
        _heartsResourcePanelHandler.SetDefaultParent(heartResourceParent);
        _monetResourcePanelHandler.TransferToTargetPanel(monetResourceParent, ResourcePanelMode.WithAddButton);
        _heartsResourcePanelHandler.TransferToTargetPanel(heartResourceParent, ResourcePanelMode.WithAddButton);
    }
}