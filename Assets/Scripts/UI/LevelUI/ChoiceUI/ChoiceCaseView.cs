using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceCaseView : MonoBehaviour
{
    [SerializeField] private Button _buttonChoice;
    [SerializeField] private TextMeshProUGUI _textButtonChoice;
    [SerializeField] private TextMeshProUGUI _priceButtonText;
    [SerializeField] private TextMeshProUGUI _additionaryPriceButtonText;
    [SerializeField] private RectTransform _rectTransformChoice;
    [SerializeField] private RectTransform _priceRectTransformChoice;
    [SerializeField] private RectTransform _additionaryPriceRectTransformChoice;
    [SerializeField] private CanvasGroup _canvasGroupChoice;
    
    public Button ButtonChoice => _buttonChoice;
    public TextMeshProUGUI TextButtonChoice => _textButtonChoice;
    public TextMeshProUGUI PriceButtonText => _priceButtonText;
    public TextMeshProUGUI AdditionaryPriceButtonText => _additionaryPriceButtonText;
    public RectTransform RectTransformChoice => _rectTransformChoice;
    
    public RectTransform PriceRectTransformChoice => _priceRectTransformChoice;
    public RectTransform AdditionaryPriceRectTransformChoice => _additionaryPriceRectTransformChoice;
    public CanvasGroup CanvasGroupChoice => _canvasGroupChoice;

}