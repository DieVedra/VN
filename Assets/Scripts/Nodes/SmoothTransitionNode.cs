﻿using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#333333")]
public class SmoothTransitionNode : BaseNode
{
    [SerializeField, HideInInspector] private bool _isStartImmediatlyCurtain;
    [SerializeField, HideInInspector] private bool _isEndImmediatlyCurtain;
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

        await Do();
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

    private async UniTask Do()
    {
        if (_isStartCurtain)
        {
            await _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
            return;
        }
        if (_isStartImmediatlyCurtain)
        {
            _curtainUIHandler.CurtainOpensImmediate();
            return;
        }
        if(_isEndCurtain)
        {
            await _curtainUIHandler.CurtainCloses(CancellationTokenSource.Token);
            return;
        }
        if(_isEndImmediatlyCurtain)
        {
            _curtainUIHandler.CurtainClosesImmediate();
        }
    }
}