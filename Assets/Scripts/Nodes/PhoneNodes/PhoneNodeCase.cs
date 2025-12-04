using System;
using UnityEngine;
using XNode;

[Serializable]
public class ContactNodeCase : ContactInfo
{
    [SerializeField] private string _portName;
    [SerializeField] private int _contactIndex;
    [SerializeField] private NodePort _port;

    public string PortName => _portName;
    public NodePort Port => _port;


    public ContactNodeCase(NodePort port, int contactIndex, string key, string name, string portName)
        : base(key, name)
    {
        _port = port;
        _portName = portName;
        _contactIndex = contactIndex;
    }
}