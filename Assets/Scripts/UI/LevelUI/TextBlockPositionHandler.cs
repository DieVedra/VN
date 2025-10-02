using System;
using TMPro;
using UnityEngine;

public class TextBlockPositionHandler
{
    private readonly LineBreaksCountCalculator _lineBreaksCountCalculator;
    private readonly AnimationCurve _textPositionAnimationCurve;
    private Vector2 _size = new Vector2();

    public TextBlockPositionHandler(LineBreaksCountCalculator lineBreaksCountCalculator, CurveProvider curveProvider)
    {
        _lineBreaksCountCalculator = lineBreaksCountCalculator;
        _textPositionAnimationCurve = curveProvider.GetCurve();
    }

    public void UpdatePosition(RectTransform textRectTransform, TextMeshProUGUI textComponent, string text)
    {
        if (String.IsNullOrEmpty(text) == false)
        {
            _size.x = textRectTransform.anchoredPosition.x;
            _size.y = _textPositionAnimationCurve.Evaluate(_lineBreaksCountCalculator.GetLineBreaksCount(textComponent, text));
            textRectTransform.anchoredPosition = _size;
        }
    }
}