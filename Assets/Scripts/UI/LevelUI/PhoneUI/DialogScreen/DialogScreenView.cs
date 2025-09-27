using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenView : BaseScreen
{
    [SerializeField] private Image _contactImage;
    [SerializeField] private TextMeshProUGUI _contactName;
    [SerializeField] private TextMeshProUGUI _contactStatus;
    [SerializeField] private Button _backArrow;
    [SerializeField] private RectTransform _dialogTransform;
    [SerializeField] private GameObject contactStatusGameObject;

    public Color ColorTopPanel => TopPanelColor;
    public Image GradientImage => BackgroundImage;
    public Image ContactImage => _contactImage;
    public GameObject ContactStatusGameObject => contactStatusGameObject;
    public TextMeshProUGUI ContactName => _contactName;
    public TextMeshProUGUI ContactStatus => _contactStatus;
    public Button BackArrow => _backArrow;
    public RectTransform DialogTransform => _dialogTransform;
}