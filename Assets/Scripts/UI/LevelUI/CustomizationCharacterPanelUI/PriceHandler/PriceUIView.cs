using TMPro;
using UnityEngine;

public class PriceUIView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _imageBackgroundRectTransform;
    [SerializeField] private RectTransform _monetsBlock;
    [SerializeField] private RectTransform _heartsBlock;
    [SerializeField] private TextMeshProUGUI _monetsPriceText;
    [SerializeField] private TextMeshProUGUI _heartsPriceText;
    
    public CanvasGroup CanvasGroup => _canvasGroup;
    public RectTransform ImageBackgroundRectTransform => _imageBackgroundRectTransform;
    public RectTransform MonetsBlock => _monetsBlock;
    public RectTransform HeartsBlock => _heartsBlock;
    public TextMeshProUGUI MonetsPriceText => _monetsPriceText;
    public TextMeshProUGUI HeartsPriceText => _heartsPriceText;
}