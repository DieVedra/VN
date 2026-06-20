
using Cysharp.Threading.Tasks;

public class GameSeriesProvider : DataProvider<SeriaNodeGraphsHandler>
{
    private const string _nodeGraphsHandlerSeriaName = "NodeGraphsHandlerSeria";
    private readonly string _storyName;

    public GameSeriesProvider(string storyName)
    {
        _storyName = storyName;
    }

    public async UniTask<int> Init()
    {
        await CreateNames($"{_storyName}{_nodeGraphsHandlerSeriaName}");
        if (AssetsFinded == true)
        {
            return NamesCount;
        }
        else
        {
            return 0;
        }
    }
}