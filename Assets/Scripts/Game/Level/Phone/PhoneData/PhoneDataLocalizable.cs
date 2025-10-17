using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhoneDataLocalizable : ILocalizable
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

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        for (int i = 0; i < PhoneContactDatasLocalizable.Count; i++)
        {
            strings.AddRange(PhoneContactDatasLocalizable[i].GetLocalizableContent());
        }

        return strings;
    }

    public bool AddContactData(out Dictionary<string, PhoneContactDataLocalizable> dictionary,
        IReadOnlyList<PhoneContactDataLocalizable> contactDataLocalizables)
    {
        dictionary = contactDataLocalizables.ToDictionary(x => x.NameContact.Key);
        TryAddContactData(dictionary);
        if (dictionary.Count > 0)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public void AddContactData(IReadOnlyList<PhoneContactDataLocalizable> contactDataLocalizables)
    {
        var dictionary = contactDataLocalizables.ToDictionary(x => x.NameContact.Key);
        TryAddContactData(dictionary);
    }
    public void AddPhoneContacts(Dictionary<string, PhoneContactDataLocalizable> dictionary, int indexSeriaInWhichContactWasAdded)
    {
        foreach (var pair in dictionary)
        {
            if (pair.Value.IndexSeriaInWhichContactWasAdded != indexSeriaInWhichContactWasAdded)
            {
                pair.Value.IndexSeriaInWhichContactWasAdded = indexSeriaInWhichContactWasAdded;
                _phoneContactDatasLocalizable.Add(pair.Value);
            }
        }
    }

    public void InsertPhoneContact(PhoneContactDataLocalizable contactDataLocalizable, int insertIndex)
    {
        _phoneContactDatasLocalizable.Insert(insertIndex, contactDataLocalizable);
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

    private void TryAddContactData(Dictionary<string, PhoneContactDataLocalizable> dictionary)
    {
        PhoneContactDataLocalizable contact;
        for (int i = 0; i < _phoneContactDatasLocalizable.Count; i++)
        {
            contact = _phoneContactDatasLocalizable[i];
            if (dictionary.TryGetValue(contact.NameContact.Key, out PhoneContactDataLocalizable value))
            {
                // contact.
                contact.AddMessages(value.PhoneMessagesLocalization);
                dictionary.Remove(contact.NameContact.Key);
            }
        }
    }
}