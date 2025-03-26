using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelView : MonoBehaviour
{
    [SerializeField] private Button _exitButton;
    [SerializeField] private Button _vkButton;
    [SerializeField] private Button _instaButton;
    [SerializeField] private Button _patreonButton;
    
    [SerializeField] private TextMeshProUGUI _textLabel;

    [SerializeField] private SettingsPanelField _notificationField;
    [SerializeField] private SettingsPanelChoiceField _localizationField;
    [SerializeField] private SettingsPanelField _soundField;
    
    
    public Button ExitButton => _exitButton;
    public Button VkButton => _vkButton;
    public Button InstaButton => _instaButton;
    public Button PatreonButton => _patreonButton;
    
    public TextMeshProUGUI TextPanelLabel => _textLabel;
    
    public SettingsPanelField NotificationField => _notificationField;
    public SettingsPanelChoiceField LocalizationField => _localizationField;
    public SettingsPanelField SoundField => _soundField;
}