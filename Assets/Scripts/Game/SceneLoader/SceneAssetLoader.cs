
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.EventSystems;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class SceneAssetLoader
{
    private readonly SaveServiceProvider _saveServiceProvider;

    private readonly float _percentMultiplier = 100f;
    public SceneAssetLoader(SaveServiceProvider saveServiceProvider)
    {
        _saveServiceProvider = saveServiceProvider;
    }

    public int LastPercentLoadValue { get; private set; }

    public async UniTask SceneLoad(string sceneId , ReactiveCommand onSceneTransition)
    {
        var handle = Addressables.LoadSceneAsync(sceneId, LoadSceneMode.Single, false);
        LoadPercentUpdate(handle).Forget();
        await handle.Task;
        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            onSceneTransition?.Execute();
            handle.Result.ActivateAsync();
        }
    }

    private async UniTask LoadPercentUpdate(AsyncOperationHandle<SceneInstance> operationHandle)
    {
        while (operationHandle.IsDone == false)
        {
            await UniTask.Yield();
            LastPercentLoadValue = (int)(operationHandle.PercentComplete * _percentMultiplier);
        }
    }
}