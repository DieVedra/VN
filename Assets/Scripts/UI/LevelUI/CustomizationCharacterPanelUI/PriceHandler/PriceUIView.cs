using TMPro;
using UnityEngine;

public class PriceUIView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private RectTransform _imageBackground;
    [SerializeField] private RectTransform _monetsBlock;
    [SerializeField] private RectTransform _heartsBlock;
    [SerializeField] private TextMeshProUGUI monetsPriceText;
    [SerializeField] private TextMeshProUGUI heartsPriceText;
    
    public CanvasGroup CanvasGroup => _canvasGroup;
    public RectTransform RectTransform => _rectTransform;
    public RectTransform BackgroundRectTransform => _imageBackground;
    public RectTransform MonetsBlock => _monetsBlock;
    public RectTransform HeartsBlock => _heartsBlock;
    public TextMeshProUGUI MonetsPriceText => monetsPriceText;
    public TextMeshProUGUI HeartsPriceText => heartsPriceText;
}