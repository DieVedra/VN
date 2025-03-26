
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MyScrollScaler
{
    private readonly IReadOnlyList<Transform> _transformsContentChilds;
    private readonly ReactiveProperty<int> _currentIndexRP;
    private readonly ReactiveProperty<bool> _outsideLeftBorder;
    private readonly ReactiveProperty<bool> _outsideRightBorder;
    private readonly AnimationCurve _scaleCurveHide;
    private readonly AnimationCurve _scaleCurveUnhide;
    private readonly float _scaleHide;
    private readonly float _scaleUnhide;
    
    private int _current, _nextIndex;
    private Vector2 _valueVector2;
    private int _currentIndex => _currentIndexRP.Value;

    public MyScrollScaler(IReadOnlyList<Transform> transformsContentChilds,
        ReactiveProperty<int> currentIndexRP, ReactiveProperty<bool> outsideLeftBorder, ReactiveProperty<bool> outsideRightBorder,
        AnimationCurve scaleCurveHide, AnimationCurve scaleCurveUnhide,
        float scaleHide, float scaleUnhide)
    {
        _transformsContentChilds = transformsContentChilds;
        _currentIndexRP = currentIndexRP;
        _scaleCurveHide = scaleCurveHide;
        _scaleCurveUnhide = scaleCurveUnhide;
        _scaleHide = scaleHide;
        _scaleUnhide = scaleUnhide;
        _outsideLeftBorder = outsideLeftBorder;
        _outsideRightBorder = outsideRightBorder;
        _valueVector2 = new Vector2();
    }
    
    public void SetScaleImmediately()
    {
        for (int i = 0; i < _transformsContentChilds.Count; i++)
        {
            if (_currentIndex == i)
            {
                SetScale(_scaleUnhide, _currentIndex);
            }
            else
            {
                SetScale(_scaleHide, i);
            }
        }
    }

    public void SetCurrentIndexes(bool isRightMoveSwipe, int current, int nextIndex)
    {
        if (isRightMoveSwipe)
        {
            _current = current;
            _nextIndex = nextIndex;
        }
        else
        {
            _current = nextIndex;
            _nextIndex = current;
        }
    }
    public void SetScaleSmooth(float value)
    {
        float tHide = _scaleCurveHide.Evaluate(value);
        float tUnhide = _scaleCurveUnhide.Evaluate(value);
        if (_outsideRightBorder.Value == false && _outsideLeftBorder.Value == false)
        {
            SetScale(Mathf.Lerp(_scaleHide, _scaleUnhide, tHide), _current);
            SetScale(Mathf.Lerp(_scaleHide, _scaleUnhide,  tUnhide), _nextIndex);
        }
        else
        if (_outsideRightBorder.Value == true)
        {
            SetScale(Mathf.Lerp(_scaleHide, _scaleUnhide, tHide), _current);
        }
        else if (_outsideLeftBorder.Value == true)
        {
            SetScale(Mathf.Lerp( _scaleHide,_scaleUnhide, tUnhide), _current);
        }
    }

    private void SetScale(float scale, int index)
    {
        _valueVector2.x = scale;
        _valueVector2.y = scale;
        _transformsContentChilds[index].localScale = _valueVector2;
    }

    public void SetScaleSmoothWhenOnePanel(float time)
    {
        float tUnhide = 0f;
        if (time >= 0.5f)
        {
            tUnhide = _scaleCurveHide.Evaluate(Mathf.InverseLerp(0.5f, 1f, time));
        }
        else if (time < 0.5f)
        {
            tUnhide = _scaleCurveUnhide.Evaluate(Mathf.InverseLerp(0f, 0.5f, time));
        }
        SetScale(Mathf.Lerp( _scaleHide,_scaleUnhide, tUnhide), 0);
    }
}