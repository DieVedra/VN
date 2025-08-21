using System;
using UniRx;

public class BasePropertyEvent<T>
{
    protected ReactiveProperty<T> BaseReactiveProperty;
    public T GetValue => BaseReactiveProperty.Value;

    public BasePropertyEvent()
    {
        BaseReactiveProperty = new ReactiveProperty<T>();
    }
    public void Dispose()
    {
        BaseReactiveProperty.Dispose();
    }
    public CompositeDisposable SubscribeWithCompositeDisposable(Action<T> operation)
    {
        CompositeDisposable compositeDisposable = new CompositeDisposable();
        BaseReactiveProperty.Subscribe(_ =>
        {
            operation.Invoke(_);
        }).AddTo(compositeDisposable);
        return compositeDisposable;
    }
    public void Subscribe(Action<T> operation)
    {
        BaseReactiveProperty.Subscribe(_ =>
        {
            operation.Invoke(_);
        });
    }
    public void SetValue(T value)
    {
        BaseReactiveProperty.Value = value;
    }
}