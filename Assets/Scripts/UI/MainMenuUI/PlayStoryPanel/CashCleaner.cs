
using UnityEngine.AddressableAssets;

public class CashCleaner
{
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
}