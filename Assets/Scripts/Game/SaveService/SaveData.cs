

[System.Serializable]
public class SaveData
{
    public int LocalizationIndex;
    public int Monets;
    public int Hearts;
    public bool SoundIsOn;

    public StoryData[] StoryDatas;


    public SaveData()
    {
        LocalizationIndex = 0;
        SoundIsOn = true;
        Monets = 96;
        Hearts = 97;
    }
}