
using System;
using UnityEngine;

public class PanelHeightHandler
{
    private readonly float _minSymbol = 70f;
    private readonly float _maxSymbol = 175f;
    private readonly float _minHeight = 700f;
    private readonly float _maxHeight = 910f;
    private readonly RectTransform _rectTransform;
    private AnimationCurve _animationCurve;

    public PanelHeightHandler(RectTransform rectTransform)
    {
        _rectTransform = rectTransform;
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