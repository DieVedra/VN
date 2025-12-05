using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class Phone
{
    [field: SerializeField] public LocalizationString NamePhone;

    [field: SerializeField] public Sprite PhoneFrame { get; private set; }

    [field: SerializeField] public Sprite Background { get; private set; }

    [SerializeField] private Sprite[] _hands;

    [SerializeField] private List<PhoneContact> _phoneContactDatas;

    // private HashSet<string> _phoneContactDictionary;
    private Dictionary<string, PhoneContact> _phoneContactDictionary;
    public IReadOnlyList<Sprite> Hands => _hands;

    // public IReadOnlyList<PhoneContact> PhoneContactDatas => _phoneContactDatas;

    // public IReadOnlyList<PhoneContact> PhoneContactDatas => _phoneContactDatas;
    // public IReadOnlyCollection<string> PhoneContactDictionary => _phoneContactDictionary;
    public IReadOnlyDictionary<string, PhoneContact> PhoneContactDictionary => _phoneContactDictionary;

    public Phone(LocalizationString namePhone, IReadOnlyList<Sprite> hands, Sprite phoneFrame, Sprite background)
    {
        NamePhone = namePhone;
        _hands = hands.ToArray();
        PhoneFrame = phoneFrame;
        Background = background;
        _phoneContactDatas = new List<PhoneContact>();
        _phoneContactDictionary = new Dictionary<string, PhoneContact>();
    }

    public void AddContact(PhoneContact contact)
    {
        Debug.Log($"AddContact1   {contact.NameLocalizationString.DefaultText}");
        if (_phoneContactDictionary.ContainsKey(contact.NameLocalizationString.Key) == false)
        {
            Debug.Log($"AddContact2   {contact.NameLocalizationString.DefaultText}");
            _phoneContactDatas.Add(contact);
            _phoneContactDictionary.Add(contact.NameLocalizationString.Key, contact);
        }
    }
}