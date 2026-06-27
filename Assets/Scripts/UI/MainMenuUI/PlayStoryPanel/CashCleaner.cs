using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

    public void Construct()
    {
#if !UNITY_EDITOR
        Addressables.CleanBundleCache();
#endif
    }

    public bool GetClearButtonActiveKey(string storyName)
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