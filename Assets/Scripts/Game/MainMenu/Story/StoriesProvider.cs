
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoriesProvider", order = 51)]
public class StoriesProvider : ScriptableObject
{
    [SerializeField] private List<Story> _stories;

    private const int _defaultIndex = 0;
    public IReadOnlyList<Story> Stories => _stories;

    public void Init(SaveData saveData)
    {
        if (saveData.StoryDatas != null)
        {
            foreach (var story in _stories)
            {
                foreach (var data in saveData.StoryDatas)
                {
                    if (story.StoryName == data.StoryName)
                    {
                        story.Init(data);
                        break;
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        foreach (var story in _stories)
        {
            story.Dispose();
        }
        
    }

    public int GetIndexByName(string storyName)
    {
        int result = _defaultIndex;
        foreach (var story in _stories)
        {
            if (story.StoryName == storyName)
            {
                break;
            }
            result++;
        }
        return result;
    }
    public void TryUpdateStoryDatas(IReadOnlyList<StoryData> datas)
    {
        foreach (var storyData in datas)
        {
            foreach (var story in _stories)
            {
                if (storyData.StoryName == story.StoryName)
                {
                    storyData.IsLiked = story.IsLiked;
                    storyData.MyIndex = story.MyIndex;
                    storyData.StoryName = story.StoryName;
                    storyData.StoryStarted = story.StoryStarted;
                    break;
                }
            }
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> content = new List<LocalizationString>();

        foreach (var story in _stories)
        {
            content.AddRange(story.GetLocalizableContent());
        }
        return content;
    }
}