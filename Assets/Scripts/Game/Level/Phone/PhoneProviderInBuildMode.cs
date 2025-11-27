using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PhoneProviderInBuildMode : IPhoneProvider, ILocalizable
{
    private readonly Func<UniTask> _createPhoneView;
    private const string _nameDataProviderAsset = "PhoneDataProviderSeria";
    private const string _nameContactsToSeriaProviderAsset = "PhoneContactsToSeria";

    private NotificationViewPrefabAssetProvider _notificationViewPrefabAssetProvider;
    private OutcomingMessagePrefabAssetProvider _outcomingMessagePrefabAssetProvider;
    private IncomingMessagePrefabAssetProvider _incomingMessagePrefabAssetProvider;
    private ContactPrefabAssetProvider _contactPrefabAssetProvider;
    private readonly DataProvider<PhoneProvider> _dataProviders;
    private readonly DataProvider<PhoneContactsProvider> _contactsToSeriaProviders;
    
    private List<Phone> _phones;
    private Dictionary<string, PhoneContact> _contactsAddToPhone;
    private PhoneContentProvider _phoneContentProvider;
    // private PhoneContactCombiner _phoneContactCombiner;
    // private PhoneCreatorBuildMode _phoneCreator;
    private PhoneSaveHandler _phoneSaveHandler;
    private IReadOnlyList<PhoneAddedContact> _saveDataContacts;
    public IParticipiteInLoad PhoneDataProviderParticipiteInLoad => _dataProviders;
    public IParticipiteInLoad PhoneContactsProviderParticipiteInLoad => _contactsToSeriaProviders;
    public PhoneContentProvider PhoneContentProvider => _phoneContentProvider;
    private bool _phoneSystemInitilized;

    public PhoneProviderInBuildMode(Func<UniTask> createPhoneView)
    {
        _createPhoneView = createPhoneView;
        // _dataProviders = new DataProvider<PhoneDataProvider>();
        _contactsToSeriaProviders = new DataProvider<PhoneContactsProvider>();
        // _phoneContactCombiner = new PhoneContactCombiner();
        _phones = new List<Phone>();
        // _contactsAddToPhone = new Dictionary<string, PhoneContactDataLocalizable>();
        _notificationViewPrefabAssetProvider = new NotificationViewPrefabAssetProvider();
        _outcomingMessagePrefabAssetProvider = new OutcomingMessagePrefabAssetProvider();
        _incomingMessagePrefabAssetProvider = new IncomingMessagePrefabAssetProvider();
        _contactPrefabAssetProvider = new ContactPrefabAssetProvider();
        _phoneSaveHandler = new PhoneSaveHandler();
        _phoneSystemInitilized = false;
    }

    public async UniTask Init(IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians)
    {
        // _phoneCreator = new PhoneCreatorBuildMode(_dataProviders.GetDatas, _contactsToSeriaProviders.GetDatas,
        //     customizableCharacterIndexesCustodians, _phoneContactCombiner);
        var notificationViewPrefab = await _notificationViewPrefabAssetProvider.LoadNotificationPrefab();
        var outcomingMessagePrefab = await _outcomingMessagePrefabAssetProvider.LoadOutcomingMessagePrefab();
        var incomingMessagePrefab = await _incomingMessagePrefabAssetProvider.LoadIncomingMessagePrefab();
        var contactPrefab = await _contactPrefabAssetProvider.LoadContactPrefab();
        _phoneContentProvider = new PhoneContentProvider(contactPrefab, incomingMessagePrefab, outcomingMessagePrefab, notificationViewPrefab);
        await UniTask.WhenAll(_dataProviders.CreateNames(_nameDataProviderAsset),
            _contactsToSeriaProviders.CreateNames(_nameContactsToSeriaProviderAsset));
    }

    public void TrySetSaveData(IReadOnlyList<PhoneAddedContact> saveDataContacts)
    {
        _saveDataContacts = saveDataContacts;
    }
    // public IReadOnlyList<PhoneAddedContact> GetSaveData()
    // {
    //     return _phoneSaveHandler.GetSaveData(_phones);
    // }
    public void Dispose()
    {
        _dataProviders.Dispose();
        _contactsToSeriaProviders.Dispose();
        _notificationViewPrefabAssetProvider.UnloadAsset();
        _outcomingMessagePrefabAssetProvider.UnloadAsset();
        _incomingMessagePrefabAssetProvider.UnloadAsset();
        _contactPrefabAssetProvider.UnloadAsset();
    }
    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex)
    {
        if (_phones.Count == 0)
        {
            Debug.Log($"currentSeriaIndex999  _phones {_phones.Count}");

            // _phoneCreator.CreatePhonesOnStart(_phones, currentSeriaIndex);
            Debug.Log($"currentSeriaIndex0  _phones {_phones.Count}");

            // if (_saveDataContacts != null)
            // {
            //     _phoneSaveHandler.AddContactsToPhoneFromSaveData(_phones, _contactsToSeriaProviders.GetDatas, _saveDataContacts, currentSeriaIndex);
            // }
            Debug.Log($"currentSeriaIndex1 {currentSeriaIndex}  _phones {_phones.Count}");
        }
        else
        {
            // _phoneCreator.TryAddDataToIntegratedContactsAndTryCreateNewPhones(_phones, currentSeriaIndex);
            Debug.Log($"currentSeriaIndex2 {currentSeriaIndex}  _phones {_phones.Count}");

        }
        return _phones;
    }

    public IReadOnlyList<PhoneContact> GetContactsToAddInPhoneInPlot(int seriaIndex)
    {
        // for (int i = 0; i < _contactsToSeriaProviders.GetDatas.Count; i++)
        // {
        //     _phoneContactCombiner.TryCreateAddebleContactsDataLocalizable(_contactsAddToPhone, _contactsToSeriaProviders.GetDatas[i].PhoneContacts);
        // }
        return _contactsAddToPhone.Select(x=>x.Value).ToList();
    }
    public void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        _dataProviders.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        _contactsToSeriaProviders.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }
    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        
        if (await _dataProviders.TryLoadData(nextSeriaNameAssetIndex))
        {
            if (_phoneSystemInitilized == false)
            {
                await _createPhoneView.Invoke();
                _phoneSystemInitilized = true;
            }
        }

        await _contactsToSeriaProviders.TryLoadData(nextSeriaNameAssetIndex);
        // if (await _contactsToSeriaProviders.TryLoadData(nextSeriaNameAssetIndex))
        // {
        //     
        // }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> list = new List<LocalizationString>();
        for (int i = 0; i < _phones.Count; i++)
        {
            // list.AddRange(_phones[i].PhoneDataLocalizable.GetLocalizableContent());
        }
        foreach (var pair in _contactsAddToPhone)
        {
            // list.AddRange(pair.Value.GetLocalizableContent());
        }
        return list;
    }
}