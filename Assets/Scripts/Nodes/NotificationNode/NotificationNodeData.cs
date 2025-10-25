using System;
using UnityEngine;

[Serializable]
public class NotificationNodeData
{
    [SerializeField] private float _showTime = 1.5f;
    [SerializeField] private float _delayDisplayTime = 0f;
    [SerializeField] private Color _color = Color.white;

    public NotificationNodeData(Color color, float showTime, float delayDisplayTime)
    {
        _color = color;
        _showTime = showTime;
        _delayDisplayTime = delayDisplayTime;
    }

    public Color Color => _color;
    public float DelayDisplayTime => _delayDisplayTime;
    public float ShowTime => _showTime;
}