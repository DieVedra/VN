
using Cysharp.Threading.Tasks;

public class GameSeriesProvider : DataProvider<SeriaNodeGraphsHandler>
{
    public const string NodeGraphsHandlerSeriaName = "NodeGraphsHandlerSeria";
    private readonly string _storyName;

    public GameSeriesProvider(string storyName)
    {
        _storyName = storyName;
    }

    public async UniTask<int> Init()
    {
        await CreateNames($"{_storyName}{NodeGraphsHandlerSeriaName}");
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