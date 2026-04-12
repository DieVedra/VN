
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class NewTaskRunner
{
    private const int _taskCountMax = 10;
    private readonly Queue<(List<Func<UniTask>>, CancellationTokenSource)> _operationsQueue = new Queue<(List<Func<UniTask>>, CancellationTokenSource)>();
    private readonly List<List<Func<UniTask>>> _operations = new List<List<Func<UniTask>>>();
    private readonly List<UniTask> _tasksEntered = new List<UniTask>();
    public bool IsRun { get; protected set; }

    public List<Func<UniTask>> GetFreeList()
    {
        foreach (var list in _operations)
        {
            if (list.Count == 0)
            {
                return list;
            }
        }

        List<Func<UniTask>> toReturn = new List<Func<UniTask>>();
        _operations.Add(toReturn);
        return toReturn;
    }
    public void AddToQueue(List<Func<UniTask>> operations, CancellationTokenSource cancellationToken = null)
    {
        _operationsQueue.Enqueue((operations, cancellationToken));
    }
    public async UniTask TryRun()
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
            queue.Item2?.Cancel();
            queue.Item1.Clear();
        }
        _operationsQueue.Clear();
    }

    private async UniTask TryRunTasksWhenAll(List<Func<UniTask>> tasksEntered)
    {
        if (tasksEntered != null && tasksEntered.Count > 0)
        {
            _tasksEntered.Clear();
            foreach (var task in tasksEntered)
            {
                _tasksEntered.Add(task.Invoke());
            }
            await UniTask.WhenAll(_tasksEntered);
            _tasksEntered.Clear();
        }
    }
}