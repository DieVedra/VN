using System;
using UnityEngine;
using XNode;

[Serializable]
public class NotificationContactInfo : ContactInfo
{
    [SerializeField] private string _portName;
    [SerializeField] private NodePort _port;
    [SerializeField] private PhoneContact _contact;
    public string PortName => _portName;
    public NodePort Port
    {
        get => _port;
        set => _port = value;
    }
    public PhoneContact Contact
    {
        get => _contact;
        set => _contact = value;
    }
    public NotificationContactInfo(string portName, string key, string name) : base(key, name)
    {
        _portName = portName;
    }
}