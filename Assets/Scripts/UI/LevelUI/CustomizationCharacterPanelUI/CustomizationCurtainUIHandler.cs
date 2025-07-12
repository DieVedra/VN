using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CustomizationCurtainUIHandler : CurtainUIHandler
{
    public CustomizationCurtainUIHandler(BlackFrameView blackFrameView, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI = null)
        : base(blackFrameView, blockGameControlPanelUI){}
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