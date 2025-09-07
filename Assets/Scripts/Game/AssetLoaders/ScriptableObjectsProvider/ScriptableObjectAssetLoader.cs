using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ScriptableObjectAssetLoader : LoadPercentProvider
{
    private AsyncOperationHandle _handle;
    public bool IsLoading { get; private set; }
    public async UniTask<T> Load<T>(string assetId)
    {
        IsLoading = true;
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetId);
        _handle = handle;
        SetHandle(handle);
        T loadObject = await handle.ToUniTask();
        if (handle.Status == AsyncOperationStatus.Succeeded && loadObject is ScriptableObject)
        {
            SetLoadComplete();
            IsLoading = false;
            return loadObject;
        }
        else
        {
            SetLoadComplete();
            IsLoading = false;
            return default;
        }
    }
    public void TryAbortLoad()
    {
        if (_handle.IsValid())
        {
            Addressables.Release(_handle);
            IsLoading = false;
        }
    }
}