using System;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PhoneProviderInEditMode : MonoBehaviour, IPhoneProvider
{
    [SerializeField, Expandable] private List<PhoneDataProvider> _dataProviders;
    [SerializeField, Expandable] private List<PhoneContactsProvider> _contactsToSeriaProviders;
    
    [SerializeField] private ContactView _contactPrefab;
    [SerializeField] private MessageView _incomingMessagePrefab;
    [SerializeField] private MessageView _outcomingMessagePrefab;
    [SerializeField] private NotificationView _notificationViewPrefab;
    [SerializeField] private List<ObjectsToDestroy> _views;
    private Dictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    private List<Phone> _phones2;

    private PhoneCreatorEditMode _phoneCreator;

    private PhoneContactCombiner _phoneContactCombiner;
    private PhoneContentProvider _phoneContentProvider;

    public PhoneContentProvider PhoneContentProvider => _phoneContentProvider;
    //в лайтайме в эдитор режиме телефоны поставляются  в каждую серию отдельные со своим контентом
    //в рантайме в эдитор режиме телефоны поставляются только в активную серию и их дата дополняется по мере перехода в след серии
    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex)
    {
        if (Application.isPlaying)
        {
            _phoneCreator.TryCreatePhones(_phones2, currentSeriaIndex);
            return _phones2;
        }
        else
        {
            
            return _phoneCreator.TryCreatePhonesForNonPlayMode(currentSeriaIndex);
        }
    }
    
    //придумать проверку если контакт был добавлен в телефон ранее то контакт будет дополнен датой текущей серии
    public void Construct(IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians)
    {
        // _customizableCharacterIndexesCustodians = customizableCharacterIndexesCustodians;
        _phoneContactCombiner = new PhoneContactCombiner();
        _phoneCreator = new PhoneCreatorEditMode(_dataProviders, customizableCharacterIndexesCustodians, _phoneContactCombiner);
        _phones2 = new List<Phone>();
        for (int i = 0; i < _views.Count; i++)
        {
            _views[i].IsNewkey = false;
        }
        _phoneContentProvider = new PhoneContentProvider(_contactPrefab, _incomingMessagePrefab, _outcomingMessagePrefab, _notificationViewPrefab, AddView);
        TryDestroyOld();
    }
    //а если контакт не был добавлен в телефон ранее а был добавлен в текущей серии то его надо добавить без контента прошлых серий

    public IReadOnlyList<PhoneContactDataLocalizable> GetContactsToSeria(int seriaIndex)
    {
        List<PhoneContactDataLocalizable> contactDatas = new List<PhoneContactDataLocalizable>();
        for (int i = 0; i < _contactsToSeriaProviders.Count; i++)
        {
            if (_contactsToSeriaProviders[i].SeriaIndex < seriaIndex)
            {
                contactDatas.AddRange(_phoneContactCombiner.CreateNewPhoneContactData(_contactsToSeriaProviders[i].PhoneContactDatas, true));
                    
            }else if (_contactsToSeriaProviders[i].SeriaIndex == seriaIndex)
            {
                contactDatas.AddRange(_phoneContactCombiner.CreateNewPhoneContactData(_contactsToSeriaProviders[i].PhoneContactDatas, false));
            }
        }
        return contactDatas;
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

//глобальное хранилище контактов для каждой серии и у каждого персонажа с телефоном есть свои контакты
//
//при сохранении игры сохраняются ключи или индексы добавленных в телефон контактов
//у каждого добавленного в телефон контакта текущей серии сохраняются ключи "прочитано" прочитанные сохраняются 
//
//из глобального хранилища можно выбрать контакт который можно добавить в конкретный телефон по ходу сюжета

//телефон создается если в серии есть ноды телефона или есть контент для телефона 
//
//телефон включается нодой в которой можно выбрать экран
//при добавлении контакта срабатывает нода оповещения