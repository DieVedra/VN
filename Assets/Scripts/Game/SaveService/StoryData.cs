using System.Collections.Generic;

[System.Serializable]
public class StoryData
{
    public string StoryName;
    public string NameUISpriteAtlas;
    public int CurrentSeriaIndex;
    public int CurrentNodeGraphIndex;
    public int CurrentNodeIndex;
    public int CurrentProgressPercent;
    public string CurrentAudioMusicKey;
    public string CurrentAudioAmbientKey;
    public bool IsLiked;
    public List<AudioEffect> AudioEffectsIsOn;
    public bool StoryStarted;
    public bool PutOnSwimsuitKey;
    
    public bool PhoneNodeIsActiveOnSave;
    public bool PhoneNotificationPressed;
    public int PhoneScreenIndex;
    public int PhoneContentNodeIndex;
    public string NotificationsInBlockScreenKey;
    public int CurrentPhoneMinute = -1;
    public string DialogContactKey;
    
    public List<int> ReadedContactNodeCaseIndexes;
    public List<string> OnlineContactsKeys;
    public List<string> NotificationsKeys;
    public List<PhoneSaveData> PhoneSaveDatas;


    public int MyIndex;
    public int CustomizableCharacterIndex;
    public BackgroundSaveData BackgroundSaveData;

    public List<SaveStat> Stats;
    public List<WardrobeSaveData> WardrobeSaveDatas;

    public StoryData()
    {
        Stats = new List<SaveStat>();
        WardrobeSaveDatas = new List<WardrobeSaveData>();
        PhoneSaveDatas = new List<PhoneSaveData>();
        AudioEffectsIsOn = new List<AudioEffect>();
    }
}