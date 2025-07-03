using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class JSonAssetLoader : LoadPercentProvider
{
    private TextAsset _cashedObject;
    
    protected async UniTask<string> Load(string assetId)
    {
        var handle = Addressables.LoadAssetAsync<TextAsset>(assetId);
        SetHandle(handle);
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            _cashedObject = handle.Result;
            return handle.Result.text;
        }
        else
        {
            return default;
        }
    }

    protected void Unload()
    {
        if (_cashedObject == null)
        {
            return;
        }
        Addressables.Release(_cashedObject);
        _cashedObject = null;
    }
}