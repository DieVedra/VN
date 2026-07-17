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
    public readonly Image CurtainImage;
    public readonly Transform Transform;
    private ReactiveProperty<bool> _fromSave;
    public CurtainUIHandler(BlackFrameView blackFrameView, ReactiveProperty<bool> fromSave, BlockGameControlPanelUIEvent<bool> blockGameControlPanelUI = null)
    {
        Transform = blackFrameView.transform;
        BlackFrameView = blackFrameView;
        BlockGameControlPanelUI = blockGameControlPanelUI;
        CurtainImage = BlackFrameView.Image;
        _fromSave = fromSave;
    }
    
    
    public virtual async UniTask CurtainOpens(CancellationToken cancellationToken)
    {
        if (_fromSave.Value == false)
        {
            BlockGameControlPanelUI?.Execute(false);
            BlackFrameView.gameObject.SetActive(true);
            SkipAtCloses();
            await CurtainImage.DOFade(AnimationValuesProvider.MinValue, AnimationValuesProvider.MaxValue).WithCancellation(cancellationToken);
            BlackFrameView.gameObject.SetActive(false);
        }
        else
        {
            _fromSave.Value = false;
        }
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