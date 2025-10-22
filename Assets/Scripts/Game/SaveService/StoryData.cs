
using System.Collections.Generic;

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
    public int CurrentPhoneMinute = -1;
    public BackgroundSaveData BackgroundSaveData;

    // public SaveStat[] Stats;
    // public PhoneAddedContact[] Contacts;
    // public WardrobeSaveData[] WardrobeSaveDatas;

    public List<SaveStat> Stats;
    public List<WardrobeSaveData> WardrobeSaveDatas;
    public List<PhoneAddedContact> Contacts;

    public StoryData()
    {
        Stats = new List<SaveStat>();
        WardrobeSaveDatas = new List<WardrobeSaveData>();
        Contacts = new List<PhoneAddedContact>();
    }
}