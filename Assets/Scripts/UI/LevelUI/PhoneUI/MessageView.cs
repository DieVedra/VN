using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageView : MonoBehaviour
{
    [SerializeField] private RectTransform _imageRectTransform;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private RectTransform _rectTransform;

    public RectTransform ImageRectTransform => _imageRectTransform;
    public TextMeshProUGUI Text => _text;
    public RectTransform ViewRectTransform => _rectTransform;

}