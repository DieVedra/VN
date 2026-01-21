using TMPro;
using UnityEngine;

public class ChoicePanelUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerPanelText;
    [SerializeField] private RectTransform _timerImageRectTransform;
    [SerializeField] private CanvasGroup _timerPanelCanvasGroup;
    
    [SerializeField] private float _durationAnim = 0.2f;
    [SerializeField] private float _heightOffset;
    [SerializeField] private float _offsetBetweenPanels;
    [SerializeField] private float _imageHeightDefault;
    [SerializeField] private float _defaultPosYCentralButtonChoice2;
    public TextMeshProUGUI TimerPanelText => _timerPanelText;
    public RectTransform TimerImageRectTransform => _timerImageRectTransform;
    public CanvasGroup TimerPanelCanvasGroup => _timerPanelCanvasGroup;
    public float DurationAnim => _durationAnim;
    public float HeightOffset => _heightOffset;
    public float OffsetBetweenPanels => _offsetBetweenPanels;
    public float ImageHeightDefault => _imageHeightDefault;
    public float DefaultPosYCentralButtonChoice2 => _defaultPosYCentralButtonChoice2;
}