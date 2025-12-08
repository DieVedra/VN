using System;
using UnityEngine;

[Serializable]
public class ContactInfo
{
    [SerializeField] protected string Key;
    [SerializeField] protected string Name;

    public string ContactKey => Key;
    public string ContactName => Name;
    
    public ContactInfo(string key, string name)
    {
        Key = key;
        Name = name;
    }
}