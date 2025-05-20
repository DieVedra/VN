
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ScriptableObjectAssetLoader : LoadPercentProvider
{
    public async UniTask<T> Load<T>(string assetId)
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(assetId);
        SetHandle(handle);
        T loadObject = await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded && loadObject is ScriptableObject scriptableObject)
        {
            return loadObject;
        }
        else
        {
            return default;
        }
    }
}