using System;
using UniRx;

public class CustomizationEndEvent<T> : BaseEvent<T>
{
    private CompositeDisposable _compositeDisposable;
    public override void Subscribe(Action<T> operation)
    {
        _compositeDisposable = new CompositeDisposable();
        BaseReactiveCommand.Subscribe(_=>
        {
            operation.Invoke(_);
        }).AddTo(_compositeDisposable);
    }

    public override void Execute(T customizationResult)
    {
        BaseReactiveCommand.Execute(customizationResult);
        _compositeDisposable.Clear();
    }
}