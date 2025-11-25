using System;
using UnityEngine;

[Serializable]
public class PhoneMessage
{
    private string _text;
    private PhoneMessageType _type;
    public PhoneMessage(string text, PhoneMessageType type)
    {
        _text = text;
        _type = type;
    }

    [field: SerializeField] public string Text { get; private set; }
    [field: SerializeField] public PhoneMessageType Type { get; private set; }

    public bool IsReaded;
    
    
}