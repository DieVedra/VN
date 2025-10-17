using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class PrefabLoader : LoadPercentProvider
{
    protected GameObject CashedPrefab { get; private set; }
    private AsyncOperationHandle _handle;
    private bool IsInstantiating;
    private bool IsLoading;
    public void Abort()
    {
        if (_handle.IsValid())
        {
            if (IsInstantiating == true)
            {
                Addressables.ReleaseInstance(_handle);
                IsInstantiating = false;
            }

            if (IsLoading == true)
            {
                Addressables.Release(_handle);
                IsLoading = false;
            }
        }
    }
    protected async UniTask Load(string assetId)
    {
        IsLoading = true;
        var handle = Addressables.LoadAssetAsync<GameObject>(assetId);
        CashedPrefab = await handle.Task;
        IsLoading = false;
    }
    protected async UniTask<GameObject> InstantiatePrefab(string assetId, Transform parent = null, bool isSynchronously = false)
    {
        IsInstantiating = true;
        var handle = Addressables.InstantiateAsync(assetId, parent);
        SetHandle(handle);
        if (isSynchronously == false)
        {
            await handle.Task;
        }
        else
        {
            handle.Task.RunSynchronously();
        }
        SetLoadComplete();
        IsInstantiating = false;
        return handle.Result;
    }
    protected async UniTask<T> InstantiatePrefab<T>(string assetId, Transform parent = null, bool isSynchronously = false)
    {
        IsInstantiating = true;
        var handle = Addressables.InstantiateAsync(assetId, parent);
        SetHandle(handle);
        if (isSynchronously == false)
        {
            await handle.Task;
        }
        else
        {
            handle.Task.RunSynchronously();
        }

        if (handle.Result.TryGetComponent(out T result))
        {
            SetLoadComplete();
            IsInstantiating = false;
            return result;
        }
        else
        {
            SetLoadComplete();
            IsInstantiating = false;
            return default;
        }
    }
    protected void Unload()
    {
        if (CashedPrefab == null)
        {
            return;
        }
        Addressables.Release(CashedPrefab);
        CashedPrefab = null;
    }
}