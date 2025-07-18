using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;

public class LevelResourceHandler
{
    private readonly LevelResourceHandlerValues _levelResourceHandlerValues;
    private readonly ResourcePanelWithCanvasGroupView _monetResourcePanelWithCanvasGroupView;
    private readonly ResourcePanelWithCanvasGroupView _heartsResourcePanelWithCanvasGroupView;
    private bool _heartsIsShow;
    public LevelResourceHandler(ResourcePanelWithCanvasGroupView monetResourcePanelWithCanvasGroupView, ResourcePanelWithCanvasGroupView heartsResourcePanelWithCanvasGroupView)
    {
        _monetResourcePanelWithCanvasGroupView = monetResourcePanelWithCanvasGroupView;
        _heartsResourcePanelWithCanvasGroupView = heartsResourcePanelWithCanvasGroupView;
        _levelResourceHandlerValues = new LevelResourceHandlerValues(monetResourcePanelWithCanvasGroupView.RectTransform, heartsResourcePanelWithCanvasGroupView.RectTransform);
        _heartsIsShow = false;
    }

    public void TryShow(SwitchInfo[] switchInfos)
    {
        int allPrice = 0;
        int allPriceAdditional = 0;
        foreach (var switchInfo in switchInfos)
        {
            allPrice += switchInfo.Price;
            allPriceAdditional += switchInfo.AdditionalPrice;
        }
        if (allPrice > 0 && allPriceAdditional > 0)
        {
            // SetMonetAndHeartsMode(allPrice, allPriceAdditional);
            SetMonetsAndHeartsMode();
        }
        else if (allPrice > 0)
        {
            // SetMonetMode(allPrice);
            SetMonetsMode();
        }
        else if(allPriceAdditional > 0)
        {
            // SetHeartsMode(additionalPrice);
            SetHeartsMode();
        }
        else
        {
            OffAll();
        }
    }

    private void OffAll()
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

        rectTransform.anchorMin = _levelResourceHandlerValues.MonetAnchorsMin;
        rectTransform.anchorMax = _levelResourceHandlerValues.MonetAnchorsMax;

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