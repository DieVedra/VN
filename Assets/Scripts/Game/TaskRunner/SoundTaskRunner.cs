using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class SoundTaskRunner : TaskRunner
{
    private readonly Queue<(List<Func<UniTask>>, CancellationTokenSource)> _operationsQueue = new Queue<(List<Func<UniTask>>, CancellationTokenSource)>();
    private readonly List<List<Func<UniTask>>> _operations = new List<List<Func<UniTask>>>();

    public List<Func<UniTask>> GetFreeList()
    {
        bool result = false;
        List<Func<UniTask>> toReturn = null;
        foreach (var list in _operations)
        {
            if (list.Count == 0)
            {
                toReturn = list;
                result = true;
                break;
            }
        }
        if (result == false)
        {
            toReturn = new List<Func<UniTask>>();
            _operations.Add(toReturn);
        }
        return toReturn;
    }
    public void AddToQueue(List<Func<UniTask>> operations, CancellationTokenSource cancellationToken)
    {
        _operationsQueue.Enqueue((operations, cancellationToken));
    }

    public async UniTask TryRunSound()
    {
        if (IsRun == false)
        {
            IsRun = true;
            while (true)
            {
                if (_operationsQueue.Count > 0)
                {
                    (List<Func<UniTask>>, CancellationTokenSource) nextOperations = _operationsQueue.Dequeue();
                    await TryRunTasksWhenAll(nextOperations.Item1);
                    nextOperations.Item1.Clear();
                }
                else
                {
                    break;
                }
            }
            IsRun = false;
        }
    }

    public void ForceStop()
    {
        foreach (var queue in _operationsQueue)
        {
            queue.Item2.Cancel();
            queue.Item1.Clear();
        }
        _operationsQueue.Clear();
    }
}