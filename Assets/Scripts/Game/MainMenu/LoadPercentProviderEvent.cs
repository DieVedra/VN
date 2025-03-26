
using System;
using UniRx;

public class LoadPercentProviderEvent
{
    private ReactiveCommand<float> _percent;


    public LoadPercentProviderEvent()
    {
        _percent = new ReactiveCommand<float>();
    }

    public void Dispose()
    {
        _percent.Dispose();
    }
    public void Subscribe(Action<float> operation)
    {
        _percent.Subscribe(_ =>
        {
            operation?.Invoke(_);
        });
    }

    public void Execute(float value)
    {
        _percent.Execute(value);
    }
}