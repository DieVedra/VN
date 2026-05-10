using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatCaseView : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _textCase;
    [SerializeField] private Image _imageCase;
    [SerializeField] private RectTransform _rectTransform;

    public TextMeshProUGUI TextCase => _textCase;
    public Image ImageCase => _imageCase;
    public RectTransform RectTransform => _rectTransform;
}