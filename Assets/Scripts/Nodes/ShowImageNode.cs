using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#515000")]

public class ShowImageNode : BaseNode
{
    [SerializeField] private Sprite _sprite;

    private Background _background;
    
    public void Construct(Background background)
    {
        _background = background;
    }
    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        if (isMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }
        await _background.ShowImageInPlayMode(_sprite, CancellationTokenSource.Token);
        TryActivateButtonSwitchToNextSlide();
    }

    protected override void SetInfoToView()
    {
        _background.ShowImage(_sprite);
    }

    public override void SkipEnterTransition()
    {
        CancellationTokenSource.Cancel();
        SetInfoToView();
        TryActivateButtonSwitchToNextSlide();
    }
    protected override void TryActivateButtonSwitchToNextSlide()
    {
        if (IsMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
        }
    }
}