
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PrefabLoader : LoadPercentProvider
{
    protected GameObject CashedPrefab { get; private set; }
    
    protected async UniTask Load(string assetId)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(assetId);
        CashedPrefab = await handle.Task;
    }

    protected async UniTask<GameObject> InstantiatePrefab(string assetId, Transform parent = null, bool isSynchronously = false)
    {
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
        return handle.Result;
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