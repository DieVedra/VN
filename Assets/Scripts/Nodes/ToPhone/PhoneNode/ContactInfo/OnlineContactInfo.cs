using System;
using UnityEngine;

[Serializable]
public class OnlineContactInfo : ContactInfo
{
    [SerializeField] private bool _isOfflineOnEndKey;
    
    public bool IsOfflineOnEndKey => _isOfflineOnEndKey;

    public OnlineContactInfo(string key, string name, bool isOfflineOnEndKey) : base(key, name)
    {
        _isOfflineOnEndKey = isOfflineOnEndKey;
    }
}