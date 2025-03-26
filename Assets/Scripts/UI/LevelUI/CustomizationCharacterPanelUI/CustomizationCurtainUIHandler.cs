
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class CustomizationCurtainUIHandler : CurtainUIHandler
{
    public CustomizationCurtainUIHandler(CurtainUI curtainUI) : base(curtainUI){}
    public override async UniTask CurtainOpens(CancellationToken cancellationToken)
    {
        CurtainUI.gameObject.SetActive(true);
        CurtainUI.Image.color = Color.black;
        CurtainUI.Image.raycastTarget = false;
        await CurtainUI.Image.DOFade(0f, GetActualityDurationCurtainOpens()).WithCancellation(cancellationToken);
        CurtainUI.gameObject.SetActive(false);
    }

    public override async UniTask CurtainCloses(CancellationToken cancellationToken)
    {
        CurtainUI.gameObject.SetActive(true);
        CurtainUI.Image.color = Color.clear;
        CurtainUI.Image.raycastTarget = true;
        await CurtainUI.Image.DOFade(1f, GetActualityDurationCurtainCloses()).WithCancellation(cancellationToken);
    }
    private float GetActualityDurationCurtainOpens()
    {
        return Mathf.Lerp(0f, CurtainUI.DurationAnim, CurtainUI.Image.color.a);
    }
    private float GetActualityDurationCurtainCloses()
    {
        return Mathf.Lerp(CurtainUI.DurationAnim, 0f,CurtainUI.Image.color.a);
    }
}