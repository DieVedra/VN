using System;
using UniRx;
using UnityEngine;

public class BaseEvent
{
    protected ReactiveCommand BaseReactiveCommand;

    public BaseEvent()
    {
        BaseReactiveCommand = new ReactiveCommand();
    }

    public void Dispose()
    {
        BaseReactiveCommand.Dispose();
    }
    public void Subscribe(Action operation)
    {
        BaseReactiveCommand.Subscribe(_ =>
        {
            operation();
        });
    }
    public virtual void Execute()
    {
        BaseReactiveCommand.Execute();
    }
}