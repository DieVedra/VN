using System.Collections.Generic;

public class PhoneSaveHandler
{
    private readonly PhoneMessagesCustodian _phoneMessagesCustodian;
    private List<PhoneSaveData> _phoneSaveData;
    private bool _phoneNodeIsLastNodeOnSave;
    private bool _phoneNodeIsActive;
    private bool _loadFromSave;
    public bool PhoneNodeIsActiveOnSave => _phoneNodeIsLastNodeOnSave;
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

    public string DialogContactKey { get; private set; }
    public int GetPhoneScreenIndex { get; private set; }
    public int PhoneContentNodeIndex { get; private set; }
    
    public IReadOnlyList<string> UnreadebleContacts { get; private set; }
    public IReadOnlyList<int> ReadedContactNodeCaseIndexes { get; private set; }

    public PhoneSaveHandler(PhoneMessagesCustodian phoneMessagesCustodian)
    {
        _phoneMessagesCustodian = phoneMessagesCustodian;
    }

    public void SetPhoneNodeActiveKey(bool key)
    {
        _phoneNodeIsActive = key;
    }

    public void SetPhoneInfoFromSaveData(StoryData storyData)
    {
        _phoneNodeIsLastNodeOnSave = storyData.PhoneNodeIsActiveOnSave;
        if (_phoneNodeIsLastNodeOnSave)
        {
            _phoneSaveData = storyData.PhoneSaveDatas;
            GetPhoneScreenIndex = storyData.PhoneScreenIndex;
            DialogContactKey = storyData.DialogContactKey;
            UnreadebleContacts = storyData.UnreadebleContacts;
            ReadedContactNodeCaseIndexes = storyData.ReadedContactNodeCaseIndexes;
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