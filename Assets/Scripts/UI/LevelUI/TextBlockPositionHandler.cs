using System;
using UnityEngine;

public class TextBlockPositionHandler
{
    private readonly RectTransform _textRectTransform;
    private readonly AnimationCurve _textPositionAnimationCurve;

    public TextBlockPositionHandler(RectTransform textRectTransform, CurveProvider curveProvider)
    {
        _textRectTransform = textRectTransform;
        _textPositionAnimationCurve = curveProvider.GetCurve();
    }

    public void UpdatePosition(string text, int lineBreaks)
    {
        if (String.IsNullOrEmpty(text) == false)
        {
            Debug.Log($"lineBreaks {lineBreaks}     {_textPositionAnimationCurve.Evaluate(lineBreaks)}");
            _textRectTransform.anchoredPosition = new Vector2(_textRectTransform.anchoredPosition.x, _textPositionAnimationCurve.Evaluate(lineBreaks));
        }
    }
}