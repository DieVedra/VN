using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class PhoneProviderInEditMode : MonoBehaviour, IPhoneProvider, ILocalizable
{
    [SerializeField, Expandable] private List<PhoneProvider> _phoneProviders;
    [SerializeField, Expandable] private List<PhoneContactsProvider> _contactsToSeriaProviders;

    [SerializeField] private ContactView _contactPrefab;
    [SerializeField] private MessageView _incomingMessagePrefab;
    [SerializeField] private MessageView _outcomingMessagePrefab;
    [SerializeField] private NotificationView _notificationViewPrefab;
    [SerializeField] private List<ObjectsToDestroy> _views;
    
    private Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    
    private List<Phone> _phones;
    private PhoneCreator _phoneCreator;
    private PhoneContactsHandler _phoneContactsHandler;


    private IReadOnlyList<PhoneAddedContact> _saveContacts;
    private PhoneSaveHandler _phoneSaveHandler;

    private PhoneContentProvider _phoneContentProvider;

    public PhoneContentProvider PhoneContentProvider => _phoneContentProvider;
    
    // public IReadOnlyList<PhoneDataProvider> DataProviders => null/*_dataProviders*/;
    public IReadOnlyList<PhoneContactsProvider> ContactsToSeriaProviders => _contactsToSeriaProviders;

    public void Construct(IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians)
    {
        var checkMathSeriaIndex = new CheckMathSeriaIndex();
        _phoneContactsHandler = new PhoneContactsHandler(_contactsToSeriaProviders, checkMathSeriaIndex);
        _phoneCreator = new PhoneCreator(_phoneProviders, _contactsToSeriaProviders, customizableCharacterIndexesCustodians, checkMathSeriaIndex);
        _phoneSaveHandler = new PhoneSaveHandler();
        for (int i = 0; i < _views.Count; i++)
        {
            _views[i].IsNewkey = false;
        }
        _phoneContentProvider = new PhoneContentProvider(_contactPrefab, _incomingMessagePrefab, _outcomingMessagePrefab, _notificationViewPrefab, AddView);
        TryDestroyOld();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        for (int i = 0; i < _phones.Count; i++)
        {
            // strings.AddRange(_phones[i].PhoneDataLocalizable.GetLocalizableContent());
        }
        return strings;
    }

    public void TrySetSaveData(IReadOnlyList<PhoneAddedContact> contacts)
    {
        _saveContacts = contacts;
    }
    // public IReadOnlyList<PhoneAddedContact> GetSaveData()
    // {
    //     return _phoneSaveHandler.GetSaveData(_phones);
    // }
    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex)
    {
        if (Application.isPlaying)
        {
            if (_phones == null)
            {
                _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfRange(currentSeriaIndex); // группировка контактов
                _phones = _phoneCreator.CreatePhonesOnStart(currentSeriaIndex);
                _phoneContactsHandler.TryAddContacts(_phones, currentSeriaIndex);

                
                // _phoneContactsHandler.FillPhonesContacts(_phones);
                
                // if (_saveContacts != null)
                // {
                //     _phoneSaveHandler.AddContactsToPhoneFromSaveData(_phones, _contactsToSeriaProviders, _saveContacts, currentSeriaIndex);
                // }
            }
            else
            {
                _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfMath(currentSeriaIndex);
                _phoneCreator.TryAddPhone(_phones, currentSeriaIndex);
                _phoneContactsHandler.TryAddContacts(_phones, currentSeriaIndex);
            }
            return _phones;
        }
        else
        {
            _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfRange(currentSeriaIndex); // группировка контактов
            var phones = _phoneCreator.CreatePhonesOnStart(currentSeriaIndex, false); // создание телефонов
            
            _phoneContactsHandler.TryAddContacts(_phones, currentSeriaIndex);

            // _phoneContactsHandler.FillPhonesContacts(_phones); //заполнение телефонов контактами которые должны быть не добавляемы из сюжета
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