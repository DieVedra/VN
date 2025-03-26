using Cysharp.Threading.Tasks;
using UnityEngine;

public class SwitchToAnotherNodeGraphNode : BaseNode, IPutOnSwimsuit
{
    [SerializeField] private LevelPartNodeGraph _nextLevelPartNodeGraph;
    [SerializeField] private bool _putOnSwimsuit;
    
    private SwitchToAnotherNodeGraphEvent _switchToAnotherNodeGraphEvent;
    public void ConstructSwitchToAnotherNodeGraphNode(SwitchToAnotherNodeGraphEvent switchToAnotherNodeGraphEvent)
    {
        _switchToAnotherNodeGraphEvent = switchToAnotherNodeGraphEvent;
    }
    public override UniTask Enter(bool isMerged = false)
    {
        _nextLevelPartNodeGraph.TryPutOnSwimsuit(_putOnSwimsuit);
        _switchToAnotherNodeGraphEvent.Execute(_nextLevelPartNodeGraph);
        return default;
    }

    public void PutOnSwimsuit(bool putOnSwimsuitKey)
    {
        _putOnSwimsuit = putOnSwimsuitKey;
    }
}