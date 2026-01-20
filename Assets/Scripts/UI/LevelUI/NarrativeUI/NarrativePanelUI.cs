using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NarrativePanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textComponent;
    [SerializeField] private RectTransform _panelRectTransform;
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private Image _image;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _durationAnim = 0.2f;
    [SerializeField] private float _heightOffset;
    [SerializeField] private bool _firstLineOffsetKey;
    [SerializeField] private AnimationCurve _animationCurve;

    public TextMeshProUGUI TextComponent => _textComponent;
    public RectTransform PanelRectTransform => _panelRectTransform;
    public RectTransform ImageRectTransform => _imageRectTransform;
    public CanvasGroup CanvasGroup => _canvasGroup;
    public float DurationAnim => _durationAnim;
    public float HeightOffset => _heightOffset;
    public bool FirstLineOffsetKey => _firstLineOffsetKey;
    public AnimationCurve AnimationCurve => _animationCurve;
    public Image Image => _image;
}