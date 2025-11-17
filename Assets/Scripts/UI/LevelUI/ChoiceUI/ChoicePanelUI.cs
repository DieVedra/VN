using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanelUI : MonoBehaviour
{
    [SerializeField] private List<RectTransform> _choiseCasesViews;
    [SerializeField] private TextMeshProUGUI _timerPanelText;
    [SerializeField] private RectTransform _timerImageRectTransform;
    [SerializeField] private RectTransform _panelRectTransform;
    [SerializeField] private CanvasGroup _timerPanelCanvasGroup;
    
    [SerializeField] private float _durationAnim = 0.2f;
    [SerializeField] private float _heightOffset;
    [SerializeField] private float _offsetBetweenPanels;
    [SerializeField] private float _imageHeightDefault;
    [SerializeField] private float _defaultPosYCentralButtonChoice2;
    
    
    
    

    [SerializeField] private Button _buttonChoice1;
    [SerializeField] private Button _buttonChoice2;
    [SerializeField] private Button _buttonChoice3;
    [SerializeField] private Button _buttonChoice4;

    [SerializeField] private TextMeshProUGUI _textButtonChoice1;
    [SerializeField] private TextMeshProUGUI _textButtonChoice2;
    [SerializeField] private TextMeshProUGUI _textButtonChoice3;
    [SerializeField] private TextMeshProUGUI _textButtonChoice4;


    [SerializeField] private TextMeshProUGUI _priceButton1Text;
    [SerializeField] private TextMeshProUGUI _priceButton2Text;
    [SerializeField] private TextMeshProUGUI _priceButton3Text;
    [SerializeField] private TextMeshProUGUI _priceButton4Text;

    [SerializeField] private TextMeshProUGUI _additionaryPriceButton1Text;
    [SerializeField] private TextMeshProUGUI _additionaryPriceButton2Text;
    [SerializeField] private TextMeshProUGUI _additionaryPriceButton3Text;
    [SerializeField] private TextMeshProUGUI _additionaryPriceButton4Text;

    [SerializeField] private RectTransform _rectTransformChoice1;
    [SerializeField] private RectTransform _rectTransformChoice2;
    [SerializeField] private RectTransform _rectTransformChoice3;
    [SerializeField] private RectTransform _rectTransformChoice4;

    [SerializeField] private RectTransform _priceRectTransformChoice1;
    [SerializeField] private RectTransform _priceRectTransformChoice2;
    [SerializeField] private RectTransform _priceRectTransformChoice3;
    [SerializeField] private RectTransform _priceRectTransformChoice4;

    [SerializeField] private RectTransform _additionaryPriceRectTransformChoice1;
    [SerializeField] private RectTransform _additionaryPriceRectTransformChoice2;
    [SerializeField] private RectTransform _additionaryPriceRectTransformChoice3;
    [SerializeField] private RectTransform _additionaryPriceRectTransformChoice4;


    [SerializeField] private CanvasGroup _canvasGroupChoice1;
    [SerializeField] private CanvasGroup _canvasGroupChoice2;
    [SerializeField] private CanvasGroup _canvasGroupChoice3;
    [SerializeField] private CanvasGroup _canvasGroupChoice4;

    // [SerializeField] private CanvasGroup _moneyPanelCanvasGroup;

    // [SerializeField] private CanvasGroup _heartsPanelCanvasGroup;

    public Button ButtonChoice1 => _buttonChoice1;
    public Button ButtonChoice2 => _buttonChoice2;
    public Button ButtonChoice3 => _buttonChoice3;
    public Button ButtonChoice4 => _buttonChoice4;
    
    public TextMeshProUGUI TextButtonChoice1 => _textButtonChoice1;
    public TextMeshProUGUI TextButtonChoice2 => _textButtonChoice2;
    public TextMeshProUGUI TextButtonChoice3 => _textButtonChoice3;
    public TextMeshProUGUI PriceButton1Text => _priceButton1Text;
    public TextMeshProUGUI PriceButton2Text => _priceButton2Text;
    public TextMeshProUGUI PriceButton3Text => _priceButton3Text;

    public TextMeshProUGUI AdditionaryPriceButton1Text => _additionaryPriceButton1Text;
    public TextMeshProUGUI AdditionaryPriceButton2Text => _additionaryPriceButton2Text;
    public TextMeshProUGUI AdditionaryPriceButton3Text => _additionaryPriceButton3Text;

    public RectTransform RectTransformChoice1 => _rectTransformChoice1;
    public RectTransform RectTransformChoice2 => _rectTransformChoice2;
    public RectTransform RectTransformChoice3 => _rectTransformChoice3;

    public RectTransform PriceRectTransformChoice1 => _priceRectTransformChoice1;
    public RectTransform PriceRectTransformChoice2 => _priceRectTransformChoice2;
    public RectTransform PriceRectTransformChoice3 => _priceRectTransformChoice3;
    public RectTransform AdditionaryPriceRectTransformChoice1 => _additionaryPriceRectTransformChoice1;
    public RectTransform AdditionaryPriceRectTransformChoice2 => _additionaryPriceRectTransformChoice2;
    public RectTransform AdditionaryPriceRectTransformChoice3 => _additionaryPriceRectTransformChoice3;
    public CanvasGroup CanvasGroupChoice1 => _canvasGroupChoice1;
    public CanvasGroup CanvasGroupChoice2 => _canvasGroupChoice2;

    public CanvasGroup CanvasGroupChoice3 => _canvasGroupChoice3;

    // public CanvasGroup MoneyPanelCanvasGroup => _moneyPanelCanvasGroup;

    // public CanvasGroup HeartsPanelCanvasGroup => _heartsPanelCanvasGroup;
    
    
    
    
    public TextMeshProUGUI TimerPanelText => _timerPanelText;
    public RectTransform TimerImageRectTransform => _timerImageRectTransform;
    public RectTransform PanelRectTransform => _panelRectTransform;
    public CanvasGroup TimerPanelCanvasGroup => _timerPanelCanvasGroup;
    public float DurationAnim => _durationAnim;
    public float HeightOffset => _heightOffset;
    public float OffsetBetweenPanels => _offsetBetweenPanels;
    public float ImageHeightDefault => _imageHeightDefault;
    public float DefaultPosYCentralButtonChoice2 => _defaultPosYCentralButtonChoice2;

}