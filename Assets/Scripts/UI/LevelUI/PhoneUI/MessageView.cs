using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageView : MonoBehaviour
{
    [SerializeField] private Image _image;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private RectTransform _rectTransform;

    public Image Image => _image;
    public TextMeshProUGUI Text => _text;
    public RectTransform RectTransform => _rectTransform;
}