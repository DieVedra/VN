using System;
using UnityEngine;

public class PanelSizeHandler
{
    private readonly RectTransform _rectTransform;
    private readonly AnimationCurve _panelHeightAnimationCurve;

    public PanelSizeHandler(RectTransform rectTransform, CurveProvider curveProvider)
    {
        _rectTransform = rectTransform;
        _panelHeightAnimationCurve = curveProvider.GetCurve();
    }

    public void UpdateSize(string text, int lineBreaks)
    {
        if (String.IsNullOrEmpty(text) == false)
        {
            _rectTransform.sizeDelta = new Vector2(_rectTransform.sizeDelta.x, _panelHeightAnimationCurve.Evaluate(lineBreaks));
        }
    }
}