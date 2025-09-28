using System;
using UnityEngine;

[Serializable]
public class ContactInfoToOnlineStatus
{
    [SerializeField] private string _name;
    [SerializeField] private string _key;
    [SerializeField] private bool _onlineKey;
    public string Key => _key;
    public string Name => _name;
    public bool OnlineKey => _onlineKey;

    public ContactInfoToOnlineStatus(string name, string key, bool onlineKey)
    {
        _name = name;
        _key = key;
        _onlineKey = onlineKey;
    }
}