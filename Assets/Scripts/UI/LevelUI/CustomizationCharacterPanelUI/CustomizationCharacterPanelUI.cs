using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CustomizationCharacterPanelUI : MonoBehaviour
{
    [SerializeField] private PriceUIView _priceUIView;
    [SerializeField] private Button _leftArrow;
    [SerializeField] private Button _rightArrow;
    [SerializeField] private Button _playButton;
    [SerializeField] private Button _skinColorButton;
    [SerializeField] private Button _hairstyleButton;
    [SerializeField] private Button _clothesButton;
    
    [SerializeField] private RectTransform _skinButtonTransform;
    [SerializeField] private RectTransform _hairstyleButtonTransform;
    [SerializeField] private RectTransform _clothesButtonTransform;
    
    [SerializeField] private TextMeshProUGUI _titleText;
    [SerializeField] private TextMeshProUGUI _skinColorButtonText;
    [SerializeField] private TextMeshProUGUI _hairstyleButtonText;
    [SerializeField] private TextMeshProUGUI _clothesButtonText;
    [SerializeField] private TextMeshProUGUI _statPanelText;
    [SerializeField] private CanvasGroup _rightArrowCanvasGroup;
    [SerializeField] private CanvasGroup _leftArrowCanvasGroup;
    [SerializeField] private CanvasGroup _skinColorModeCanvasGroup;
    [SerializeField] private CanvasGroup _hairstyleModeCanvasGroup;
    [SerializeField] private CanvasGroup _clothesModeCanvasGroup;
    [SerializeField] private CanvasGroup _statPanelCanvasGroup;
    [SerializeField] private CanvasGroup _playButtonCanvasGroup;
    [SerializeField] private float _durationAnimPriceView = 0.2f;
    [SerializeField] private float _durationAnimStatView = 0.2f;
    [SerializeField] private float _durationButtonPlay = 0.5f;
    [SerializeField] private int _sublingIndex = 6;
    
    public PriceUIView PriceUIView => _priceUIView;
    public Button LeftArrow =>_leftArrow;
    public Button RightArrow =>_rightArrow;
    public Button PlayButton =>_playButton;
    public Button SkinColorButton =>_skinColorButton;
    public Button HairstyleButton =>_hairstyleButton;
    public Button ClothesButton =>_clothesButton;
    
    public TextMeshProUGUI TitleText => _titleText;
    public TextMeshProUGUI StatPanelText => _statPanelText;
    
    public CanvasGroup RightArrowCanvasGroup => _rightArrowCanvasGroup;
    public CanvasGroup LeftArrowCanvasGroup => _leftArrowCanvasGroup;
    public CanvasGroup SkinColorModeCanvasGroup => _skinColorModeCanvasGroup;
    public CanvasGroup HairstyleModeCanvasGroup => _hairstyleModeCanvasGroup;
    public CanvasGroup ClothesModeCanvasGroup => _clothesModeCanvasGroup;
    public CanvasGroup StatPanelCanvasGroup => _statPanelCanvasGroup;
    public CanvasGroup PlayButtonCanvasGroup => _playButtonCanvasGroup;
    public float DurationAnimPriceView => _durationAnimPriceView;
    public float DurationAnimStatView => _durationAnimStatView;
    public float DurationButtonPlay => _durationButtonPlay;
    public int SublingIndex => _sublingIndex;
    public Dictionary<ArrowSwitchMode, (Button, TextMeshProUGUI, RectTransform)> CustomizationButtons => new Dictionary<ArrowSwitchMode, (Button, TextMeshProUGUI, RectTransform)>()
    {
        {ArrowSwitchMode.SkinColor, (_skinColorButton, _skinColorButtonText, _skinButtonTransform)},
        {ArrowSwitchMode.Hairstyle, (_hairstyleButton, _hairstyleButtonText, _hairstyleButtonTransform)},
        {ArrowSwitchMode.Clothes, (_clothesButton, _clothesButtonText, _clothesButtonTransform)},
        {ArrowSwitchMode.Swimsuits, (_clothesButton, _clothesButtonText, _clothesButtonTransform)}
    };
}