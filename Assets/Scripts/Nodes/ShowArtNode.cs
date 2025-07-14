using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#515000"), NodeWidth(350)]

public class ShowArtNode : BaseNode
{
    [SerializeField] private int _spriteIndex;

    private Background _background;
    public IReadOnlyList<Sprite> Arts => _background.GetArtsSprites;

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
        await _background.ShowImageInPlayMode(_spriteIndex, CancellationTokenSource.Token);
        TryActivateButtonSwitchToNextSlide();
    }

    protected override void SetInfoToView()
    {
        _background.ShowArtImage(_spriteIndex);
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