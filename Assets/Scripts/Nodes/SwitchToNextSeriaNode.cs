
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#FF0404")]
public class SwitchToNextSeriaNode: BaseNode, IPutOnSwimsuit
{
    [SerializeField] private bool _putOnSwimsuit;
    private SwitchToNextSeriaEvent<bool> _switchToNextSeriaEvent;

    public void Construct(SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent)
    {
        _switchToNextSeriaEvent = switchToNextSeriaEvent;
    }

    public override UniTask Enter(bool isMerged = false)
    {
        _switchToNextSeriaEvent.Execute(_putOnSwimsuit);
        // return base.Enter(isMerged);
        return default;
    }

    public void PutOnSwimsuit(bool putOnSwimsuitKey)
    {
        
    }
}