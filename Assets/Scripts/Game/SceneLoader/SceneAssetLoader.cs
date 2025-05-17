
using System;
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneAssetLoader
{
    private readonly SaveServiceProvider _saveServiceProvider;

    private readonly float _percentMultiplier = 100f;
    // private SceneInstance _cashedScene;
    public SceneAssetLoader(SaveServiceProvider saveServiceProvider)
    {
        _saveServiceProvider = saveServiceProvider;
    }

    public int LastPercentLoadValue { get; private set; }
    public event Action<int> OnLoadPercentUpdate;
    public event Action<AsyncOperationHandle<SceneInstance>> OnCompleteLoad;

    public async UniTask StartLoad(string sceneId)
    {
        var handle = Addressables.LoadSceneAsync(sceneId, LoadSceneMode.Single, false);
        handle.Completed += CompleteLoad;
        await LoadPercentUpdate(handle);
    }

    // public void Unload()
    // {
    //     Addressables.UnloadSceneAsync(_cashedScene);
    // }

    private async UniTask LoadPercentUpdate(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        while (operationHandle.IsDone == false)
        {
            await UniTask.Yield();
            LastPercentLoadValue = (int)(operationHandle.PercentComplete * _percentMultiplier);
            OnLoadPercentUpdate?.Invoke(LastPercentLoadValue);
        }
    }

    public void Activate(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        if (operationHandle.Status == AsyncOperationStatus.Succeeded)
        {
            // _cashedScene = operationHandle.Result;
            _saveServiceProvider.CashedScene = operationHandle.Result;
            operationHandle.Result.ActivateAsync();
        }
    }

    private void CompleteLoad(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        OnCompleteLoad?.Invoke(operationHandle);
    }
}