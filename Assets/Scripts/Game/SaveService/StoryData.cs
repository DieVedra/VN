
[System.Serializable]
public class StoryData
{
    public string StoryName;
    public int CurrentSeriaIndex;
    public int CurrentNodeGraphIndex;
    public int CurrentNodeIndex;
    public int CurrentProgressPercent;
    public int CurrentAudioClipIndex;
    public bool IsLiked;
    public bool LowPassEffectIsOn;
    public bool StoryStarted;
    public int MyIndex;
    public int CustomizableCharacterIndex;
    public SaveStat[] Stats;
    public BackgroundSaveData BackgroundSaveData;
    public WardrobeSaveData[] WardrobeSaveDatas;
}