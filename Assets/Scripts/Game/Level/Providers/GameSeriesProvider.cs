
using Cysharp.Threading.Tasks;

public class GameSeriesProvider : DataProvider<SeriaNodeGraphsHandler>
{
    private const string NodeGraphsHandlerSeriaName = "NodeGraphsHandlerSeria";

    public async UniTask Init()
    {
        await CreateNames(NodeGraphsHandlerSeriaName);
    }
}