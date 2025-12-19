using System;
using System.Collections.Generic;
using UniRx;

public class PhoneSaveHandler
{
    private readonly PhoneMessagesCustodian _phoneMessagesCustodian;
    private readonly ReactiveProperty<bool> _phoneNodeIsActive;
    private List<PhoneSaveData> _phoneSaveData;
    private bool _loadFromSave;
    public bool PhoneNodeIsActiveKey => _phoneNodeIsActive.Value;
    public bool LoadFromSaveKey {
        get
        {
            if (_loadFromSave == true)
            {
                _loadFromSave = false;
                return true;
            }
            return _loadFromSave;
        }
    }

    public bool NotificationPressed { get; private set; } //оповещение было нажато
    public string DialogContactKey { get; private set; } // с каким контактом диалог
    public int GetPhoneScreenIndex { get; private set; } 
    public int PhoneContentNodeIndex { get; private set; }  // индекс текущей ноды
    public int NotificationsInBlockScreenIndex { get; private set; } 
    
    public IReadOnlyList<int> ReadedContactNodeCaseIndexes { get; private set; }
    public IReadOnlyList<string> OnlineContactsKeys { get; private set; }
    public IReadOnlyList<string> NotificationsKeys { get; private set; }
    public event Func<OnSaveInfo> OnSave;

    public PhoneSaveHandler(PhoneMessagesCustodian phoneMessagesCustodian, ReactiveProperty<bool> phoneNodeIsActive)
    {
        _phoneMessagesCustodian = phoneMessagesCustodian;
        _phoneNodeIsActive = phoneNodeIsActive;
    }


    public void SetPhoneInfoFromSaveData(StoryData storyData)
    {
        if (storyData.PhoneNodeIsActiveOnSave)
        {
            _phoneSaveData = storyData.PhoneSaveDatas;
            GetPhoneScreenIndex = storyData.PhoneScreenIndex;
            PhoneContentNodeIndex = storyData.PhoneContentNodeIndex;
            DialogContactKey = storyData.DialogContactKey;
            ReadedContactNodeCaseIndexes = storyData.ReadedContactNodeCaseIndexes;
            OnlineContactsKeys = storyData.OnlineContactsKeys;
            NotificationsKeys = storyData.NotificationsKeys;
            NotificationPressed = storyData.NotificationPressed;
            NotificationsInBlockScreenIndex = storyData.NotificationsInBlockScreenIndex;
            _loadFromSave = true;
        }
        else
        {
            _loadFromSave = false;
        }

        if (storyData.PhoneSaveDatas?.Count > 0)
        {
            _phoneSaveData = storyData.PhoneSaveDatas;
            _phoneMessagesCustodian.Init(_phoneSaveData);
        }
    }
    public List<PhoneSaveData> GetSaveData(List<Phone> phones)
    {
        var info = OnSave?.Invoke();
        if (info != null)
        {
            DialogContactKey = info.DialogContactKey;
            GetPhoneScreenIndex = info.GetPhoneScreenIndex;
            PhoneContentNodeIndex = info.PhoneContentNodeIndex;
            ReadedContactNodeCaseIndexes = info.ReadedContactNodeCaseIndexes;
            OnlineContactsKeys = info.OnlineContactsKeys;
            NotificationsKeys = info.NotificationsKeys;
            NotificationPressed = info.NotificationPressed;
            NotificationsInBlockScreenIndex = info.NotificationsInBlockScreenIndex;
        }


        List<PhoneSaveData> list = new List<PhoneSaveData>();
        PhoneSaveData data;
        List<string> contactsKeys;
        foreach (var phone in phones)
        {
            contactsKeys = new List<string>();
            foreach (var pair in phone.PhoneContactDictionary)
            {
                contactsKeys.Add(pair.Key);
            }
            data = new PhoneSaveData()
            {
                PhoneNameKey = phone.NamePhone.Key,
                ContactsKeys = contactsKeys,
                MessageHistory = GetMessagesHistory(phone)
            };
            list.Add(data);
        }
        return list;
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

    private Dictionary<string, List<PhoneSaveMessage>> GetMessagesHistory(Phone phone)
    {
        Dictionary<string, List<PhoneSaveMessage>> dictionary = new Dictionary<string, List<PhoneSaveMessage>>();
        IReadOnlyList<PhoneMessage> history;
        foreach (var pair in phone.PhoneContactDictionary)
        {
            history = _phoneMessagesCustodian.GetMessagesHistory(phone.NamePhone.Key, pair.Key);
            if (history != null)
            {
                dictionary.Add(pair.Key, GetHistoryToSave());
            }
        }

        return dictionary;
        List<PhoneSaveMessage> GetHistoryToSave()
        {
            List<PhoneSaveMessage> list = new List<PhoneSaveMessage>();
            foreach (var message in history)
            {
                PhoneSaveMessage messageToSave = new PhoneSaveMessage()
                {
                    KeyMessage = message.TextMessage.Key,
                    MessageTypeIndex = (int)message.MessageType
                };
                list.Add(messageToSave);
            }
            return list;
        }
    }
}