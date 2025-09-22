using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PhoneData
{
    [SerializeField] private int _seriaIndex;
    [SerializeField] private string _namePhone = "ElisPhone";
    [SerializeField] private string _keyCharacterName;
    [SerializeField] private List<PhoneContactData> _phoneContactDatas;

    public PhoneData(int seriaIndex, string namePhone, string keyCharacterName)
    {
        _seriaIndex = seriaIndex;
        _namePhone = namePhone;
        _keyCharacterName = keyCharacterName;
        _phoneContactDatas = new List<PhoneContactData>();
    }

    public string NamePhone => _namePhone;
    public int SeriaIndex => _seriaIndex;
    public string KeyCharacterName => _keyCharacterName;
    public IReadOnlyList<PhoneContactData> PhoneContactDatas => _phoneContactDatas;

    public void AddDataToPhoneContact(Dictionary<string, PhoneContactData> addebleContacts) 
    {
        PhoneContactData contact;
        for (int i = 0; i < _phoneContactDatas.Count; i++)
        {
            contact = _phoneContactDatas[i];
            if (addebleContacts.TryGetValue(contact.LocalizationString.Key, out PhoneContactData phoneContactData))
            {
                // if (phoneContactData.)
                // {
                //     
                // }
                contact.AddMessages(phoneContactData.PhoneMessages);
            }
        }
    }
}
