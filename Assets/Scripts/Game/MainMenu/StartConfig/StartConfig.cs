using UnityEngine;

[CreateAssetMenu(fileName = "StartConfig", order = 51)]
public class StartConfig : ScriptableObject
{
    [SerializeField] private int _monets;
    [SerializeField] private int _hearts;
    [SerializeField] private bool _soundStatus;
    [SerializeField] private string _nameStartStory;
    [SerializeField] private string _defaultLanguageLocalizationKey;
    
    public int Monets => _monets;
    public int Hearts => _hearts;
    public bool SoundStatus => _soundStatus;
    public string NameStartStory => _nameStartStory;
    public string DefaultLanguageLocalizationKey => _defaultLanguageLocalizationKey;
}