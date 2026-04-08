using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class TaskRunner
{
    protected const int TaskCountMax = 10;
    protected List<Func<UniTask>> Operations;
    public bool IsRun { get; protected set; }
    public bool WaitToBeRunned { get; protected set; }
    
    public void AddOperationToList(Func<UniTask> operation)
    {
        WaitToBeRunned = true;
        if (IsRun == false)
        {
            if (Operations == null)
            {
                Operations = new List<Func<UniTask>>();
            }
            if (Operations.Count < TaskCountMax)
            {
                Operations.Add(operation);
            }
        }
    }

    public async UniTask TryRunTasksWhenAll()
    {
        if (IsRun == false)
        {
            IsRun = true;
            await TryRunTasksWhenAll(Operations);
            Operations?.Clear();
            IsRun = false;
            WaitToBeRunned = false;
        }
    }
    public async UniTask TryRunTasksWhenAny()
    {
        if (IsRun == false)
        {
            IsRun = true;
            await TryRunTasksWhenAny(Operations);
            Operations?.Clear();
            IsRun = false;
            WaitToBeRunned = false;
        }
    }

    public async UniTask TryRunTasksWhenAll(List<Func<UniTask>> tasksEntered)
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

    public async UniTask TryRunTasksWhenAny(List<Func<UniTask>> tasksEntered)
    {
        if (tasksEntered != null && tasksEntered.Count > 0)
        {
            switch (tasksEntered.Count)
            {
                case 1:
                    await UniTask.WhenAny(tasksEntered[0].Invoke());
                    break;
                case 2:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke());
                    break;
                case 3:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke());
                    break;
                case 4:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke());
                    break;
                case 5:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke());
                    break;
                case 6:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke());
                    break;
                case 7:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke());
                    break;
                case 8:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke(),tasksEntered[7].Invoke());
                    break;
                case 9:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke(),tasksEntered[7].Invoke(),tasksEntered[8].Invoke());
                    break;
                case 10:
                    await UniTask.WhenAny(tasksEntered[0].Invoke(),tasksEntered[1].Invoke(),tasksEntered[2].Invoke(),tasksEntered[3].Invoke(),
                        tasksEntered[4].Invoke(),tasksEntered[5].Invoke(),tasksEntered[6].Invoke(),tasksEntered[7].Invoke(),tasksEntered[8].Invoke(),tasksEntered[9].Invoke());
                    break;
            }
        }
    }
}