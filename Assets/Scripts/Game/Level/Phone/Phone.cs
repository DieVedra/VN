using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class Phone
{
    [field: SerializeField] public LocalizationString NamePhone;

    [field: SerializeField] public Sprite PhoneFrame { get; private set; }

    [field: SerializeField] public Sprite Background { get; private set; }

    [SerializeField] private Sprite[] _hands;

    [SerializeField] private List<PhoneContact> _phoneContactDatas;

    public IReadOnlyList<Sprite> Hands => _hands;

    // public IReadOnlyList<PhoneContact> PhoneContactDatas => _phoneContactDatas;

    public List<PhoneContact> PhoneContactDatas => _phoneContactDatas;

    public Phone(LocalizationString namePhone, IReadOnlyList<Sprite> hands, Sprite phoneFrame, Sprite background)
    {
        NamePhone = namePhone;
        _hands = hands.ToArray();
        PhoneFrame = phoneFrame;
        Background = background;
        _phoneContactDatas = new List<PhoneContact>();
    }

    public void AddContact(params PhoneContact[] contacts)
    {
        if (_phoneContactDatas == null)
        {
            _phoneContactDatas = new List<PhoneContact>();
        }
        _phoneContactDatas.AddRange(contacts);
    }
}