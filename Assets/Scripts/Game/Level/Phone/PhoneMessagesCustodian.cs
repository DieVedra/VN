using System;
using System.Collections.Generic;
using UniRx;


// public void Cleanup() { /* очистка */ }
// public void Release() { /* освобождение */ }
// public void Destroy() { /* уничтожение */ }
// public void Shutdown() { /* завершение работы */ }
// public void Close() { /* закрытие */ }
// public void FreeResources() { /* освобождение ресурсов */ }
public class PhoneMessagesCustodian
{
    private const int _defaultCapasity = 10;
    private readonly Dictionary<string, Dictionary<string, List<PhoneMessage>>> _phonesMessageHistory;
    private readonly ReactiveCommand _applyAddMessages;
    private Dictionary<string, Dictionary<string, List<PhoneMessage>>> _bufferPhonesMessageHistory;

    public PhoneMessagesCustodian(ReactiveCommand applyAddMessages)
    {
        _phonesMessageHistory = new Dictionary<string, Dictionary<string, List<PhoneMessage>>>();
        _bufferPhonesMessageHistory = new Dictionary<string, Dictionary<string, List<PhoneMessage>>>();
        _applyAddMessages = applyAddMessages;
        _applyAddMessages.Subscribe(_=>
        {
            ApplyAddMessages();
        });
    }

    public void Init()
    {
        
    }

    public void Shutdown()
    {
        
    }

    public void AddPhoneHistory(Phone phone)
    {
        if (_phonesMessageHistory.ContainsKey(phone.NamePhone.Key))
        {
            _phonesMessageHistory.Add(phone.NamePhone.Key, new Dictionary<string, List<PhoneMessage>>());
        }
    }

    public void AddMessageHistoryToBuffer(string phoneKey, string contactKey, PhoneMessage phoneMessage)
    {
        if (_bufferPhonesMessageHistory == null)
        {
            _bufferPhonesMessageHistory = new Dictionary<string, Dictionary<string, List<PhoneMessage>>>();
        }
        if (_bufferPhonesMessageHistory.ContainsKey(phoneKey))
        {
            if (_bufferPhonesMessageHistory[phoneKey].ContainsKey(contactKey))
            {
                _bufferPhonesMessageHistory[phoneKey][contactKey].Add(phoneMessage);
            }
            else
            {
                Add(_bufferPhonesMessageHistory[phoneKey], phoneMessage, contactKey);
            }
        }
        else
        {
            var newDictionary = new Dictionary<string, List<PhoneMessage>>();
            _bufferPhonesMessageHistory.Add(phoneKey, newDictionary);
            Add(newDictionary, phoneMessage, contactKey);

        }
    }

    private void ApplyAddMessages()
    {
        foreach (var pairBuffer in _bufferPhonesMessageHistory)
        {
            if (_phonesMessageHistory.TryGetValue(pairBuffer.Key, out Dictionary<string, List<PhoneMessage>> dictHistory))
            {
                foreach (var keyValuePair in pairBuffer.Value)
                {
                    if (dictHistory.ContainsKey(keyValuePair.Key))
                    {
                        var list = dictHistory[keyValuePair.Key];
                        if (list == null)
                        {
                            list = dictHistory[keyValuePair.Key] = new List<PhoneMessage>(_defaultCapasity);
                        }
                        list.AddRange(keyValuePair.Value);
                    }
                }
            }
            else
            {
                _phonesMessageHistory.Add(pairBuffer.Key, pairBuffer.Value);
            }
        }

        _bufferPhonesMessageHistory = null;
    }

    public (IReadOnlyList<PhoneMessage>, IReadOnlyList<PhoneMessage>) GetMessagesHistory(string phoneKey, string contactKey)
    {
        IReadOnlyList<PhoneMessage> history = null;
        IReadOnlyList<PhoneMessage> buffer = null;
        if (_phonesMessageHistory.ContainsKey(phoneKey))
        {
            if (_phonesMessageHistory[phoneKey].ContainsKey(contactKey))
            {
                history = _phonesMessageHistory[phoneKey][contactKey];
            }
        }

        if (_bufferPhonesMessageHistory.ContainsKey(phoneKey))
        {
            if (_bufferPhonesMessageHistory[phoneKey].ContainsKey(contactKey))
            {
                buffer = _bufferPhonesMessageHistory[phoneKey][contactKey];
            }
        }

        return (history, buffer);
    }

    private void Add(Dictionary<string, List<PhoneMessage>> targetDictionary, PhoneMessage phoneMessage, string key)
    {
        var newList = new List<PhoneMessage>(_defaultCapasity);
        targetDictionary.Add(key, newList);
        newList.Add(phoneMessage);
    }
}