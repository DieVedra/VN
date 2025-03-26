
using System;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class IndexFinder
{
    private readonly RectTransform _content;
    private readonly ReactiveProperty<int> _currentIndexRP;
    private IReadOnlyList<float> _contentChildsPosX;
    private float _offset;

    public IndexFinder(IReadOnlyList<float> contentChildsPosX, ReactiveProperty<int> currentIndexRP, RectTransform content, float moveStep)
    {
        _contentChildsPosX = contentChildsPosX;
        _currentIndexRP = currentIndexRP;
        _content = content;
        _offset = moveStep * 0.5f;
    }

    public void FindAndSet()
    {
        for (int i = 0; i < _contentChildsPosX.Count; i++)
        {
            if (Math.Abs(_contentChildsPosX[i] - _content.anchoredPosition.x) < _offset)
            {
                _currentIndexRP.Value = i;
                return;
            }
        }
        
    }
}