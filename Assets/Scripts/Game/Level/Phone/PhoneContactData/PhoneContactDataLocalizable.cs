using System.Collections.Generic;
using UnityEngine;

public class PhoneContactDataLocalizable
{
    private readonly LocalizationString _nameContact;
    private readonly Sprite _icon; 
    private readonly List<PhoneMessageLocalization> _phoneMessages;
    private readonly Color _colorIcon;
    private readonly bool _isEmptyIconKey;
    public Sprite Icon => _icon;
    public Color ColorIcon => _colorIcon;
    public bool IsEmptyIconKey => _isEmptyIconKey;
    public LocalizationString NameContactLocalizationString => _nameContact;
    public IReadOnlyList<PhoneMessageLocalization> PhoneMessagesLocalization => _phoneMessages;

    public PhoneContactDataLocalizable(string name, Sprite icon, bool isEmptyIconKey)
    {
        _nameContact = new LocalizationString(name);
        _icon = icon;
        _isEmptyIconKey = isEmptyIconKey;
        if (isEmptyIconKey == true)
        {
            _colorIcon = PhoneContactsColors.GetColor();
            
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
            _phoneMessages.Add(new PhoneMessageLocalization(phoneMessages[i].Text, phoneMessages[i].Type, 
                keyMessagesReadedBySeria == true || phoneMessages[i].IsReaded));
        }
    }
}