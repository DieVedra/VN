
using TMPro;
using UnityEngine;

public class LoadIndicatorView : MonoBehaviour
{
    [SerializeField] private RectTransform _rectTransformIcon;
    [SerializeField] private TextMeshProUGUI _loadText;

    public RectTransform RectTransformIcon => _rectTransformIcon;
    public TextMeshProUGUI LoadText => _loadText;

}