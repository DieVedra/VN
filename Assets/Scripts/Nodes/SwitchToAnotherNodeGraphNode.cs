using Cysharp.Threading.Tasks;
using UnityEngine;

public class SwitchToAnotherNodeGraphNode : BaseNode, IPutOnSwimsuit
{
    [SerializeField] private SeriaPartNodeGraph _seriaPartNodeGraph;
    [SerializeField] private bool _putOnSwimsuit;
    
    private SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> _switchToAnotherNodeGraphEvent;
    public void ConstructSwitchToAnotherNodeGraphNode(SwitchToAnotherNodeGraphEvent<SeriaPartNodeGraph> switchToAnotherNodeGraphEvent)
    {
        _switchToAnotherNodeGraphEvent = switchToAnotherNodeGraphEvent;
    }
    public override UniTask Enter(bool isMerged = false)
    {
        _seriaPartNodeGraph.TryPutOnSwimsuit(_putOnSwimsuit);
        _switchToAnotherNodeGraphEvent.Execute(_seriaPartNodeGraph);
        return default;
    }

    public void PutOnSwimsuit(bool putOnSwimsuitKey)
    {
        _putOnSwimsuit = putOnSwimsuitKey;
    }
}