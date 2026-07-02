using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class CashCleaner : ILocalizable
{
    public const int FontSizeValue = 80;
    public const float HeightPanel = 650f;
    public readonly LocalizationString CashLabelTextToConfirmedPanel = "Кэш";
    public readonly LocalizationString CashQuestionTextToConfirmedPanel  = "Удалить кэш истории для экономии памяти?";
    private readonly StoriesProvider _storiesProvider;

    private readonly SaveServiceProvider _saveServiceProvider;
    public CashCleaner(StoriesProvider storiesProvider, SaveServiceProvider saveServiceProvider)
    {
        _storiesProvider = storiesProvider;
        _saveServiceProvider = saveServiceProvider;
    }

    public async UniTask Construct()
    {
// #if !UNITY_EDITOR
        string cachePath = Path.Combine(Application.persistentDataPath, "UnityCache", "Shared");
        
        if (!Directory.Exists(cachePath))
        {
            Debug.Log("Папка кэша не существует, пропускаем очистку");
            return;
        }
        AsyncOperationHandle<bool> cleanOp = default;
        try
        {
            cleanOp = Addressables.CleanBundleCache();
            bool success = await cleanOp.Task.AsUniTask();

            if (success)
            {
                Debug.Log("Кэш успешно очищен");
            }
            else
            {
                Debug.LogWarning("Очистка кэша вернула false");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Ошибка при очистке кэша: {e.Message}");
        }
        finally
        {
            if (cleanOp.IsValid())
            {
                Addressables.Release(cleanOp);
            }
        }
// #endif
    }

    public bool GetKeyActiveClearCashButton(string storyName)
    {
        bool result = false;
        if (_saveServiceProvider.SaveData.StoryDatas.TryGetValue(storyName, out StoryData storyData))
        {
            if (storyData.CashHasBeenLoaded == true)
            {
                result = true;
            }
        }
        return result;
    }
    public void CleanCashStory(string storyName)
    {
        if (_saveServiceProvider.SaveData.StoryDatas.TryGetValue(storyName, out StoryData storyData))
        {
            Addressables.ClearDependencyCacheAsync(storyName);
            storyData.CashHasBeenLoaded = false;
        }
    }

    public void CleanAllCash()
    {
        foreach (var story in _storiesProvider.Stories)
        {
            Addressables.ClearDependencyCacheAsync(story.StoryName);
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new [] {CashLabelTextToConfirmedPanel, CashQuestionTextToConfirmedPanel};
    }
}