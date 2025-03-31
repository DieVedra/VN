using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class TaskRunner
{
    private List<Func<UniTask>> _operation;

    public void AddOperationToList(Func<UniTask> operation)
    {
        if (_operation == null)
        {
            _operation = new List<Func<UniTask>>();
        }
        _operation.Add(operation);
    }

    public async UniTask TryRunTasks()
    {
        await TryRunTasks(_operation);
    }

    public async UniTask TryRunTasks(List<Func<UniTask>> tasksEntered)
    {
        switch (tasksEntered.Count)
        {
            case 1:
                await UniTask.WhenAll(tasksEntered[0].Invoke());
                _operation = null;
                break;
            case 2:
                await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke());
                _operation = null;
                break;
            case 3:
                await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke());
                _operation = null;
                break;
            case 4:
                await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke());
                _operation = null;
                break;
            case 5:
                await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                    tasksEntered[4].Invoke());
                _operation = null;
                break;
            case 6:
                await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                    tasksEntered[4].Invoke(),tasksEntered[5].Invoke());
                _operation = null;
                break;
            case 7:
                await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                    tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke());
                _operation = null;
                break;
            case 8:
                await UniTask.WhenAll(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                    tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke(),tasksEntered[7].Invoke());
                _operation = null;
                break;
        }
    }
}