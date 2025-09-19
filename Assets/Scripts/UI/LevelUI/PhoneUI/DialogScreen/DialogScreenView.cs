using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenView : BaseScreen
{
    [SerializeField] private Image _contactImage;
    [SerializeField] private Image _contactStatusImage;
    [SerializeField] private TextMeshProUGUI _contactName;
    [SerializeField] private TextMeshProUGUI _contactStatus;
    [SerializeField] private Button _backArrow;
    [SerializeField] private RectTransform _dialogTransform;

    public Color ColorTopPanel => TopPanelColor;
    public Image GradientImage => BackgroundImage;
    public Image ContactImage => _contactImage;
    public Image ContactStatusImage => _contactStatusImage;
    public TextMeshProUGUI ContactName => _contactName;
    public TextMeshProUGUI ContactStatus => _contactStatus;
    public Button BackArrow => _backArrow;
    public RectTransform DialogTransform => _dialogTransform;
}