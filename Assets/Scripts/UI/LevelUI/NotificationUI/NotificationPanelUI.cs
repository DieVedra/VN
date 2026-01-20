using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NotificationPanelUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private Image _image;

    [SerializeField] private float _durationAnim = 0.2f;
    [SerializeField] private float _heightOffset;

    public TextMeshProUGUI Text => _text;
    public RectTransform RectTransform => _rectTransform;
    public RectTransform ImageRectTransform => _imageRectTransform;
    public CanvasGroup CanvasGroup => _canvasGroup;
    public Image Image => _image;
    public float DurationAnim => _durationAnim;
    public float HeightOffset => _heightOffset;

}