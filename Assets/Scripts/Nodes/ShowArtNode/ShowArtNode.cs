using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[NodeTint("#515000"), NodeWidth(350)]

public class ShowArtNode : BaseNode
{
    [SerializeField] private ShowArtMode _artMode;
    [SerializeField] private int _spriteIndex;
    [SerializeField] private string _spriteKey;

    private IBackgroundProviderToShowArtNode _background;
    public IReadOnlyDictionary<string, Sprite> GetArtsSpritesDictionary => _background?.GetArtsSpritesDictionary;

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
        await _background.ShowImageInPlayMode(_spriteKey, CancellationTokenSource.Token);
        TryActivateButtonSwitchToNextSlide();
    }

    public override async UniTask Exit()
    {
        CancellationTokenSource = new CancellationTokenSource();

        switch (_artMode)
        {
            case ShowArtMode.Mode2:
                await _background.HideImageInPlayMode(CancellationTokenSource.Token);
                break;
        }
    }

    protected override void SetInfoToView()
    {
        _background.ShowArtImage(_spriteKey);
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