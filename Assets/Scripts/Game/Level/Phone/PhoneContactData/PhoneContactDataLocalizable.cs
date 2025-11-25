using System.Collections.Generic;
using UnityEngine;

public class PhoneContactDataLocalizable : ILocalizable
{
    private const int _defaultIndexAddedInitially = -1; 
    public readonly LocalizationString NameContact;
    public readonly LocalizationString NikNameContact;
    private readonly Sprite _icon; 
    private readonly List<PhoneMessageLocalization> _phoneMessages;
    private readonly Color _colorIcon;
    private readonly bool _isEmptyIconKey;
    private readonly bool _isAddebleContactKey;
    public int IndexSeriaInWhichContactWasAdded = _defaultIndexAddedInitially;
    public Sprite Icon => _icon;
    public Color ColorIcon => _colorIcon;
    public bool IsEmptyIconKey => _isEmptyIconKey;
    public bool IsAddebleContactKey => _isAddebleContactKey;
    public IReadOnlyList<PhoneMessageLocalization> PhoneMessagesLocalization => _phoneMessages;
    public PhoneMessagesGraph PhoneMessagesGraph;
    

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        for (int i = 0; i < _phoneMessages.Count; i++)
        {
            strings.Add(_phoneMessages[i].TextMessage);
        }
        strings.Add(NameContact);
        strings.Add(NikNameContact);

        return strings;
    }

    public PhoneContactDataLocalizable(string name, string nikName, Sprite icon, Color color, bool isEmptyIconKey, bool isAddebleContactKey = false)
    {
        NameContact = new LocalizationString(name);
        NikNameContact = new LocalizationString(nikName);
        _icon = icon;
        _isEmptyIconKey = isEmptyIconKey;
        _isAddebleContactKey = isAddebleContactKey;
        if (isEmptyIconKey == true)
        {
            _colorIcon = color;
            
        }
        _phoneMessages = new List<PhoneMessageLocalization>();
    }

    public void AddMessages(IReadOnlyList<PhoneMessageLocalization> phoneMessages)
    {
        _phoneMessages.AddRange(phoneMessages);
    }

    public void AddMessages(IReadOnlyList<PhoneMessage> phoneMessages, bool keyMessagesReadedBySeria)
    {
        for (int i = 0; i < phoneMessages.Count; i++)
        {
            _phoneMessages.Add(CreatePhoneMessageLocalization(phoneMessages[i], keyMessagesReadedBySeria == true || phoneMessages[i].IsReaded));
        }
    }
    public void AddMessages(IReadOnlyList<PhoneMessage> phoneMessages)
    {
        for (int i = 0; i < phoneMessages.Count; i++)
        {
            _phoneMessages.Add(CreatePhoneMessageLocalization(phoneMessages[i]));
        }
    }
    // public void AddMessages(params PhoneMessageLocalization[] phoneMessages)
    // {
    //     _phoneMessages.AddRange(phoneMessages);
    // }

    public void AddMessages(IReadOnlyList<PhoneMessage> phoneMessages, int[] lastSeriaReadedMessagesIndexes)
    {
        bool keyRead = false;
        if (lastSeriaReadedMessagesIndexes != null && lastSeriaReadedMessagesIndexes.Length > 0)
        {
            for (int i = 0; i < phoneMessages.Count; i++)
            {
                keyRead = false;
                for (int l = 0; l < lastSeriaReadedMessagesIndexes.Length; l++)
                {
                    if (i == lastSeriaReadedMessagesIndexes[l])
                    {
                        keyRead = true;
                        break;
                    }
                }
                _phoneMessages.Add(CreatePhoneMessageLocalization(phoneMessages[i], keyRead));
            }
        }
        else
        {
            AddMessages(phoneMessages);
        }
    }

    private PhoneMessageLocalization CreatePhoneMessageLocalization(PhoneMessage phoneMessage, bool keyMessageReadedBySeria = false)
    {
        return new PhoneMessageLocalization(phoneMessage.Text, phoneMessage.Type, keyMessageReadedBySeria);
    }
}