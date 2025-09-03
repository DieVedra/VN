using System;
using UniRx;

public class BaseEvent
{
    protected ReactiveCommand BaseReactiveCommand;
    private readonly CompositeDisposable CompositeDisposable;
    public BaseEvent()
    {
        CompositeDisposable = new CompositeDisposable();
        BaseReactiveCommand = new ReactiveCommand().AddTo(CompositeDisposable);
    }
    public void Dispose()
    {
        CompositeDisposable.Clear();
    }
    public void Subscribe(Action operation)
    {
        BaseReactiveCommand.Subscribe(_ =>
        {
            operation();
        });
    }
    public CompositeDisposable SubscribeWithCompositeDisposable(Action operation)
    {
        var compositeDisposable = new CompositeDisposable();
        BaseReactiveCommand.Subscribe(_ =>
        {
            operation();
        }).AddTo(compositeDisposable);
        return compositeDisposable;
    }
    public virtual void Execute()
    {
        BaseReactiveCommand.Execute();
    }
}

public class BaseEvent<T>
{
    protected ReactiveCommand<T> BaseReactiveCommand;
    public BaseEvent()
    {
        BaseReactiveCommand = new ReactiveCommand<T>();
    }
    public void Dispose()
    {
        BaseReactiveCommand.Dispose();
    }
    public virtual void Subscribe(Action<T> operation)
    {
        BaseReactiveCommand.Subscribe(_ =>
        {
            operation.Invoke(_);
        });
    }
    public virtual void Execute(T somebody)
    {
        BaseReactiveCommand.Execute(somebody);
    }
}