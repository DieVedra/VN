using System.Linq;
using Cysharp.Threading.Tasks;

public class PhoneSwitchNode : SwitchNode
{
    public bool IsOver { get; private set; }

    public void ConstructMyPhoneSwitchNode(IGameStatsProvider gameStatsProvider, int seriaIndex)
    {
        IsOver = false;
        ConstructMySwitchNode(gameStatsProvider, seriaIndex);
    }
    public override UniTask Enter(bool isMerged = false)
    {
        var result = SwitchNodeLogic.GetPortIndexOnSwitchResult(_casesForStats);
        bool caseFoundSuccessfuly = result.Item1;
        int indexCase = result.Item2;
        if (caseFoundSuccessfuly == true)
        {
            SetNextNode(DynamicOutputs.ElementAt(indexCase).Connection.node as BaseNode);
        }
        else
        {
            TryFindDefaultNextNodeAndSet();
        }
        IsOver = true;
        return default;
    }
}