using System;
using UnityEngine;

[Serializable]
public class ContactInfoToGame
{
    [SerializeField] private string _keyName;
    [SerializeField] private string _name;
    [SerializeField] private bool _keyOnline;
    [SerializeField] private bool _keyNotification;
    public string KeyName => _keyName;

    public string Name => _name;

    public bool KeyOnline
    {
        get => _keyOnline;
        set => _keyOnline = value;
    }
    public bool KeyNotification
    {
        get => _keyNotification;
        set => _keyNotification = value;
    }
    public ContactInfoToGame(string keyName, string name, bool keyOnline, bool keyNotification)
    {
        _keyName = keyName;
        _name = name;
        _keyOnline = keyOnline;
        _keyNotification = keyNotification;
    }
}