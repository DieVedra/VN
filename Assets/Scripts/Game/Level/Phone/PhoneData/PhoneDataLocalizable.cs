using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhoneDataLocalizable
{
    private List<PhoneContactDataLocalizable> _phoneContactDatasLocalizable;
    public Sprite Hand { get; private set; }
    public Sprite PhoneFrame { get; private set; }
    public Sprite Background { get; private set; }
    public IReadOnlyList<PhoneContactDataLocalizable> PhoneContactDatasLocalizable => _phoneContactDatasLocalizable;

    public PhoneDataLocalizable(List<PhoneContactDataLocalizable> phoneContactDatasLocalizable)
    {
        _phoneContactDatasLocalizable = phoneContactDatasLocalizable;
    }
    public void AddPhoneContactAndContactData(IReadOnlyList<PhoneContactDataLocalizable> contactDataLocalizables)
    {
        var dictionary = contactDataLocalizables.ToDictionary(x => x.NameContactLocalizationString.Key);
        PhoneContactDataLocalizable contact;
        for (int i = 0; i < _phoneContactDatasLocalizable.Count; i++)
        {
            contact = _phoneContactDatasLocalizable[i];
            if (dictionary.TryGetValue(contact.NameContactLocalizationString.Key, out PhoneContactDataLocalizable value))
            {
                contact.AddMessages(value.PhoneMessagesLocalization);
                dictionary.Remove(contact.NameContactLocalizationString.Key);
            }
        }
        if (dictionary.Count > 0)
        {
            foreach (var pair in dictionary)
            {
                _phoneContactDatasLocalizable.Add(pair.Value);
            }
        }
    }
    public void SetHandSprite(Sprite hand)
    {
        if (hand != null)
        {
            Hand = hand;
        }
    }

    public void SetPhoneFrameSprite(Sprite frame)
    {
        if (frame != null)
        {
            PhoneFrame = frame;
        }
    }
    public void SetPhoneBackgroundSprite(Sprite background)
    {
        if (background != null)
        {
            Background = background;
        }
    }
}