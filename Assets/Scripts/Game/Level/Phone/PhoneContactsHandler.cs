using System;
using System.Collections.Generic;

public class PhoneContactsHandler
{
    private readonly IReadOnlyList<PhoneContactsProvider> _contactsToSeriaProviders;
    private readonly CheckMathSeriaIndex _checkMathSeriaIndex;
    private readonly Dictionary<string, PhoneContact> _dictionary;
    public IReadOnlyDictionary<string, PhoneContact> PhoneContactsDictionary => _dictionary;
    public PhoneContactsHandler(IReadOnlyList<PhoneContactsProvider> contactsToSeriaProviders, CheckMathSeriaIndex checkMathSeriaIndex)
    {
        _contactsToSeriaProviders = contactsToSeriaProviders;
        _checkMathSeriaIndex = checkMathSeriaIndex;
        _dictionary = new Dictionary<string, PhoneContact>();
    }

    public void TryAddContacts(List<Phone> phones)
    {
        PhoneContact phoneContact;
        foreach (var phone in phones)
        {
            foreach (var pair in _dictionary)
            {
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
        foreach (var contactsProvider in _contactsToSeriaProviders)
        {
            if (checkOperation.Invoke(contactsProvider.SeriaIndex, seriaIndex))
            {
                foreach (var phoneContact in contactsProvider.PhoneContacts)
                {
                    if (_dictionary.ContainsKey(phoneContact.NameLocalizationString.Key) == false)
                    {
                        _dictionary.Add(phoneContact.NameLocalizationString.Key, phoneContact);
                    }
                }
            }
        }
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