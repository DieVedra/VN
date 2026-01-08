
using System.Collections.Generic;

[System.Serializable]
public class SaveData
{
    public int Monets;
    public int Hearts;
    public bool SoundStatus;
    public string NameStartStory;
    public string LanguageLocalizationKey;
    // public List<StoryData> StoryDatas;
    public Dictionary<string, StoryData> StoryDatas;
}