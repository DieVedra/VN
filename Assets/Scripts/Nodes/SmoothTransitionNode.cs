using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#333333")]
public class SmoothTransitionNode : BaseNode
{
    [SerializeField, HideInInspector] private bool _isStartImmediatlyCurtain;
    [SerializeField, HideInInspector] private bool _isEndImmediatlyCurtain;
    [SerializeField, HideInInspector] private bool _isStartCurtain;
    [SerializeField, HideInInspector] private bool _isEndCurtain;
    [SerializeField, HideInInspector] private bool _customDelay;
    [SerializeField] private float _delay;

    private const float _defaultDelay = 0.7f;
    private CurtainUIHandler _curtainUIHandler;

    public void ConstructMySmoothTransitionNode(CurtainUIHandler curtainUIHandler)
    {
        _curtainUIHandler = curtainUIHandler;
        if (_delay == 0)
        {
            _delay = _defaultDelay;
        }
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        Debug.Log($"SmoothTransitionNode1  {_delay}");
        IsMerged = isMerged;
        if (isMerged == false)
        {
            ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
        }
        await Do();
        Debug.Log($"SmoothTransitionNode2");
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
            if (_customDelay == true)
            {
                _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: CancellationTokenSource.Token);

            }
            else
            {
                await _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
            }
            return;
        }
        if (_isStartImmediatlyCurtain)
        {
            _curtainUIHandler.CurtainOpensImmediate();
            return;
        }
        if(_isEndCurtain)
        {
            if (_customDelay == true)
            {
                _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token).Forget();
                await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: CancellationTokenSource.Token);

            }
            else
            {
                await _curtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
            }
            return;
        }
        if(_isEndImmediatlyCurtain)
        {
            _curtainUIHandler.CurtainClosesImmediate();
        }
    }

    private async UniTask WithDelayCheck(Func<UniTask> operation)
    {
        if (_customDelay == true)
        {
            Debug.Log($"WithDelayCheck");
            operation.Invoke().Forget();
            await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: CancellationTokenSource.Token);
        }
        else
        {
            await operation.Invoke();
        }
    }
}