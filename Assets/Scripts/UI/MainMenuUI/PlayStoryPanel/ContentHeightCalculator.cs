using TMPro;
using UnityEngine;

public class ContentHeightCalculator
{
    private const float _x = 20f;
    private const float _y = 10f;
    private readonly float _maxWidth;
    private Vector2 _padding = new Vector2(_x, _y);
    private readonly TextMeshProUGUI _tmpText;
    private readonly RectTransform _rectTransform;
    private readonly RectTransform _parentRectTransform;

    public ContentHeightCalculator(TextMeshProUGUI tmpText)
    {
        _tmpText = tmpText;
        _rectTransform = _tmpText.transform as RectTransform;
        _parentRectTransform = _tmpText.transform.parent as RectTransform;
        _maxWidth = _rectTransform.sizeDelta.x;
    }
    public void UpdateTextSize(string text)
    {
        _tmpText.text = text;
        _tmpText.ForceMeshUpdate();
        Vector2 preferredSize = _tmpText.GetPreferredValues(_maxWidth, float.PositiveInfinity);
        _rectTransform.sizeDelta = new Vector2(
            _maxWidth + _padding.x,
            preferredSize.y + _padding.y);
        
        _parentRectTransform.sizeDelta = new Vector2(
            _parentRectTransform.sizeDelta.x,
            preferredSize.y + _padding.y
        );
    }
}