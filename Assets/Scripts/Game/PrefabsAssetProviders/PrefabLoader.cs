
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class PrefabLoader
{
    protected GameObject CashedPrefab { get; private set; }
    
    protected async UniTask Load(string assetId)
    {
        var handle = Addressables.LoadAssetAsync<GameObject>(assetId);
        CashedPrefab = await handle.Task;
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