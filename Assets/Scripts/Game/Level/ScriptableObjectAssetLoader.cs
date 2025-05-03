
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ScriptableObjectAssetLoader
{
    private List<ScriptableObject> _cashedObjects;

    public async UniTask<T> Load<T>(string assetId)
    {
        if (_cashedObjects == null)
        {
            _cashedObjects = new List<ScriptableObject>();
        }

        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetId);
        LoadPercentUpdate(handle).Forget();
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
    
    private async UniTaskVoid LoadPercentUpdate<T>(AsyncOperationHandle<T> operationHandle)
    {
        while (operationHandle.IsDone == false)
        {
            await UniTask.Yield();
            // LastPercentLoadValue = (int)(operationHandle.PercentComplete * _percentMultiplier);
            Debug.Log($"PercentComplete: {operationHandle.PercentComplete * 100f}");
            
            // OnLoadPercentUpdate?.Invoke(LastPercentLoadValue);
        }
    }
}