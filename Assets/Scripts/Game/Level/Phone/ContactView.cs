using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContactView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private Image _onlineStatusImage;
    [SerializeField] private Image _newMessageIndicatorImage;
    [SerializeField] private TextMeshProUGUI _textIcon;
    [SerializeField] private TextMeshProUGUI _textName;
    [SerializeField] private Button _contactButton;
    [SerializeField] private RectTransform _rectTransform;
    public Image Image => _image;
    public Image OnlineStatusImage => _onlineStatusImage;
    public Image NewMessageIndicatorImage => _newMessageIndicatorImage;
    public TextMeshProUGUI TextIcon => _textIcon;
    public TextMeshProUGUI TextName => _textName;
    public Button ContactButton => _contactButton;
    public RectTransform RectTransform => _rectTransform;
}