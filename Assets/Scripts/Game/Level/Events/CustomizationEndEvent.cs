using System;
using UniRx;

public class CustomizationEndEvent 
{
    private readonly ReactiveCommand<CustomizationResult> _reactiveCommand;
    private CompositeDisposable _compositeDisposable;
    public CustomizationEndEvent()
    {
        _reactiveCommand = new ReactiveCommand<CustomizationResult>();
    }
    
    public void Dispose()
    {
        _reactiveCommand.Dispose();
    }
    public void Subscribe(Action<CustomizationResult> operation)
    {
        _compositeDisposable = new CompositeDisposable();
        _reactiveCommand.Subscribe(operation.Invoke).AddTo(_compositeDisposable);
    }

    public void Execute(CustomizationResult customizationResult)
    {
        _reactiveCommand.Execute(customizationResult);
        _compositeDisposable.Clear();
    }
}