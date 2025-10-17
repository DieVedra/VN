using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationView : MonoBehaviour
{
    [SerializeField] private Button _button;
    [SerializeField] private Image _icon;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _textIcon;
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _notificationText;

    public Button Button => _button;
    public Image Icon => _icon;
    public RectTransform RectTransform => _rectTransform;
    public TextMeshProUGUI TextIcon => _textIcon;
    public TextMeshProUGUI NameText => _nameText;
    public TextMeshProUGUI NotificationText => _notificationText;
}