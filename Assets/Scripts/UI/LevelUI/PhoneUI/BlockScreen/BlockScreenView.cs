using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BlockScreenView : BaseScreen
{
    [SerializeField] private TextMeshProUGUI _time;
    [SerializeField] private TextMeshProUGUI _data;
    [SerializeField] private Image _notificationContactIcon;
    [SerializeField] private TextMeshProUGUI _contactName;
    [SerializeField] private TextMeshProUGUI _notificationText;
    [SerializeField] private TextMeshProUGUI _iconText;
    
    public Color ColorTopPanel => TopPanelColor;
    public Image ImageBackground =>  BackgroundImage;
    public TextMeshProUGUI Time => _time;
    public TextMeshProUGUI Data => _data;
    public TextMeshProUGUI ContactName => _contactName;
    public TextMeshProUGUI NotificationText => _notificationText;
    public TextMeshProUGUI IconText => _iconText;
    public Image Background => BackgroundImage;
    public Image NotificationContactIcon => _notificationContactIcon;
}