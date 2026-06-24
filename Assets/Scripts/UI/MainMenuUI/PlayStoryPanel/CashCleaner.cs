
using System.Collections.Generic;
using UnityEngine.AddressableAssets;

public class CashCleaner : ILocalizable
{
    public const int FontSizeValue = 80;
    public const float HeightPanel = 650f;
    public readonly LocalizationString CashLabelTextToConfirmedPanel = "Кэш";
    public readonly LocalizationString CashQuestionTextToConfirmedPanel  = "Удалить кэш истории для экономии памяти?";
    private readonly StoriesProvider _storiesProvider;

    public CashCleaner(StoriesProvider storiesProvider)
    {
        _storiesProvider = storiesProvider;
    }

    public void CleanCashStory(string storyName)
    {
        Addressables.ClearDependencyCacheAsync(storyName);
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