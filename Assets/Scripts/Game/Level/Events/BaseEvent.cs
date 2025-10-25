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
    public void SubscribeWithCompositeDisposable(Action operation, CompositeDisposable compositeDisposable)
    {
        BaseReactiveCommand.Subscribe(_ =>
        {
            operation();
        }).AddTo(compositeDisposable);
    }
    public virtual void Execute()
    {
        BaseReactiveCommand.Execute();
    }
}

public class BaseEvent<T>
{
    protected ReactiveCommand<T> BaseReactiveCommand;
    private readonly CompositeDisposable CompositeDisposable;

    public BaseEvent()
    {
        CompositeDisposable = new CompositeDisposable();
        BaseReactiveCommand = new ReactiveCommand<T>().AddTo(CompositeDisposable);
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
    public CompositeDisposable SubscribeWithCompositeDisposable(Action operation)
    {
        var compositeDisposable = new CompositeDisposable();
        BaseReactiveCommand.Subscribe(_ =>
        {
            operation();
        }).AddTo(compositeDisposable);
        return compositeDisposable;
    }
    public void SubscribeWithCompositeDisposable(Action operation, CompositeDisposable compositeDisposable)
    {
        BaseReactiveCommand.Subscribe(_ =>
        {
            operation();
        }).AddTo(compositeDisposable);
    }
    public void SubscribeWithCompositeDisposable(Action<T> operation, CompositeDisposable compositeDisposable)
    {
        BaseReactiveCommand.Subscribe(operation).AddTo(compositeDisposable);
    }
    public CompositeDisposable SubscribeWithCompositeDisposable(Action<T> operation)
    {
        var compositeDisposable = new CompositeDisposable();
        BaseReactiveCommand.Subscribe(operation.Invoke).AddTo(compositeDisposable);
        return compositeDisposable;
    }
    
    public virtual void Execute(T somebody)
    {
        BaseReactiveCommand.Execute(somebody);
    }
}