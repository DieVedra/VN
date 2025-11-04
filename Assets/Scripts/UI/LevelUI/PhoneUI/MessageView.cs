using TMPro;
using UnityEngine;

public class MessageView : MonoBehaviour
{
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private RectTransform _rectTransform;

    public RectTransform ImageRectTransform => _imageRectTransform;
    public TextMeshProUGUI Text => _text;
    public RectTransform RectTransform => _rectTransform;

}