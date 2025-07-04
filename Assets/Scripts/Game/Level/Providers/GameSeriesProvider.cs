﻿
using Cysharp.Threading.Tasks;

public class GameSeriesProvider : DataProvider<SeriaNodeGraphsHandler>
{
    private const string NodeGraphsHandlerSeriaName = "NodeGraphsHandlerSeria";
    public async UniTask<int> Init()
    {
        await CreateNames(NodeGraphsHandlerSeriaName);
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