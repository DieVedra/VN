using System;
using UnityEngine;

[Serializable]
public class PhoneMessage
{
    [SerializeField] private string _text;
    public bool IsReaded;
    private LocalizationString _textMessage;

    public PhoneMessage(string text, bool isReaded)
    {
        _text = text;
        IsReaded = isReaded;
    }

    public LocalizationString LocalizationString
    {
        get
        {
            if (_textMessage == null)
            {
                _textMessage = new LocalizationString(_text);
            }
            else
            {
                _textMessage.SetText(_text);
                _textMessage.GenerateStableHash();
            }
            return _textMessage;
        }
    }
}
[Serializable]
public class PhoneMessageWithLocalization
{
    [SerializeField] private string _text;
    public bool IsReaded;
    private LocalizationString _textMessage;

    public PhoneMessageWithLocalization(string text, bool isReaded)
    {
        _text = text;
        IsReaded = isReaded;
    }

    public LocalizationString LocalizationString
    {
        get
        {
            if (_textMessage == null)
            {
                _textMessage = new LocalizationString(_text);
            }
            else
            {
                _textMessage.SetText(_text);
                _textMessage.GenerateStableHash();
            }
            return _textMessage;
        }
    }
}
public interface IPhoneMessage
{
    
}