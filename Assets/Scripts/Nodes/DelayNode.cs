
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class DelayNode : BaseNode
{
    [SerializeField] private float _delay;

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        await UniTask.Delay(TimeSpan.FromSeconds(_delay), cancellationToken: CancellationTokenSource.Token);
        SwitchToNextNodeEvent.Execute();
    }
}