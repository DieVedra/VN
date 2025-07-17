using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LevelResourceHandler
{
    private const float _minValue = 0f;
    private const float _maxValue = 1f;
    private readonly ResourcePanelWithCanvasGroupView _monetResourcePanelWithCanvasGroupView;
    private readonly ResourcePanelWithCanvasGroupView _heartsResourcePanelWithCanvasGroupView;
    private bool _heartsIsShow;
    public LevelResourceHandler(ResourcePanelWithCanvasGroupView monetResourcePanelWithCanvasGroupView, ResourcePanelWithCanvasGroupView heartsResourcePanelWithCanvasGroupView)
    {
        _monetResourcePanelWithCanvasGroupView = monetResourcePanelWithCanvasGroupView;
        _heartsResourcePanelWithCanvasGroupView = heartsResourcePanelWithCanvasGroupView;
        _heartsIsShow = false;
    }

    public void OffAll()
    {
        _monetResourcePanelWithCanvasGroupView.gameObject.SetActive(false);
        _heartsResourcePanelWithCanvasGroupView.gameObject.SetActive(false);
    }

    public void SetMonet(string text)
    {
        _monetResourcePanelWithCanvasGroupView.Text.text = text;
    }

    public void SetHearts(string text)
    {
        _heartsResourcePanelWithCanvasGroupView.Text.text = text;
    }

    public async UniTask AnimShowPanelMonets(CancellationToken cancellationToken, float duration)
    {
        await DoAnimPanel(_monetResourcePanelWithCanvasGroupView, cancellationToken, duration, _minValue, _maxValue);
        Debug.Log($"ShowMonetPanelAnim");
    }

    public async UniTask AnimHidePanelMonets(CancellationToken cancellationToken, float duration)
    {
        await DoAnimPanel(_monetResourcePanelWithCanvasGroupView, cancellationToken, duration, _maxValue, _minValue);
    }

    public async UniTask AnimShowPanelHearts(CancellationToken cancellationToken, float duration)
    {
        _heartsIsShow = true;
        await DoAnimPanel(_heartsResourcePanelWithCanvasGroupView, cancellationToken, duration, _minValue, _maxValue);
    }

    public async UniTask TryAnimHidePanelHearts(CancellationToken cancellationToken, float duration)
    {
        if (_heartsIsShow == true)
        {
            await DoAnimPanel(_heartsResourcePanelWithCanvasGroupView, cancellationToken, duration, _maxValue, _minValue);
            _heartsIsShow = false;
        }
    }

    public void ShowMonetPanel()
    {
        DoPanel(_monetResourcePanelWithCanvasGroupView, _maxValue, true);
        Debug.Log($"ShowMonetPane");
    }

    public void ShowHeartsPanel()
    {
        DoPanel(_heartsResourcePanelWithCanvasGroupView, _maxValue, true);
    }

    public void HideMonetPanel()
    {
        DoPanel(_monetResourcePanelWithCanvasGroupView, _minValue, false);
    }

    public void HideHeartsPanel()
    {
        DoPanel(_heartsResourcePanelWithCanvasGroupView, _minValue, false);
    }

    private void DoPanel(ResourcePanelWithCanvasGroupView view, float alpha, bool key)
    {
        view.CanvasGroup.alpha = alpha;
        view.gameObject.SetActive(key);
    }
    private async UniTask DoAnimPanel(ResourcePanelWithCanvasGroupView view, CancellationToken cancellationToken, float duration, float alpha, float endValue)
    {
        view.CanvasGroup.alpha = alpha;
        view.gameObject.SetActive(false);
        await view.CanvasGroup.DOFade(endValue, duration)
            .WithCancellation(cancellationToken);
    }
}