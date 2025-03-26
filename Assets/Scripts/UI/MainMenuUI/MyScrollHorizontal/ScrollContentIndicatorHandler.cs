
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ScrollContentIndicatorHandler
{
    private readonly float _posAContent;
    private readonly float _posBContent;
    private readonly float _posAIndicator;
    private readonly float _posBIndicator;
    private readonly RectTransform _content;
    private readonly RectTransform _swipeIndicatorFill;

    private Vector2 _position;
    
    public ScrollContentIndicatorHandler(List<float> contentChildsPosX,
        RectTransform swipeIndicatorFill,
        RectTransform content, RectTransform swipeProgressIndicatorParent,
        Func<float,int, float> calculateAddValueToPositionXOperation, int contentCount, float moveStepIndicator, float moveStep)
    {
        _position = new Vector2();
        _content = content;
        _swipeIndicatorFill = swipeIndicatorFill;
        _swipeIndicatorFill.anchoredPosition = swipeProgressIndicatorParent.anchoredPosition;
        _position = _swipeIndicatorFill.anchoredPosition;

        if (contentCount == 1)
        {
            _posAContent = moveStep;
            _posBContent = -moveStep;
            _posAIndicator = moveStepIndicator;
            _posBIndicator = -moveStepIndicator;
        }
        else
        {
            _posAContent = contentChildsPosX[0];
            _posBContent = contentChildsPosX[contentCount - 1];
            float value = calculateAddValueToPositionXOperation.Invoke(moveStepIndicator, contentCount);
            value = _swipeIndicatorFill.anchoredPosition.x - value;
            _swipeIndicatorFill.anchoredPosition = new Vector2(value, _position.y);
            _posAIndicator = value;
            _posBIndicator = _posAIndicator;
            for (int i = 0; i < contentCount -1; i++)
            {
                _posBIndicator += moveStepIndicator;
            }
        }

        UpdateIndicator();
    }
    public void UpdateIndicator()
    {
        float posContent = InverseLerpUnclamped(_posAContent, _posBContent, _content.anchoredPosition.x);
        _position.x = Mathf.LerpUnclamped(_posAIndicator, _posBIndicator, posContent);
        _swipeIndicatorFill.anchoredPosition = _position;
    }
    private float InverseLerpUnclamped(float a, float b, float value)
    {
        return (value - a) / (b - a);
    }
}