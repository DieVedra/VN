using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class Phone
{
    [field: SerializeField] public LocalizationString NamePhone;
    [field: SerializeField] public Sprite PhoneFrame { get; private set; }
    [field: SerializeField] public Sprite Background { get; private set; }
    
    [SerializeField] private Sprite[] _hands;
    private List<PhoneContact> _phoneContactDatas;
    
    private IReadOnlyList<Sprite> Hands => _hands;
    private IReadOnlyList<PhoneContact> PhoneContactDatas => _phoneContactDatas;
    public void AddContact(params PhoneContact[] contacts)
    {
        if (_phoneContactDatas == null)
        {
            _phoneContactDatas = new List<PhoneContact>();
        }
        _phoneContactDatas.AddRange(contacts);
    }
}