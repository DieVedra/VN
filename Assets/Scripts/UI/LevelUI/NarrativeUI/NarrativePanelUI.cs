

using TMPro;
using UnityEngine;

public class NarrativePanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textComponent;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _durationAnim = 0.2f;

    public TextMeshProUGUI TextComponent => textComponent;
    public RectTransform RectTransform => _rectTransform;
    public CanvasGroup CanvasGroup => _canvasGroup;
    public float DurationAnim => _durationAnim;

}