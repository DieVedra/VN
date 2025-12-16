using System;
using System.Collections.Generic;

public class PhoneSaveHandler
{
    private readonly PhoneMessagesCustodian _phoneMessagesCustodian;
    private List<PhoneSaveData> _phoneSaveData;
    private bool _phoneNodeIsLastNodeOnSave;
    public bool PhoneNodeIsLastNodeOnSave
    {
        get
        {
            if (_phoneNodeIsLastNodeOnSave == true)
            {
                _phoneNodeIsLastNodeOnSave = false;
                return true;
            }
            return _phoneNodeIsLastNodeOnSave;
        }
        private set => _phoneNodeIsLastNodeOnSave = value;
    }

    public string DialogContactKey { get; private set; }
    public int GetPhoneScreenIndex { get; private set; }
    
    
    public IReadOnlyList<string> UnreadebleContacts { get; private set; }

    public event Action OnGetSaveData;

    public PhoneSaveHandler(PhoneMessagesCustodian phoneMessagesCustodian)
    {
        _phoneMessagesCustodian = phoneMessagesCustodian;
    }

    public void SetPhoneSaveData(StoryData storyData)
    {
        _phoneNodeIsLastNodeOnSave = storyData.PhoneNodeIsLastNodeOnSave;
        if (_phoneNodeIsLastNodeOnSave)
        {
            _phoneSaveData = storyData.PhoneSaveDatas;
            GetPhoneScreenIndex = storyData.PhoneScreenIndex;
            DialogContactKey = storyData.DialogContactKey;
            UnreadebleContacts = storyData.UnreadebleContacts;
        }

        if (storyData.PhoneSaveDatas?.Count > 0)
        {
            _phoneSaveData = storyData.PhoneSaveDatas;
            _phoneMessagesCustodian.Init(_phoneSaveData);
        }
    }

    public List<PhoneSaveData> GetSaveData(List<Phone> phones)
    {
        OnGetSaveData?.Invoke();
        return null;
    }

    public void TryFillPhonesFromSaveData(List<Phone> phones, IReadOnlyDictionary<string, PhoneContact> phoneContactsDictionary)
    {
        if (_phoneSaveData != null)
        {
            foreach (var phone in phones)
            {
                foreach (var data in _phoneSaveData)
                {
                    if (phone.NamePhone.Key == data.PhoneNameKey)
                    {
                        foreach (var key in data.ContactsKeys)
                        {
                            if (phoneContactsDictionary.ContainsKey(key))
                            {
                                phone.AddContact(phoneContactsDictionary[key]);
                            }
                        }
                    }
                }
            }
        }
    }
}