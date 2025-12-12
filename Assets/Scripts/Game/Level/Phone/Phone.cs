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
    [field: SerializeField] public Color BlockScreenTopPanelColor { get; private set; }
    [field: SerializeField] public Color DialogScreenTopPanelColor { get; private set; }
    [field: SerializeField] public Color ContactsScreenTopPanelColor { get; private set; }
    [SerializeField] private Sprite[] _hands;
    [SerializeField] private List<PhoneContact> _phoneContactDatas;
    private Dictionary<string, PhoneContact> _phoneContactDictionary;
    public IReadOnlyList<Sprite> Hands => _hands;
    public IReadOnlyDictionary<string, PhoneContact> PhoneContactDictionary => _phoneContactDictionary;

    public Phone(Color blockScreenTopPanelColor, Color dialogScreenTopPanelColor, Color contactsScreenTopPanelColor, LocalizationString namePhone, IReadOnlyList<Sprite> hands, Sprite phoneFrame, Sprite background)
    {
        BlockScreenTopPanelColor = blockScreenTopPanelColor;
        DialogScreenTopPanelColor = dialogScreenTopPanelColor;
        ContactsScreenTopPanelColor = contactsScreenTopPanelColor;
        NamePhone = namePhone;
        _hands = hands.ToArray();
        PhoneFrame = phoneFrame;
        Background = background;
        _phoneContactDatas = new List<PhoneContact>();
        _phoneContactDictionary = new Dictionary<string, PhoneContact>();
    }

    public void AddContact(PhoneContact contact)
    {
        if (_phoneContactDictionary.ContainsKey(contact.NameLocalizationString.Key) == false)
        {
            _phoneContactDatas.Add(contact);
            _phoneContactDictionary.Add(contact.NameLocalizationString.Key, contact);
        }
    }
}