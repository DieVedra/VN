
using System;
using UnityEngine;

[Serializable]
public class Stat : BaseStat
{
    [SerializeField] private Color _colorField;
    [SerializeField] private bool _showKey;

    public Color ColorField => _colorField;
    public bool ShowKey => _showKey;

    public Stat(string name, int value, bool showKey, Color colorField) : base(name, value)
    {
        _colorField = colorField;
        _showKey = showKey;
    }
}