
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
        if (saveData.StoryDatas.Count > 0)
        {
            foreach (var story in _stories)
            {
                if (saveData.StoryDatas.TryGetValue(story.StoryName, out StoryData storyData))
                {
                    story.Init(storyData);
                }
            }
        }
        else
        {
            foreach (var story in _stories)
            {
                var newStory = new StoryData
                {
                    StoryName = story.StoryName, NameUISpriteAtlas = story.NameUISpriteAtlas, CurrentNodeGraphIndex = 0,
                    CurrentNodeIndex = 0, PutOnSwimsuitKey = false
                };
                
                saveData.StoryDatas.Add(story.StoryName, newStory);
            }
        }
    }

    public void Shutdown()
    {
        foreach (var story in _stories)
        {
            story.Shutdown();
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
    public void TryUpdateStoryDatas(IReadOnlyDictionary<string, StoryData> datas)
    {
        foreach (var pair in datas)
        {
            foreach (var story in _stories)
            {
                if (pair.Value.StoryName == story.StoryName)
                {
                    pair.Value.IsLiked = story.IsLiked;
                    pair.Value.MyIndex = story.MyIndex;
                    pair.Value.StoryName = story.StoryName;
                    pair.Value.StoryStarted = story.StoryStarted;
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