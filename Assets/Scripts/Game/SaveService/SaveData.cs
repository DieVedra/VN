

[System.Serializable]
public class SaveData
{
    public int Monets;
    public int Hearts;
    public bool SoundStatus;
    public int StartIndexStory;
    public string LanguageLocalizationKey;
    public StoryData[] StoryDatas;


    public SaveData()
    {
        SoundStatus = true;
        Monets = 96;
        Hearts = 97;
        StartIndexStory = 0;
    }
}