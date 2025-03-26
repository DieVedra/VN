
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;

public class SceneLoader
{
    private readonly float _percentMultiplier = 100f;
    public int LastPercentLoadValue { get; private set; }
    public event Action<int> OnLoadPercentUpdate;
    public event Action<AsyncOperation> OnCompleteLoad;

    public async UniTask StartLoad(int index)
    {
        var asyncOperation = SceneManager.LoadSceneAsync(index, LoadSceneMode.Additive);
        asyncOperation.completed += UnloadPreviousScene;
        asyncOperation.allowSceneActivation = false;
        await LoadPercentUpdate(asyncOperation);
    }

    public void Activate(AsyncOperation operation)
    {
        operation.allowSceneActivation = true;
    }

    private void UnloadPreviousScene(AsyncOperation operation)
    {
        Addressables.UnloadSceneAsync(SaveServiceProvider.CashedScene);
    }
    private async UniTask LoadPercentUpdate(AsyncOperation operation)
    {
        // while (operation.progress < 0.9f)
        while (operation.progress < 0.9f)
        {
            Debug.Log($"operation.progress {operation.progress}");

            await UniTask.Yield();
            LastPercentLoadValue = (int)(operation.progress * _percentMultiplier);
            OnLoadPercentUpdate?.Invoke(LastPercentLoadValue);
        }
        OnCompleteLoad?.Invoke(operation);
        

    }
}