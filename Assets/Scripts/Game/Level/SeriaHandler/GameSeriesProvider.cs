
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class GameSeriesProvider
{
    private const string NodeGraphsHandlerSeriaName = "NodeGraphsHandlerSeria";
    private readonly ScriptableObjectAssetLoader _scriptableObjectAssetLoader;
    private readonly AssetExistsHandler _assetExistsHandler;
    private List<string> _names;
    public event Action<SeriaNodeGraphsHandler> OnLoad;

    public GameSeriesProvider()
    {
        _scriptableObjectAssetLoader = new ScriptableObjectAssetLoader();
        _assetExistsHandler = new AssetExistsHandler();
        
    }
    public void Dispose()
    {
        _scriptableObjectAssetLoader.UnloadAll();
    }
    public async UniTask LoadFirstSeria()
    {
        _names = await _assetExistsHandler.CheckExistsAssetsNames(NodeGraphsHandlerSeriaName);
        await LoadSeria(_names[0]);
        LoadOtherCharacters().Forget();
    }

    private async UniTaskVoid LoadOtherCharacters()
    {
        for (int i = 1; i < _names.Count; ++i)
        {
            await LoadSeria(_names[i]);
        }
    }
    private async UniTask LoadSeria(string name)
    {
        SeriaNodeGraphsHandler seriaNodeGraphsHandler = await _scriptableObjectAssetLoader.Load<SeriaNodeGraphsHandler>(name);
        OnLoad?.Invoke(seriaNodeGraphsHandler);
    }
}