using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LocalAssetLoader
{
    private GameObject _cashedObject;
    
    protected async UniTask<T> Load<T>(string assetId, Transform parent = null)
    {
        var handle = Addressables.InstantiateAsync(assetId, parent);
        _cashedObject = await handle.Task;
        if (_cashedObject.TryGetComponent(out T component) == false)
        {
            throw new NullReferenceException($"Object of type {typeof(T)} is null on attempt load from addresables");
        }

        return component;
    }

    protected void Unload()
    {
        if (_cashedObject == null)
        {
            return;
        }
        _cashedObject.SetActive(false);
        Addressables.ReleaseInstance(_cashedObject);
        _cashedObject = null;
    }
}