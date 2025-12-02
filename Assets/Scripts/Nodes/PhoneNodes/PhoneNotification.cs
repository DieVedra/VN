using System;
using UnityEngine;

[Serializable]
public class PhoneNotification
{
    [SerializeField] private string _contactKey;
    [SerializeField] private string _contactName;

    public string ContactKey => _contactKey;
    public string ContactName => _contactName;
    
    public PhoneNotification(string contactKey, string contactName)
    {
        _contactKey = contactKey;
        _contactName = contactName;
    }
}