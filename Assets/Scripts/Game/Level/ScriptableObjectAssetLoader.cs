
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ScriptableObjectAssetLoader
{
    private ScriptableObject _cashedObject;
    protected async UniTask<T> Load<T>(string assetId)
    {
        var handle = Addressables.LoadAssetAsync<T>(assetId);
        var loadObject = await handle.Task;
        if (loadObject is ScriptableObject scriptableObject)
        {
            _cashedObject = scriptableObject;
        }
        else
        {
            throw new NullReferenceException($"Object of type {typeof(T)} is null on attempt load from addresables");
        }

        return loadObject;
    }

    protected void Unload()
    {
        if (_cashedObject == null)
        {
            return;
        }
        // Addressables.ReleaseInstance(_cashedObject);
        Addressables.Release(_cashedObject);
        _cashedObject = null;
    }
}