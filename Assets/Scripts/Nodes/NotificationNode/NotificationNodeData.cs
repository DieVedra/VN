using System;
using UnityEngine;

[Serializable]
public class NotificationNodeData
{
    public const float _defaultShowTime = 1.5f;
    public const float _defaultDelayDisplayTime = 0f;
    [SerializeField] private float _showTime;
    [SerializeField] private float _delayDisplayTime;
    [SerializeField] private Color _color;

    public float ShowTime
    {
        get
        {
            if (_showTime <= 0f)
            {
                _showTime = _defaultShowTime;
            }

            return _showTime;
        }
        set => _showTime = value;
    }

    public float DelayDisplayTime
    {
        get => _delayDisplayTime;
        set => _delayDisplayTime = value;
    }

    public Color Color
    {
        get
        {
            if (_color == Color.clear)
            {
                _color = Color.white;
            }

            return _color;
        }
        set => _color = value;
    }

    public void Reset()
    {
        if (_showTime <= _defaultDelayDisplayTime)
        {
            _showTime = _defaultShowTime;
        }
        if (_color == Color.clear)
        {
            _color = Color.white;
        }
        if (_delayDisplayTime < _defaultDelayDisplayTime)
        {
            _delayDisplayTime = _defaultDelayDisplayTime;
        }
    }
}