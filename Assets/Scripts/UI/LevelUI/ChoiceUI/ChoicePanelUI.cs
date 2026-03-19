using TMPro;
using UnityEngine;

public class ChoicePanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerPanelText;
    [SerializeField] private RectTransform _timerImageRectTransform;
    [SerializeField] private RectTransform _choicesParent;
    [SerializeField] private CanvasGroup _timerPanelCanvasGroup;
    [SerializeField] private RectTransform _monetResourceParent;
    [SerializeField] private RectTransform _heartResourceParent;
    [SerializeField] private float _durationAnim = 0.2f;
    [SerializeField] private float _heightOffset;
    [SerializeField] private float _imageHeightDefault;
    [SerializeField] private Color _blinkColor;
    public TextMeshProUGUI TimerPanelText => _timerPanelText;
    public RectTransform TimerImageRectTransform => _timerImageRectTransform;
    public CanvasGroup TimerPanelCanvasGroup => _timerPanelCanvasGroup;
    public RectTransform MonetResourceParent => _monetResourceParent;
    public RectTransform HeartResourceParent => _heartResourceParent;
    public RectTransform ChoicesParent => _choicesParent;
    public float DurationAnim => _durationAnim;
    public float HeightOffset => _heightOffset;
    public float ImageHeightDefault => _imageHeightDefault;
    public Color BlinkColor => _blinkColor;
}