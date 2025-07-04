using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class JSonAssetLoader : LoadPercentProvider
{
    private TextAsset _cashedObject;
    private AsyncOperationHandle<TextAsset> _handle;
    protected async UniTask<string> Load(string assetId)
    {
        _handle = Addressables.LoadAssetAsync<TextAsset>(assetId);
        SetHandle(_handle);
        await _handle.Task;
        if (_handle.Status == AsyncOperationStatus.Succeeded)
        {
            _cashedObject = _handle.Result;
            return _handle.Result.text;
        }
        else
        {
            return default;
        }
    }

    public void AbortLoad()
    {
        Addressables.Release(_handle);
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