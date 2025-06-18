
using System;
using UnityEngine;

[Serializable]
public class Stat : BaseStat
{
    [SerializeField] private Color _colorField = Color.white;
    [SerializeField] private bool _showKey;
    [SerializeField] private bool _showInEndGameResultKey;

    public Color ColorField => _colorField;
    public bool ShowKey => _showKey;
    public bool ShowInEndGameResultKey => _showInEndGameResultKey;

    public Stat(string name, int value, bool showKey, Color colorField) : base(name, value)
    {
        _colorField = colorField;
        _showKey = showKey;
    }
}