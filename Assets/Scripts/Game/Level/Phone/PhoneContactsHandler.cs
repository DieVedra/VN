using System;
using System.Collections.Generic;
using UnityEngine;

public class PhoneContactsHandler
{
    private readonly IReadOnlyList<PhoneContactsProvider> _contactsToSeriaProviders;
    private readonly CheckMathSeriaIndex _checkMathSeriaIndex;
    private readonly Dictionary<string, PhoneContact> _dictionary;
    public PhoneContactsHandler(IReadOnlyList<PhoneContactsProvider> contactsToSeriaProviders, CheckMathSeriaIndex checkMathSeriaIndex)
    {
        _contactsToSeriaProviders = contactsToSeriaProviders;
        _checkMathSeriaIndex = checkMathSeriaIndex;
        _dictionary = new Dictionary<string, PhoneContact>();
    }

    public void TryAddContacts(List<Phone> phones)
    {
        Phone phone;
        PhoneContact phoneContact;
        for (int i = 0; i < phones.Count; i++)
        {
            foreach (var pair in _dictionary)
            {
                phone = phones[i];
                phoneContact = pair.Value;
                if (phoneContact.ToPhoneKey == phone.NamePhone.Key && phoneContact.AddInPlot == false)
                {
                    phone.AddContact(phoneContact);
                }
                
            }
        }
    }

    public void TryCollectAllContactsBySeriaIndexOfRange(int seriaIndex)
    {
        if (_checkMathSeriaIndex.Check(seriaIndex))
        {
            CollectAllContactsBySeriaIndex(seriaIndex, CheckingOfRange);
        }

    }
    public void TryCollectAllContactsBySeriaIndexOfMath(int seriaIndex)
    {
        if (_checkMathSeriaIndex.Check(seriaIndex))
        {
            CollectAllContactsBySeriaIndex(seriaIndex, CheckingOfMath);
        }
    }
    public void CollectAllContactsBySeriaIndex(int seriaIndex, Func<int,int, bool> checkOperation)
    {
        _dictionary.Clear();
        for (int i = 0; i < _contactsToSeriaProviders.Count; i++)
        {
            if (checkOperation.Invoke(_contactsToSeriaProviders[i].SeriaIndex, seriaIndex))
            {
                PhoneContact phoneContact;
                for (int j = 0; j < _contactsToSeriaProviders[i].PhoneContacts.Count; j++)
                {
                    phoneContact = _contactsToSeriaProviders[i].PhoneContacts[j];
                    if (_dictionary.ContainsKey(phoneContact.NameLocalizationString.Key) == false)
                    {
                        _dictionary.Add(phoneContact.NameLocalizationString.Key, phoneContact);
                    }
                }
            }
        }
    }
    
    public List<PhoneContact> GetAllContactsToPhoneNode()
    {
        List<PhoneContact> contacts = new List<PhoneContact>();
        foreach (var pair in _dictionary)
        {
            contacts.Add(pair.Value);
        }
        
        return contacts;
    }
    public List<PhoneContact> GetContactsAddebleToPhoneBySeriaIndexInPlot(int seriaIndex)
    {
        List<PhoneContact> contacts = new List<PhoneContact>();
        foreach (var pair in _dictionary)
        {
            if (pair.Value.AddInPlot == true)
            {
                contacts.Add(pair.Value);
            }
        }
        
        return contacts;
    }

    private bool CheckingOfRange(int a, int b)
    {
        if (a <= b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    
    private bool CheckingOfMath(int a, int b)
    {
        if (a == b)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
}