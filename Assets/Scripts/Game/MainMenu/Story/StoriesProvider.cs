
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StoriesProvider", order = 0)]
public class StoriesProvider : ScriptableObject
{
    [SerializeField] private List<Story> _stories;
    
    public IReadOnlyList<Story> Stories => _stories;

    public void Init(SaveData saveData)
    {
        for (int i = 0; i < _stories.Count; i++)
        {
            for (int j = 0; j < saveData.StoryDatas.Length; j++)
            {
                if (_stories[i].NameSceneAsset == saveData.StoryDatas[j].NameAsset)
                {
                    _stories[i].Init(saveData.StoryDatas[j]);
                    break;
                }
            }
        }
    }

    public StoryData[] GetStoryDatas()
    {
        List<StoryData> storyDatas = new List<StoryData>(_stories.Count);
        for (int i = 0; i < _stories.Count; i++)
        {
            storyDatas.Add(_stories[i].GetStoryData());
        }

        return storyDatas.ToArray();
    }
    public void Dispose()
    {
        for (int i = 0; i < _stories.Count; i++)
        {
            _stories[i].Dispose();
        }
    }
}