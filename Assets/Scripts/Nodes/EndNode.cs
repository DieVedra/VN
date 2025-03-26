
using Cysharp.Threading.Tasks;
using UnityEngine;

public class EndNode : BaseNode
{
    public override UniTask Enter(bool isMerged = false)
    {
        SwitchToNextNodeEvent.Dispose();
        Debug.Log($"End");
        return base.Enter(isMerged);
    }
}