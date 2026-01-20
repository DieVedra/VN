using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CharacterPanelUI : MonoBehaviour
{
    [SerializeField] private RectTransform _imageTransform;
    [SerializeField] private RectTransform _panelTransform;
    [SerializeField] private RectTransform _nameTextTransform;
    [SerializeField] private RectTransform _talkTextTransform;
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _nameTextComponent;
    [SerializeField] private TextMeshProUGUI _talkTextComponent;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _durationAnim = 0.2f;
    [SerializeField] private float _heightOffset;
    [SerializeField] private float _imageHeightDefault;

    public RectTransform ImageTransform => _imageTransform;
    public RectTransform PanelTransform => _panelTransform;
    public RectTransform NameTextTransform => _nameTextTransform;
    public RectTransform TalkTextTransform => _talkTextTransform;
    public TextMeshProUGUI NameTextComponent => _nameTextComponent;
    public TextMeshProUGUI TalkTextComponent => _talkTextComponent;
    public CanvasGroup CanvasGroup => _canvasGroup;
    public Image Image => _image;
    public float DurationAnim => _durationAnim;
    public float HeightOffset => _heightOffset;
    public float ImageHeightDefault => _imageHeightDefault;
}