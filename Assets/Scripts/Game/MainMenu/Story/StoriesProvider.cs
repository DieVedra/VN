
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
            for (int i = 0; i < _stories.Count; i++)
            {
                for (int j = 0; j < saveData.StoryDatas.Count; j++)
                {
                    if (_stories[i].StoryName == saveData.StoryDatas[j].StoryName)
                    {
                        _stories[i].Init(saveData.StoryDatas[j]);
                        break;
                    }
                }
            }
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < _stories.Count; i++)
        {
            _stories[i].Dispose();
        }
    }

    public int GetIndexByName(string storyName)
    {
        int result = _defaultIndex;
        for (int i = 0; i < _stories.Count; i++)
        {
            if (_stories[i].StoryName == storyName)
            {
                result = i;
                break;
            }
        }
        return result;
    }
    public void TryUpdateStoryDatas(IReadOnlyList<StoryData> datas)
    {
        StoryData storyData = null;
        for (int i = 0; i < datas.Count; i++)
        {
            storyData = datas[i];
            for (int j = 0; j < _stories.Count; j++)
            {
                if (storyData.StoryName == _stories[j].StoryName)
                {
                    storyData.IsLiked = _stories[i].IsLiked;
                    storyData.MyIndex = _stories[i].MyIndex;
                    storyData.StoryName = _stories[i].StoryName;
                    storyData.StoryStarted = _stories[i].StoryStarted;
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