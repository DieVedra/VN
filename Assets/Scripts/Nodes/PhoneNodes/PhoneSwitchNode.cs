using System.Linq;
using Cysharp.Threading.Tasks;
using XNode;

public class PhoneSwitchNode : SwitchNode
{
    public PhoneMessageNode PhoneMessageNode { get; private set; }
    public bool IsOver { get; private set; }

    public void ConstructMyPhoneSwitchNode(IGameStatsProvider gameStatsProvider, int seriaIndex)
    {
        IsOver = false;
        PhoneMessageNode = null;
        ConstructMySwitchNode(gameStatsProvider, seriaIndex);
    }
    public override async UniTask Enter(bool isMerged = false)
    {
        var result = SwitchNodeLogic.GetPortIndexOnSwitchResult(_casesForStats);
        bool caseFoundSuccessfuly = result.Item1;
        int indexCase = result.Item2;
        if (caseFoundSuccessfuly == true)
        {
            TryFindConnectedPhoneMessageNode(DynamicOutputs.ElementAt(indexCase));
        }
        else
        {
            TryFindConnectedPhoneMessageNode(OutputPortBaseNode);
        }
        IsOver = true;
    }
    private void TryFindConnectedPhoneMessageNode(NodePort outputPort)
    {
        for (int i = 0; i < outputPort.GetConnections().Count; i++)
        {
            if (outputPort.GetConnection(i).node is PhoneMessageNode phoneMessageNode)
            {
                PhoneMessageNode = phoneMessageNode;
                break;
            }
            else
            {
                PhoneMessageNode = null;
            }
        }
    }
}