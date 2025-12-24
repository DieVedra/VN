using System.Collections.Generic;
using UnityEngine;

// public void Cleanup() { /* очистка */ }
// public void Release() { /* освобождение */ }
// public void Destroy() { /* уничтожение */ }
// public void Shutdown() { /* завершение работы */ }
// public void Close() { /* закрытие */ }
// public void FreeResources() { /* освобождение ресурсов */ }
public class PhoneMessagesCustodian : ILocalizable
{
    private const int _defaultCapasity = 10;
    private Dictionary<string, Dictionary<string, List<PhoneMessage>>> _phonesMessageHistory;

    public PhoneMessagesCustodian()
    {
        _phonesMessageHistory = new Dictionary<string, Dictionary<string, List<PhoneMessage>>>(_defaultCapasity);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> messagesStrings = new List<LocalizationString>();
        Collect(messagesStrings, _phonesMessageHistory);
        return messagesStrings;
    }

    private void Collect(List<LocalizationString> messagesStrings, Dictionary<string, Dictionary<string, List<PhoneMessage>>> messageHistory)
    {
        foreach (var pair in messageHistory)
        {
            foreach (var messages in pair.Value)
            {
                foreach (var item in messages.Value)
                {
                    messagesStrings.Add(item.TextMessage);
                }
            }
        }
    }

    public void Init(IReadOnlyList<PhoneSaveData> saveDatas)
    {
        string key;
        foreach (var data in saveDatas)
        {
            key = data.PhoneNameKey;
            AddPhoneHistory(key);
            if (_phonesMessageHistory.TryGetValue(key, out var dictionary))
            {
                List<PhoneMessage> list;
                foreach (var pair in data.MessageHistory)
                {
                    list = new List<PhoneMessage>(); 
                    foreach (var saveMessage in pair.Value)
                    {
                        PhoneMessage message = new PhoneMessage
                        {
                            TextMessage = new LocalizationString(null, saveMessage.KeyMessage),
                            MessageType = (PhoneMessageType) saveMessage.MessageTypeIndex,
                            IsReaded = true
                        };
                        list.Add(message);
                    }
                    dictionary.Add(pair.Key, list);
                }
            }
            else
            {
                
            }
        }
    }

    public void AddPhoneHistory(string phoneNameKey)
    {
        if (_phonesMessageHistory.ContainsKey(phoneNameKey) == false)
        {
            _phonesMessageHistory.Add(phoneNameKey, new Dictionary<string, List<PhoneMessage>>());
        }
    }

    public void AddMessageHistory(string phoneKey, string contactKey, PhoneMessage phoneMessage)
    {
        if (_phonesMessageHistory.ContainsKey(phoneKey))
        {
            if (_phonesMessageHistory[phoneKey].ContainsKey(contactKey))
            {
                _phonesMessageHistory[phoneKey][contactKey].Add(phoneMessage);
            }
            else
            {
                Add(_phonesMessageHistory[phoneKey], phoneMessage, contactKey);
            }
        }
        else
        {
            var newDictionary = new Dictionary<string, List<PhoneMessage>>();
            _phonesMessageHistory.Add(phoneKey, newDictionary);
            Add(newDictionary, phoneMessage, contactKey);
        }
    }

    public IReadOnlyList<PhoneMessage> GetMessagesHistory(string phoneKey, string contactKey)
    {
        IReadOnlyList<PhoneMessage> history = null;
        if (_phonesMessageHistory.ContainsKey(phoneKey))
        {
            if (_phonesMessageHistory[phoneKey].ContainsKey(contactKey))
            {
                history = _phonesMessageHistory[phoneKey][contactKey];
            }
        }
        return history;
    }

    private void Add(Dictionary<string, List<PhoneMessage>> targetDictionary, PhoneMessage phoneMessage, string key)
    {
        var newList = new List<PhoneMessage>(_defaultCapasity);
        targetDictionary.Add(key, newList);
        newList.Add(phoneMessage);
    }
}