
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

public class NewTaskRunner
{
    private readonly Queue<(List<Func<UniTask>>, CancellationTokenSource)> _operationsQueue = new Queue<(List<Func<UniTask>>, CancellationTokenSource)>();
    private readonly List<List<Func<UniTask>>> _operations = new List<List<Func<UniTask>>>();
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
    protected async UniTask TryRunTasksWhenAll(List<Func<UniTask>> tasksEntered)
    {
        if (tasksEntered != null && tasksEntered.Count > 0)
        {
            switch (tasksEntered.Count)
            {
                case 1:
                    await UniTask.WhenAll(tasksEntered[0].Invoke());
                    break;
                case 2:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke());
                    break;
                case 3:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke());
                    break;
                case 4:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke());
                    break;
                case 5:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke());
                    break;
                case 6:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke());
                    break;
                case 7:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke());
                    break;
                case 8:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke(),tasksEntered[7].Invoke());
                    break;
                case 9:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke(),tasksEntered[7].Invoke(),tasksEntered[8].Invoke());
                    break;
                case 10:
                    await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke(),tasksEntered[7].Invoke(),tasksEntered[8].Invoke(),tasksEntered[9].Invoke());
                    break;
            }
        }
    }
}