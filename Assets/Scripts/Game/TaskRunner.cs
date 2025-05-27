using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class TaskRunner
{
    public const int TaskCountMax = 10;
    private List<Func<UniTask>> _operation;
    
    public void AddOperationToList(Func<UniTask> operation)
    {
        if (_operation == null)
        {
            _operation = new List<Func<UniTask>>();
        }
        if (_operation.Count < TaskCountMax)
        {
            _operation.Add(operation);
        }
    }

    public async UniTask TryRunTasks()
    {
        await TryRunTasks(_operation);
        _operation = null;
    }

    public async UniTask TryRunTasks(List<Func<UniTask>> tasksEntered)
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