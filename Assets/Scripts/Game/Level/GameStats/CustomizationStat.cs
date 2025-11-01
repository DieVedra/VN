using System;
using UnityEngine;

[Serializable]
public class CustomizationStat : BaseStat
{
    [SerializeField] private bool _showKey;
    public int CustomizationStatValue { get => _value; set => _value = value; }
    public bool CustomizationStatNotificationKey { get => _notificationKey; set => _notificationKey = value; }
    public bool ShowKey { get => _showKey; set => _showKey = value; }

    public CustomizationStat(string name, string key, int value, bool notificationKey, Color colorField)
        : base(name, key, value, notificationKey, colorField) { }
}