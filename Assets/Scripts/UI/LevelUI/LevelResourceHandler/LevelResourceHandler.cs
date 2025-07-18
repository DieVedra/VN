using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LevelResourceHandler
{
    private readonly LevelResourceHandlerValues _levelResourceHandlerValues = new LevelResourceHandlerValues();
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
        await DoAnimPanel(_monetResourcePanelWithCanvasGroupView, cancellationToken, duration, LevelResourceHandlerValues.MinValue, LevelResourceHandlerValues.MaxValue);
        Debug.Log($"ShowMonetPanelAnim");
    }

    public async UniTask AnimHidePanelMonets(CancellationToken cancellationToken, float duration)
    {
        await DoAnimPanel(_monetResourcePanelWithCanvasGroupView, cancellationToken, duration, LevelResourceHandlerValues.MaxValue, LevelResourceHandlerValues.MinValue);
    }

    public async UniTask AnimShowPanelHearts(CancellationToken cancellationToken, float duration)
    {
        _heartsIsShow = true;
        await DoAnimPanel(_heartsResourcePanelWithCanvasGroupView, cancellationToken, duration, LevelResourceHandlerValues.MinValue, LevelResourceHandlerValues.MaxValue);
    }

    public async UniTask TryAnimHidePanelHearts(CancellationToken cancellationToken, float duration)
    {
        if (_heartsIsShow == true)
        {
            await DoAnimPanel(_heartsResourcePanelWithCanvasGroupView, cancellationToken, duration, LevelResourceHandlerValues.MaxValue, LevelResourceHandlerValues.MinValue);
            _heartsIsShow = false;
        }
    }

    public void ShowMonetPanel()
    {
        DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MaxValue, true);
        Debug.Log($"ShowMonetPane");
    }

    public void ShowHeartsPanel()
    {
        DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MaxValue, true);
    }

    public void HideMonetPanel()
    {
        DoPanel(_monetResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
    }

    public void HideHeartsPanel()
    {
        DoPanel(_heartsResourcePanelWithCanvasGroupView, LevelResourceHandlerValues.MinValue, false);
    }

    public void SetHeartsMode()
    {
        var rectTransform = _heartsResourcePanelWithCanvasGroupView.RectTransform;

        rectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin; // (0, 0)
        rectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;

// Задаём отступы: left=50, right=30, top=20, bottom=10
        // rectTransform.offsetMin = new Vector2(50, 10);  // left, bottom
        // rectTransform.offsetMax = new Vector2(-30, -20); // -right, -top
        rectTransform.offsetMin = _levelResourceHandlerValues.MonetOffsetMin;
        rectTransform.offsetMax = _levelResourceHandlerValues.MonetOffsetMax;
    }

    public void SetMonetsMode()
    {
        var monetsRectTransform = _monetResourcePanelWithCanvasGroupView.RectTransform;
        monetsRectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin;
        monetsRectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;
        monetsRectTransform.offsetMin = _levelResourceHandlerValues.MonetOffsetMin;
        monetsRectTransform.offsetMax = _levelResourceHandlerValues.MonetOffsetMax;
    }

    public void SetMonetsAndHeartsMode()
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
}