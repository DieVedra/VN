using System;
using UniRx;

public class SwitchToAnotherNodeGraphEvent
{
    private ReactiveCommand<LevelPartNodeGraph> _reactiveCommand;

    public SwitchToAnotherNodeGraphEvent()
    {
        _reactiveCommand = new ReactiveCommand<LevelPartNodeGraph>();
    }
    public void Subscribe(Action<LevelPartNodeGraph> operation)
    {
        _reactiveCommand.Subscribe(_ =>
        {
            operation.Invoke(_);
        });
    }
    public void Execute(LevelPartNodeGraph nodeGraph)
    {
        _reactiveCommand.Execute(nodeGraph);
    }
}