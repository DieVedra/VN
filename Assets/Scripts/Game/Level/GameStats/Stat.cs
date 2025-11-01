
using System;
using UnityEngine;

[Serializable]
public class Stat : BaseStat
{
    [SerializeField] private bool _showInEndGameResultKey;
    public bool ShowInEndGameResultKey => _showInEndGameResultKey;

    public Stat(string name, int value, Color colorField) : base(name, value, colorField) { }
    public Stat(string name, string key, int value, Color colorField) : base(name, key, value, colorField) { }
}