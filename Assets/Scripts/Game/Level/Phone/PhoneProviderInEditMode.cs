using System;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class PhoneProviderInEditMode : MonoBehaviour, IPhoneProvider, ILocalizable
{
    [SerializeField, Expandable] private List<PhoneDataProvider> _dataProviders;
    [SerializeField, Expandable] private List<PhoneContactsProvider> _contactsToSeriaProviders;
    
    [SerializeField] private ContactView _contactPrefab;
    [SerializeField] private MessageView _incomingMessagePrefab;
    [SerializeField] private MessageView _outcomingMessagePrefab;
    [SerializeField] private NotificationView _notificationViewPrefab;
    [SerializeField] private List<ObjectsToDestroy> _views;
    
    private Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    private List<Phone> _phones;
    private Dictionary<string, PhoneContactDataLocalizable> _contactsAddToPhone;
    private PhoneCreatorEditMode _phoneCreator;
    private IReadOnlyList<PhoneAddedContact> _saveContacts;
    private PhoneContactCombiner _phoneContactCombiner;
    private PhoneSaveHandler _phoneSaveHandler;

    private PhoneContentProvider _phoneContentProvider;

    public PhoneContentProvider PhoneContentProvider => _phoneContentProvider;
    
    public IReadOnlyList<PhoneDataProvider> DataProviders => _dataProviders;
    public IReadOnlyList<PhoneContactsProvider> ContactsToSeriaProviders => _contactsToSeriaProviders;

    public void Construct(IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians)
    {
        _phoneContactCombiner = new PhoneContactCombiner();
        _phoneCreator = new PhoneCreatorEditMode(_dataProviders, _contactsToSeriaProviders, customizableCharacterIndexesCustodians,
            _phoneContactCombiner);
        _phones = new List<Phone>();
        _contactsAddToPhone = new Dictionary<string, PhoneContactDataLocalizable>();
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
            strings.AddRange(_phones[i].PhoneDataLocalizable.GetLocalizableContent());
        }
        return strings;
    }

    public void TrySetSaveData(IReadOnlyList<PhoneAddedContact> contacts)
    {
        _saveContacts = contacts;
    }
    public IReadOnlyList<PhoneAddedContact> GetSaveData()
    {
        return _phoneSaveHandler.GetSaveData(_phones);
    }
    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex)
    {
        if (Application.isPlaying)
        {
            if (_phones.Count == 0)
            {
                _phoneCreator.CreatePhonesOnStart(_phones, currentSeriaIndex);
                if (_saveContacts != null)
                {
                    _phoneSaveHandler.AddContactsToPhoneFromSaveData(_phones, _contactsToSeriaProviders, _saveContacts, currentSeriaIndex);
                }
            }
            else
            {
                _phoneCreator.TryAddDataToIntegratedContactsAndTryCreateNewPhones(_phones, currentSeriaIndex);
            }
            return _phones;
        }
        else
        {
            return _phoneCreator.TryCreatePhonesForNonPlayMode(currentSeriaIndex);
        }
    }

    //а если контакт не был добавлен в телефон ранее а был добавлен в текущей серии то его надо добавить без контента прошлых серий
    //идея добавлять контент только если контакт был добавлен
    //если контакт может быть добавлен в текущей серии то дать ему дату только текущей серии

    //к каждой серии контакт должен идти без контента прошлых серий что бы при добавлении был без истории переписки
    public IReadOnlyList<PhoneContactDataLocalizable> GetContactsAddToPhone(int seriaIndex)
    {
        for (int i = 0; i < _contactsToSeriaProviders.Count; i++)
        {
            _phoneContactCombiner.TryCreateAddebleContactsDataLocalizable(_contactsAddToPhone, _contactsToSeriaProviders[i].PhoneContactDatas);
        }
        return _contactsAddToPhone.Select(x=>x.Value).ToList();
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