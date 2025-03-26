
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadScreenUIView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _disclaimerText;
    [SerializeField] private Image _loadScreenImage;
    [SerializeField] private Image _loadScreenMaskImage;
    [SerializeField] private Image _logoImage;
    [SerializeField] private RectMask2D _rectMask2D;
    [SerializeField] private float _paddingValue;
    [SerializeField] private float _maskHideDuration;
    
    public TextMeshProUGUI DisclaimerText => _disclaimerText;
    public Image LoadScreenImage => _loadScreenImage;
    public Image LoadScreenMaskImage => _loadScreenMaskImage;
    public Image LogoImage => _logoImage;
    public RectMask2D RectMask2D => _rectMask2D;
    public float PaddingValue => _paddingValue;
    public float MaskHideDuration => _maskHideDuration;
}