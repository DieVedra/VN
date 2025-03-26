using System;
using UniRx;
using UnityEngine;

public class ChoiceResultEvent
{
    private ReactiveCommand<int> _onChoiceEvent;
    public ChoiceResultEvent()
    {
        _onChoiceEvent = new ReactiveCommand<int>();
    }

    public void Dispose()
    {
        _onChoiceEvent.Dispose();
    }
    public void Subscribe(Action<int> operation)
    {
        _onChoiceEvent.Subscribe(index =>
        {
            operation?.Invoke(index);
        });
    }

    public void Execute(int index)
    {
        _onChoiceEvent.Execute(index);
        _onChoiceEvent.Dispose();
        _onChoiceEvent = new ReactiveCommand<int>();
    }
}