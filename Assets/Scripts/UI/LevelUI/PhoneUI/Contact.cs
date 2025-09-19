using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Contact : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _name;
    [SerializeField] private RectTransform _rectTransform;
    
    public Image Icon => _icon;
    public TextMeshProUGUI Name => _name;
    public RectTransform RectTransform => _rectTransform;
}