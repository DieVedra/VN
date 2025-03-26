using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#333333")]
public class SmoothTransitionNode : BaseNode
{
    [SerializeField, HideInInspector] private bool _isStartCurtain;
    [SerializeField, HideInInspector] private bool _isEndCurtain;

    private CurtainUIHandler _curtainUIHandler;

    public void ConstructMySmoothTransitionNode(CurtainUIHandler curtainUIHandler)
    {
        _curtainUIHandler = curtainUIHandler;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        IsMerged = isMerged;
        if (isMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }

        if (_isStartCurtain)
        {
            await _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
        }
        
        if(_isEndCurtain)
        {
            await _curtainUIHandler.CurtainCloses(CancellationTokenSource.Token);
        }

        if (isMerged == false)
        {
            SwitchToNextNodeEvent.Execute();
        }
    }

    public override void SkipEnterTransition()
    {
        CancellationTokenSource.Cancel();
        if (_isStartCurtain)
        {
            _curtainUIHandler.SkipAtOpens();
        }
        if (_isEndCurtain)
        {
            _curtainUIHandler.SkipAtCloses();
        }
    }
}