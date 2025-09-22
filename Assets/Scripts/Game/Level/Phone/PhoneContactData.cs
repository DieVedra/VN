using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhoneContactData
{
    [field: SerializeField] public string Name { get; private set; }

    // [field: SerializeField] public string NameKey { get; private set; }

    [field: SerializeField] public Sprite Icon  { get; private set; }

    [field: SerializeField] public bool WithIcon  { get; private set; }

    [SerializeField] private List<PhoneMessage> _phoneMessages;

    private LocalizationString _nameContact;

    public LocalizationString LocalizationString
    {
        get
        {
            if (_nameContact == null)
            {
                _nameContact = new LocalizationString(Name);
            }
            if (string.IsNullOrEmpty(_nameContact.DefaultText))
            {
                _nameContact.SetText(Name);
            }
            if (string.IsNullOrEmpty(_nameContact.Key))
            {
                _nameContact.GenerateStableHash();
            }
            return _nameContact;
        }
    }

    public IReadOnlyList<PhoneMessage> PhoneMessages => _phoneMessages;

    public PhoneContactData(string name, Sprite icon)
    {
        Name = name;
        Icon = icon;
    }

    public void AddMessages(IReadOnlyList<PhoneMessage> phoneMessages)
    {
        _phoneMessages.AddRange(phoneMessages);
    }
}