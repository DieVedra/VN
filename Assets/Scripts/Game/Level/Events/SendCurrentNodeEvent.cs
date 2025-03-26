using System;
using UniRx;

public class SendCurrentNodeEvent
{
    private ReactiveCommand<BaseNode> _sendCurrentNodeEvent;

    public SendCurrentNodeEvent()
    {
        _sendCurrentNodeEvent = new ReactiveCommand<BaseNode>();
    }

    public void Dispose()
    {
        _sendCurrentNodeEvent.Dispose();
    }
    public void Subscribe(Action<BaseNode> operation)
    {
        _sendCurrentNodeEvent.Subscribe(operation.Invoke);
    }
    public void Execute(BaseNode baseNode)
    {
        _sendCurrentNodeEvent.Execute(baseNode);
    }
}