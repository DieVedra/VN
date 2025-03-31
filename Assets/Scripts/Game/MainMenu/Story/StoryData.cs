
[System.Serializable]
public class StoryData
{
    public string NameAsset;
    public int CurrentSeriaIndex;
    public int CurrentNodeGraphIndex;
    public int CurrentNodeIndex;
    public int CurrentProgressPercent;
    public int CurrentAudioClipIndex;
    public bool IsLiked;
    public bool LowPassEffectIsOn;
    public int MyIndex;

    public SaveStat[] Stats;
    public BackgroundSaveData BackgroundSaveData;

    public StoryData(string nameAsset)
    {
        NameAsset = nameAsset;
        CurrentSeriaIndex = 0;
        CurrentNodeGraphIndex = 0;
        CurrentNodeIndex = 0;
        CurrentProgressPercent = 0;
        IsLiked = false;
    }
}