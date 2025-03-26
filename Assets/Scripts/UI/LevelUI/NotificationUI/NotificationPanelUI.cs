

using TMPro;
using UnityEngine;

public class NotificationPanelUI : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _durationAnim = 0.2f;

    public TextMeshProUGUI Text => _text;
    public RectTransform RectTransform => _rectTransform;
    public CanvasGroup CanvasGroup => _canvasGroup;
    public float DurationAnim => _durationAnim;

}