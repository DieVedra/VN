using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenView : BaseScreen
{
    [SerializeField] private Image _contactImage;
    [SerializeField] private TextMeshProUGUI _contactName;
    [SerializeField] private TextMeshProUGUI _contactStatus;
    [SerializeField] private TextMeshProUGUI _printsText;
    [SerializeField] private TextMeshProUGUI _iconText;
    [SerializeField] private Button _backArrowButton;
    [SerializeField] private Image _backArrowImage;
    [SerializeField] private Image[] _printsImages;
    [SerializeField] private Button _readDialogButton;
    [SerializeField] private RectTransform _dialogTransform;
    [SerializeField] private GameObject _contactStatusGameObject;

    public Color ColorTopPanel => TopPanelColor;
    public Image GradientImage => BackgroundImage;
    public Image ContactImage => _contactImage;
    public Image BackArrowImage => _backArrowImage;
    public IReadOnlyList<Image> PrintsImages => _printsImages;
    public GameObject ContactStatusGameObject => _contactStatusGameObject;
    public TextMeshProUGUI ContactName => _contactName;
    public TextMeshProUGUI ContactOnlineStatus => _contactStatus;
    public TextMeshProUGUI IconText => _iconText;
    public TextMeshProUGUI PrintsText => _printsText;
    public Button BackArrowButton => _backArrowButton;
    public Button ReadDialogButtonButton => _readDialogButton;
    public RectTransform DialogTransform => _dialogTransform;
}