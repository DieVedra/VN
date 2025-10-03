using System;
using TMPro;
using UnityEngine;

public class PanelSizeHandler
{
    private readonly LineBreaksCountCalculator _lineBreaksCountCalculator;
    private readonly AnimationCurve _panelHeightAnimationCurve;
    private Vector2 _size = new Vector2();

    public PanelSizeHandler(LineBreaksCountCalculator lineBreaksCountCalculator, CurveProvider curveProvider)
    {
        _lineBreaksCountCalculator = lineBreaksCountCalculator;
        _panelHeightAnimationCurve = curveProvider.GetCurve();
    }

    public void UpdateSize(RectTransform rectTransform, TextMeshProUGUI textComponent, string text, bool clear = true)
    {
        if (String.IsNullOrEmpty(text) == false)
        {
            _size.x = rectTransform.sizeDelta.x;
            _size.y = _panelHeightAnimationCurve.Evaluate(_lineBreaksCountCalculator.GetLineBreaksCount(textComponent, text, clear));
            rectTransform.sizeDelta = _size;
        }
    }
}