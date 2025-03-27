using System;
using UniRx;
using UnityEngine;

public class ChoiceResultEvent<T> : BaseEvent<T>
{
    public override void Execute(T index)
    {
        BaseReactiveCommand.Execute(index);
        BaseReactiveCommand.Dispose();
        BaseReactiveCommand = new ReactiveCommand<T>();
    }
}