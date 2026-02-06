using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CurtainUIHandler
{
    private const float _unfadeSkipValue = 0.2f;
    protected readonly BlackFrameView BlackFrameView;
    protected readonly BlockGameControlPanelUIEvent<bool> BlockGameControlPanelUI;
    public readonly Image CurtainImage;
    public readonly Transform Transform;
    public CurtainUIHandler(BlackFrameView blackFrameView, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI = null)
    {
        Transform = blackFrameView.transform;
        BlackFrameView = blackFrameView;
        BlockGameControlPanelUI = blockGameControlPanelUI;
        CurtainImage = BlackFrameView.Image;
    }
    
    
    public virtual async UniTask CurtainOpens(CancellationToken cancellationToken)
    {
        BlockGameControlPanelUI?.Execute(false);
        BlackFrameView.gameObject.SetActive(true);
        SkipAtCloses();
        await CurtainImage.DOFade(AnimationValuesProvider.MinValue, AnimationValuesProvider.MaxValue).WithCancellation(cancellationToken);
        BlackFrameView.gameObject.SetActive(false);
    }

    public virtual async UniTask CurtainCloses(CancellationToken cancellationToken)
    {
        CurtainClosesImmediate();
        await CurtainImage.DOFade(AnimationValuesProvider.MaxValue, AnimationValuesProvider.MaxValue).WithCancellation(cancellationToken);
    }

    public void CurtainOpensImmediate()
    {
        BlockGameControlPanelUI?.Execute(false);
        BlackFrameView.gameObject.SetActive(true);
        SkipAtCloses();
        BlackFrameView.gameObject.SetActive(false);
    }
    public void CurtainClosesImmediate()
    {
        BlockGameControlPanelUI?.Execute(true);
        BlackFrameView.gameObject.SetActive(true);
        SkipAtOpens();
    }
    public void SkipAtOpens()
    {
        BlackFrameView.Image.color = Color.clear;
    }
    public void SkipAtCloses()
    {
        BlackFrameView.Image.color = Color.black;
    }
}