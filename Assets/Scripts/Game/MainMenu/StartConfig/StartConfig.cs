using UnityEngine;

[CreateAssetMenu(fileName = "StartConfig", order = 51)]
public class StartConfig : ScriptableObject
{
    [SerializeField] private int _monets;
    [SerializeField] private int _hearts;
    [SerializeField] private bool _soundStatus;
    [SerializeField] private bool _analyticsStatus;
    [SerializeField] private bool _crashlyticsStatus;
    [SerializeField] private bool _advertisementStatus;
    [SerializeField] private bool _cCDStatus;
    [SerializeField] private int _advertisementMonetReward;
    [SerializeField] private int _advertisementHearthReward;
    [SerializeField] private string _nameStartStory;
    [SerializeField] private string _defaultLanguageLocalizationKey;
    [SerializeField] private SaveMethod _saveMethod;
    public int Monets => _monets;
    public int Hearts => _hearts;
    public bool SoundStatus => _soundStatus;
    public bool AnalyticsStatus => _analyticsStatus;
    public bool CrashlyticsStatus => _crashlyticsStatus;
    public bool AdvertisementStatus => _advertisementStatus;
    public bool CCDStatus => _cCDStatus;
    public int AdvertisementMonetReward =>  _advertisementMonetReward;
    public int AdvertisementHearthReward =>  _advertisementHearthReward;
    public string NameStartStory => _nameStartStory;
    public string DefaultLanguageLocalizationKey => _defaultLanguageLocalizationKey;
    public SaveMethod SaveMethod => _saveMethod;
    
}