
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class WardrobeSeriaDataProviderBuildMode : IWardrobeSeriaDataProvider
{
    private const string _nameAsset = "WardrobeDataSeria";
    private readonly ScriptableObjectAssetLoader _scriptableObjectAssetLoader;
    private readonly AssetExistsHandler _assetExistsHandler;
    private readonly List<WardrobeSeriaData> _wardrobeSeriaDatas;
    private List<string> _names;

    public WardrobeSeriaDataProviderBuildMode()
    {
        _wardrobeSeriaDatas = new List<WardrobeSeriaData>();
        _scriptableObjectAssetLoader = new ScriptableObjectAssetLoader();
        _assetExistsHandler = new AssetExistsHandler();
    }

    public void Dispose()
    {
        _scriptableObjectAssetLoader.UnloadAll();
    }
    public WardrobeSeriaData GetWardrobeSeriaData(int index)
    {
        if (_wardrobeSeriaDatas.Count > index)
        {
            return _wardrobeSeriaDatas[index];
        }
        else
        {
            return null;
        }
    }

    public async UniTask LoadFirstSeriaWardrobeContent()
    {
        _names = await _assetExistsHandler.CheckExistsAssetsNames(_nameAsset);
        
        AddContent(await LoadSeriaWardrobeContent(_names[0]));
        LoadOtherSeriaWardrobeContent().Forget();
    }

    private async UniTaskVoid LoadOtherSeriaWardrobeContent()
    {
        for (int i = 1; i < _names.Count; i++)
        {
            AddContent(await LoadSeriaWardrobeContent(_names[i]));
        }
    }
    private async UniTask<WardrobeSeriaData> LoadSeriaWardrobeContent(string name)
    {
       return await _scriptableObjectAssetLoader.Load<WardrobeSeriaData>(name);
    }

    private void AddContent(WardrobeSeriaData wardrobeSeriaData)
    {
        _wardrobeSeriaDatas.Add(wardrobeSeriaData);
    }
}