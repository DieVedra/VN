
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class CurtainUIHandler
{
    private const float _unfadeSkipValue = 0.2f;
    protected readonly BlackFrameView BlackFrameView;
    protected readonly BlockGameControlPanelUIEvent<bool> BlockGameControlPanelUI;
    protected readonly Image CurtainImage;
    public CurtainUIHandler(BlackFrameView blackFrameView, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI)
    {
        BlackFrameView = blackFrameView;
        BlockGameControlPanelUI = blockGameControlPanelUI;
        CurtainImage = BlackFrameView.Image;
        if (Application.isPlaying == true)
        {
            BlackFrameView.gameObject.SetActive(true);
            SkipAtCloses();
        }
        else
        {
            BlackFrameView.gameObject.SetActive(false);
        }
    }
    public virtual async UniTask CurtainOpens(CancellationToken cancellationToken)
    {
        BlockGameControlPanelUI.Execute(false);
        BlackFrameView.gameObject.SetActive(true);
        BlackFrameView.Image.color = Color.black;
        await UniTask.WhenAny(CurtainImage.DOFade(AnimationValuesProvider.MinValue, AnimationValuesProvider.MaxValue).WithCancellation(cancellationToken),
            UniTask.Delay(TimeSpan.FromSeconds(AnimationValuesProvider.MaxValue - _unfadeSkipValue), cancellationToken: cancellationToken));
        BlackFrameView.gameObject.SetActive(false);
    }

    public virtual async UniTask CurtainCloses(CancellationToken cancellationToken)
    {
        BlockGameControlPanelUI.Execute(true);
        BlackFrameView.gameObject.SetActive(true);
        BlackFrameView.Image.color = Color.clear;
        await CurtainImage.DOFade(AnimationValuesProvider.MaxValue, AnimationValuesProvider.MaxValue).WithCancellation(cancellationToken);
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