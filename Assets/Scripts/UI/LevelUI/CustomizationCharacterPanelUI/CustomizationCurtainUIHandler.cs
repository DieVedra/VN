using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CustomizationCurtainUIHandler : CurtainUIHandler
{
    private const float _fadeEndValue = 0.3f;
    private int _blackoutFrameSiblingIndexBufer;
    private int _infoPanelSiblingIndexBufer;
    private Color _colorHide = new Color(_fadeEndValue,_fadeEndValue,_fadeEndValue,_fadeEndValue);
    private RectTransform _infoPanel;
    public CustomizationCurtainUIHandler(BlackFrameView blackFrameView, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI = null)
        : base(blackFrameView, blockGameControlPanelUI){}

    public void SetCurtainUnderTargetPanel(RectTransform infoPanel, int targetSiblingIndex, bool raycastTarget = false)
    {
        _infoPanel = infoPanel;
        _blackoutFrameSiblingIndexBufer = Transform.GetSiblingIndex();
        _infoPanelSiblingIndexBufer = _infoPanel.GetSiblingIndex();
        Transform.SetSiblingIndex(targetSiblingIndex);
        _infoPanel.SetSiblingIndex(Transform.GetSiblingIndex());
        CurtainImage.raycastTarget = raycastTarget;
        CurtainImage.color = _colorHide;
        CurtainImage.gameObject.SetActive(true);

    }
    public void SetCurtainToDefaultSibling()
    {
        Transform.SetSiblingIndex(_blackoutFrameSiblingIndexBufer);
        _infoPanel.SetSiblingIndex(_infoPanelSiblingIndexBufer);
        CurtainImage.raycastTarget = true;
        CurtainImage.gameObject.SetActive(false);
        _infoPanel = null;
    }
    
    public override async UniTask CurtainOpens(CancellationToken cancellationToken)
    {
        BlockGameControlPanelUI?.Execute(false);
        BlackFrameView.gameObject.SetActive(true);
        BlackFrameView.Image.color = Color.black;
        BlackFrameView.Image.raycastTarget = false;
        await BlackFrameView.Image.DOFade(AnimationValuesProvider.MinValue, GetActualityDurationCurtainOpens()).WithCancellation(cancellationToken);
        BlackFrameView.gameObject.SetActive(false);
    }

    public override async UniTask CurtainCloses(CancellationToken cancellationToken)
    {
        BlockGameControlPanelUI?.Execute(true);
        BlackFrameView.gameObject.SetActive(true);
        BlackFrameView.Image.color = Color.clear;
        BlackFrameView.Image.raycastTarget = true;
        await BlackFrameView.Image.DOFade(AnimationValuesProvider.MaxValue, GetActualityDurationCurtainCloses()).WithCancellation(cancellationToken);
    }
    private float GetActualityDurationCurtainOpens()
    {
        return Mathf.Lerp(AnimationValuesProvider.MinValue, AnimationValuesProvider.MaxValue, BlackFrameView.Image.color.a);
    }
    private float GetActualityDurationCurtainCloses()
    {
        return Mathf.Lerp(AnimationValuesProvider.MaxValue, AnimationValuesProvider.MinValue,BlackFrameView.Image.color.a);
    }
}