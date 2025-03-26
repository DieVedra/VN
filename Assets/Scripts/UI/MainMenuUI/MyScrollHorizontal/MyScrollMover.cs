using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class MyScrollMover
{
    private readonly float _timeMultiplier = 2;
    private readonly float _rightMoveTimeBorder = 0.2f;
    private readonly float _leftMoveTimeBorder = 0.8f;
    private readonly float _moveStep;
    private readonly RectTransform _content;
    private readonly MyScrollScaler _myScrollScaler;
    private readonly ChangeEffectHandler _changeEffectHandler;
    private readonly ScrollContentIndicatorHandler _scrollContentIndicatorHandler;
    private readonly ReactiveProperty<int> _currentIndexRP;
    private readonly ReactiveProperty<bool> _isRightMoveRP;
    private readonly ReactiveProperty<bool> _outsideLeftBorder;
    private readonly ReactiveProperty<bool> _outsideRightBorder;
    private IndexFinder _indexFinder;
    private bool _isRightMoveSwipe;
    private Vector2 _valueVector2;
    private int _contentCount;
    private int _nextIndexForScale;
    private int _currentIndexForScale;
    private float _posB;
    private float _posA;
    private float _time;
    private float _previousSwipeValue;
    private bool _panelCloserIsWorking;

    private Action<bool,float> _closerOperation;
    private IReadOnlyList<float> _contentChildsPosX;
    private CompositeDisposable _compositeDisposableCloser;

    public bool IsMove { get; private set; }
    private int _currentIndex => _currentIndexRP.Value;
    private float _myDeltaTimeRight => Time.deltaTime * _timeMultiplier;
    private float _myDeltaTimeLeft => -Time.deltaTime * _timeMultiplier;
    
    
    public MyScrollMover(ReactiveProperty<bool> isRightMoveRP, ReactiveProperty<int> currentIndexRP,
        ReactiveProperty<bool> outsideLeftBorder, ReactiveProperty<bool> outsideRightBorder,
        RectTransform content, ScrollContentIndicatorHandler scrollContentIndicatorHandler, IReadOnlyList<float> contentChildsPosX,
        MyScrollScaler myScrollScaler, ChangeEffectHandler changeEffectHandler, float moveStep, int contentCount, int startIndex)
    {
        _indexFinder = new IndexFinder(contentChildsPosX, currentIndexRP, content, moveStep);
        _valueVector2 = new Vector2();
        _outsideLeftBorder = outsideLeftBorder;
        _outsideLeftBorder.Value = false;
        _outsideRightBorder = outsideRightBorder;
        _outsideRightBorder.Value = false;
        _content = content;
        _scrollContentIndicatorHandler = scrollContentIndicatorHandler;
        _isRightMoveRP = isRightMoveRP;
        _time = 0f;
        _currentIndexRP = currentIndexRP;
        _contentChildsPosX = contentChildsPosX;
        _myScrollScaler = myScrollScaler;
        _changeEffectHandler = changeEffectHandler;
        _moveStep = moveStep;
        _contentCount = contentCount;
        _myScrollScaler.SetScaleImmediately();
        _valueVector2.y = _content.anchoredPosition.y;
        _closerOperation = null;
        _currentIndexForScale = startIndex;
        if (contentCount > 1)
        {
            _time = 0f;
            _posA = _contentChildsPosX[_currentIndex];
            _posB = GetNextRightPositionX();
            _myScrollScaler.SetCurrentIndexes(true,_currentIndexForScale, _nextIndexForScale);
            _closerOperation = MoveFromSwipe;
        }
        else
        {
            _time = 0.5f;
            _posA = moveStep;
            _posB = -moveStep;
            _closerOperation = MoveWhenOnePanel;
        }

        _previousSwipeValue = 0f;
    }
    public void MoveFromSwipe(bool swipeDirectionKey, float swipeValue)
    {
        _time += swipeValue;
        _previousSwipeValue = swipeValue;
        IsMove = true;
        if (_time < 0f) // move left
        {
            _indexFinder.FindAndSet();
            _posB = GetCurrentPositionX();

            if (_outsideLeftBorder.Value == false)
            {
                _time = 1f;
            }
            else
            {
                _time = 0f;
            }

            _posA = GetNextLeftPositionX();
            _myScrollScaler.SetCurrentIndexes(swipeDirectionKey, _currentIndexForScale, _nextIndexForScale);
            TrySetCloserOff();
            IsMove = false;
        }
        else if (_time > 1f)
        {
            _indexFinder.FindAndSet();

            _posA = GetCurrentPositionX();
            if ((_outsideLeftBorder.Value == false && _currentIndex == 0) || _outsideRightBorder.Value == true)
            {
                _time = 1f;}
            else
            {
                _time = 0f;
            }

            _posB = GetNextRightPositionX();
            _myScrollScaler.SetCurrentIndexes(swipeDirectionKey, _currentIndexForScale, _nextIndexForScale);
            TrySetCloserOff();
            IsMove = false;
        }
        _scrollContentIndicatorHandler.UpdateIndicator();

        _isRightMoveSwipe = swipeDirectionKey;
        _valueVector2.x = Mathf.Lerp(_posA, _posB,_time);
        
        _myScrollScaler.SetScaleSmooth(_time);
        _content.anchoredPosition = _valueVector2;
    }

    public void MoveWhenOnePanel(bool directionKeyRight, float swipeValue)
    {
        _time += swipeValue;
        _previousSwipeValue = swipeValue;
        _time = Mathf.Clamp(_time, 0f, 1f);
        IsMove = true;
        if (_panelCloserIsWorking == true)
        {
            if (directionKeyRight == true && _time > 0.5f)
            {
                TrySetCloserOff();
                _time = 0.5f;
                IsMove = false;
            }
            else if(directionKeyRight == false && _time < 0.5f)
            {
                TrySetCloserOff();
                _time = 0.5f;
                IsMove = false;
            }
        }

        _valueVector2.x = Mathf.Lerp(_posA, _posB,_time);
        _content.anchoredPosition = _valueVector2;
        _scrollContentIndicatorHandler.UpdateIndicator();
        _myScrollScaler.SetScaleSmoothWhenOnePanel(_time);
    }
    
    public void MoveImmediately()
    {
        if (_isRightMoveRP.Value && _currentIndex < _contentCount - 1)
        {
            _currentIndexRP.Value++;
            _content.anchoredPosition = new Vector2(_contentChildsPosX[_currentIndex], _content.anchoredPosition.y);

            _scrollContentIndicatorHandler.UpdateIndicator();
        }
        else if(!_isRightMoveRP.Value && _currentIndexRP.Value != 0)
        {
            _currentIndexRP.Value--;
            _content.anchoredPosition = new Vector2(_contentChildsPosX[_currentIndex], _content.anchoredPosition.y);


            _scrollContentIndicatorHandler.UpdateIndicator();
        }
        _myScrollScaler.SetScaleImmediately();
    }

    public void OnPress()
    {
        TrySetCloserOff();
    }
    public void StartPanelCloserWhenOnePanel()
    {
        if (_previousSwipeValue != 0f)
        {
            if (_time > 0.5f)
            {
                RightCloser();
            }
            else if (_time < 0.5f)
            {
                LeftCloser();
            }
        }
    }
    public void StartPanelCloser()
    {
        if (_previousSwipeValue != 0f)
        {
            if (_time >= 0f)
            {
                if (_isRightMoveSwipe)
                {
                    if (_currentIndex == _contentChildsPosX.Count - 1)
                    {
                        LeftCloser();
                    }
                    else if (_time > _rightMoveTimeBorder)
                    {
                        RightCloser();
                    }
                    else if(_time < _rightMoveTimeBorder)
                    {
                        LeftCloser();
                    }
                }
                else
                {
                    if (_currentIndex == 0)
                    {
                        LeftCloser();
                    }
                    else if (_time > _leftMoveTimeBorder)
                    {
                        LeftCloser();
        
                    }
                    else if(_time < _leftMoveTimeBorder)
                    {
                        RightCloser();
                    }
                }
            }
        }
    }

    private void InitStartPress()
    {
        _posA = _contentChildsPosX[_currentIndex];
    }

    private void RightCloser()
    {
        StartCloser(_isRightMoveSwipe);
    }

    private void LeftCloser()
    {
        StartCloser(!_isRightMoveSwipe);
    }
    private void StartCloser(bool moveKey)
    {
        _panelCloserIsWorking = true;
        _compositeDisposableCloser = new CompositeDisposable();
        Observable.EveryUpdate().Subscribe(_ =>
        {
            _closerOperation.Invoke(moveKey, moveKey ? _myDeltaTimeRight : _myDeltaTimeLeft);
        }).AddTo(_compositeDisposableCloser);
    }
    private float GetNextLeftPositionX()
    {
        if (_currentIndex - 1 >= 0)
        {
            _outsideRightBorder.Value = false;
            _nextIndexForScale = _currentIndex - 1;
            return _contentChildsPosX[_currentIndex - 1];
        }
        else
        {
            _outsideLeftBorder.Value = true;
            _nextIndexForScale = _currentIndex;
            return _contentChildsPosX[_currentIndex] + _moveStep;
        }
    }
    private float GetNextRightPositionX()
    {
        if (_currentIndex  + 1 < _contentChildsPosX.Count)
        {
            _outsideLeftBorder.Value = false;
            _nextIndexForScale = _currentIndex + 1;
            return _contentChildsPosX[_currentIndex + 1];
        }
        else
        {
            _outsideRightBorder.Value = true;
            _nextIndexForScale = _currentIndex;
            return _contentChildsPosX[_currentIndex] - _moveStep;
        }
    }

    private float GetCurrentPositionX()
    {
        _currentIndexForScale = _currentIndex;
        return _contentChildsPosX[_currentIndex];
    }

    private void TrySetCloserOff()
    {
        if (_panelCloserIsWorking == true)
        {
            _panelCloserIsWorking = false;
            _changeEffectHandler.PlayEffect().Forget();
            _compositeDisposableCloser.Clear();
            _previousSwipeValue = 0f;
        }
    }
    
}