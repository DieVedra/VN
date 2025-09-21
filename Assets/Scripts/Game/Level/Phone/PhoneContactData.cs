using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhoneContactData
{
    [field: SerializeField] public string Name { get; private set; }
    // [field: SerializeField] public string NameKey { get; private set; }

    [SerializeField] private Sprite _icon;
    [SerializeField] private bool _withIcon;
    [SerializeField] private List<PhoneMessage> PhoneMessages;
    private LocalizationString _nameContact;

    public LocalizationString LocalizationString
    {
        get
        {
            if (_nameContact == null)
            {
                _nameContact = new LocalizationString(Name);
            }
            else
            {
                _nameContact.SetText(Name);
            }
            return _nameContact;
        }
    }
}