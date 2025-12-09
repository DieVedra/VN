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
    public int ContactIndex => _contactIndex;
    public bool IsReaded = false;
    public NodePort Port
    {
        get => _port;
        set => _port = value;
    }

    public ContactNodeCase(NodePort port, int contactIndex, string key, string name, string portName)
        : base(key, name)
    {
        _port = port;
        _portName = portName;
        _contactIndex = contactIndex;
    }
}