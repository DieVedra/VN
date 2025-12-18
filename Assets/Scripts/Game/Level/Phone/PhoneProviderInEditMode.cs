using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class PhoneProviderInEditMode : MonoBehaviour, IPhoneProvider
{
    [SerializeField, Expandable] private List<PhoneProvider> _phoneProviders;
    [SerializeField, Expandable] private List<PhoneContactsProvider> _contactsToSeriaProviders;

    [SerializeField] private ContactView _contactPrefab;
    [SerializeField] private MessageView _incomingMessagePrefab;
    [SerializeField] private MessageView _outcomingMessagePrefab;
    [SerializeField] private NotificationView _notificationViewPrefab;
    [SerializeField] private List<ObjectsToDestroy> _views;
    
    
    private Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    
    [SerializeField] private List<Phone> _phones;
    private PhoneCreator _phoneCreator;
    private PhoneContactsHandler _phoneContactsHandler;
    private PhoneSaveHandler _phoneSaveHandler;

    private PhoneContentProvider _phoneContentProvider;

    public PhoneContentProvider PhoneContentProvider => _phoneContentProvider;
    public IReadOnlyList<PhoneContactsProvider> ContactsToSeriaProviders => _contactsToSeriaProviders;
    public PhoneSaveHandler PhoneSaveHandler => _phoneSaveHandler;

    public void Construct(PhoneMessagesCustodian phoneMessagesCustodian, PhoneSaveHandler phoneSaveHandler)
    {
        var checkMathSeriaIndex = new CheckMathSeriaIndex();
        _phoneContactsHandler = new PhoneContactsHandler(_contactsToSeriaProviders, checkMathSeriaIndex);
        _phoneCreator = new PhoneCreator(_phoneProviders, phoneMessagesCustodian, checkMathSeriaIndex);
        _phoneSaveHandler = phoneSaveHandler;
        for (int i = 0; i < _views.Count; i++)
        {
            _views[i].IsNewkey = false;
        }
        _phoneContentProvider = new PhoneContentProvider(_contactPrefab, _incomingMessagePrefab, _outcomingMessagePrefab, _notificationViewPrefab, AddView);
        TryDestroyOld();
        if (Application.isPlaying)
        {
            _phones.Clear();
        }
    }
    public void FillPhoneSaveInfo(StoryData data)
    {
        data.PhoneSaveDatas = _phoneSaveHandler.GetSaveData(_phones);
        if (_phoneSaveHandler.PhoneNodeIsActiveOnSave == true)
        {
            data.PhoneNodeIsActiveOnSave = true;
            data.PhoneScreenIndex = _phoneSaveHandler.GetPhoneScreenIndex;
            data.DialogContactKey = _phoneSaveHandler.DialogContactKey;
            data.UnreadebleContacts = _phoneSaveHandler.UnreadebleContacts.ToList();
            data.ReadedContactNodeCaseIndexes = _phoneSaveHandler.ReadedContactNodeCaseIndexes.ToList();
            data.PhoneContentNodeIndex = _phoneSaveHandler.PhoneContentNodeIndex;
        }
        else
        {
            data.PhoneNodeIsActiveOnSave = false;
            data.PhoneScreenIndex = -1;
            data.PhoneContentNodeIndex = -1;
            data.DialogContactKey = null;
            data.UnreadebleContacts = null;
            data.ReadedContactNodeCaseIndexes = null;
        }
    }
    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex)
    {
        if (Application.isPlaying)
        {
            if (_phones.Count == 0)
            {
                _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfRange(currentSeriaIndex); // группировка контактов
                _phones = _phoneCreator.CreatePhonesOnStart(currentSeriaIndex);
                _phoneContactsHandler.TryAddContacts(_phones);

                _phoneSaveHandler.TryFillPhonesFromSaveData(_phones, _phoneContactsHandler.PhoneContactsDictionary);
            }
            else
            {
                _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfMath(currentSeriaIndex);
                _phoneCreator.TryAddPhone(_phones, currentSeriaIndex);
                _phoneContactsHandler.TryAddContacts(_phones);
            }
            return _phones;
        }
        else
        {
            _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfRange(currentSeriaIndex); // группировка контактов
            var phones = _phoneCreator.CreatePhonesOnStart(currentSeriaIndex, false); // создание телефонов
            _phoneContactsHandler.TryAddContacts(phones);
            return phones;
        }
    }

    public IReadOnlyList<PhoneContact> GetContactsToAddInPhoneInPlot(int seriaIndex)
    {
        if (Application.isPlaying)
        {
            _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfMath(seriaIndex);
        }
        else
        {
            _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfRange(seriaIndex);
        }
        return _phoneContactsHandler.GetContactsAddebleToPhoneBySeriaIndexInPlot(seriaIndex);
    }

    private void TryDestroyOld()
    {
        bool recursion = false;
        for (int i = 0; i < _views.Count; i++)
        {
            if (_views[i].IsNewkey == false)
            {
                recursion = true;
                DestroyImmediate(_views[i].Go);
                _views.RemoveAt(i);
            }
        }

        if (recursion == true)
        {
            recursion = false;
            TryDestroyOld();
        }
    }

    private void AddView(GameObject view)
    {
        _views.Add(new ObjectsToDestroy(){IsNewkey = true, Go = view});
    }

    [Serializable]
    protected class ObjectsToDestroy
    {
        [field: SerializeField] public bool IsNewkey;
        [field: SerializeField] public GameObject Go;
    }
}