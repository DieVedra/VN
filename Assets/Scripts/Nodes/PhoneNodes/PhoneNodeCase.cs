using System;
using UnityEngine;

[Serializable]
public class PhoneNodeCase
{
    [SerializeField] private string _portName;
    [SerializeField] private string _contactKey;
    [SerializeField] private int _contactIndex;
    [field: SerializeField] public string ContactName { get; private set; }

    public string PortName => _portName;
    public string ContactKey => _contactKey;
    public int ContactIndex => _contactIndex;


    public PhoneNodeCase(string contactKey, string contactName, string portName, int contactIndex)
    {
        _contactKey = contactKey;
        ContactName = contactName;
        _portName = portName;
        _contactIndex = contactIndex;
    }
}