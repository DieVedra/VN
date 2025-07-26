
using System;
using UnityEngine;

public class PanelHeightHandler
{
    private const float _minSymbol = 70f;
    private const float _maxSymbol = 175f;
    private const float _minHeight = 700f;
    private const float _maxHeight = 910f;
    private readonly RectTransform _rectTransform;
    private readonly RectTransform _textRectTransform;
    private AnimationCurve _animationCurve;

    public PanelHeightHandler(RectTransform rectTransform, RectTransform textRectTransform)
    {
        _rectTransform = rectTransform;
        _textRectTransform = textRectTransform;
        _animationCurve = new AnimationCurve(new Keyframe(_minSymbol,_minHeight), new Keyframe(_maxSymbol,_maxHeight));
    }

    public void UpdateHeight(string text)
    {
        if (String.IsNullOrEmpty(text) == false)
        {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _animationCurve.Evaluate(text.Length));
        }
    }
}