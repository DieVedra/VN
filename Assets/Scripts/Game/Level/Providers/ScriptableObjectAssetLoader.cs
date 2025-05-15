
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ScriptableObjectAssetLoader : LoadPercentProvider
{
    private List<ScriptableObject> _cashedObjects;

    public async UniTask<T> Load<T>(string assetId)
    {
        if (_cashedObjects == null)
        {
            _cashedObjects = new List<ScriptableObject>();
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetId);
        SetHandle(handle);
        T loadObject = await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded && loadObject is ScriptableObject scriptableObject)
        {
            _cashedObjects.Add(scriptableObject);
        }

        return loadObject;
    }

    public void UnloadAll()
    {
        for (int i = 0; i < _cashedObjects.Count; i++)
        {
            Addressables.Release(_cashedObjects[i]);
            _cashedObjects[i] = null;
        }

        _cashedObjects = null;
    }
}