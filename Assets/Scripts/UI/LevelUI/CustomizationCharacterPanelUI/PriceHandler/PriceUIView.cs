using TMPro;
using UnityEngine;

public class PriceUIView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _background;
    [SerializeField] private RectTransform _monetsBlock;
    [SerializeField] private RectTransform _heartsBlock;
    [SerializeField] private TextMeshProUGUI monetsPriceText;
    [SerializeField] private TextMeshProUGUI heartsPriceText;
    
    public CanvasGroup CanvasGroup => _canvasGroup;
    public RectTransform BackgroundRectTransform => _background;
    public RectTransform MonetsBlock => _monetsBlock;
    public RectTransform HeartsBlock => _heartsBlock;
    public TextMeshProUGUI MonetsPriceText => monetsPriceText;
    public TextMeshProUGUI HeartsPriceText => heartsPriceText;
}