using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

public class PhoneProviderInBuildMode : IPhoneProvider, ILocalizable
{
    private readonly PhoneMessagesCustodian _phoneMessagesCustodian;
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
    private PhoneContentProvider _phoneContentProvider;
    private PhoneContactsHandler _phoneContactsHandler;
    private PhoneCreator _phoneCreator;

    private PhoneSaveHandler _phoneSaveHandler;
    private IReadOnlyList<PhoneAddedContact> _saveDataContacts;
    public IParticipiteInLoad PhoneDataProviderParticipiteInLoad => _dataProviders;
    public IParticipiteInLoad PhoneContactsProviderParticipiteInLoad => _contactsToSeriaProviders;
    public PhoneContentProvider PhoneContentProvider => _phoneContentProvider;
    private bool _phoneSystemInitilized;


    public PhoneProviderInBuildMode(PhoneMessagesCustodian phoneMessagesCustodian, Func<UniTask> createPhoneView)
    {
        _phoneMessagesCustodian = phoneMessagesCustodian;
        _createPhoneView = createPhoneView;
        _dataProviders = new DataProvider<PhoneProvider>();
        _contactsToSeriaProviders = new DataProvider<PhoneContactsProvider>();
        _phones = new List<Phone>();
        _notificationViewPrefabAssetProvider = new NotificationViewPrefabAssetProvider();
        _outcomingMessagePrefabAssetProvider = new OutcomingMessagePrefabAssetProvider();
        _incomingMessagePrefabAssetProvider = new IncomingMessagePrefabAssetProvider();
        _contactPrefabAssetProvider = new ContactPrefabAssetProvider();
        _phoneSaveHandler = new PhoneSaveHandler();
        _phoneSystemInitilized = false;
    }

    public async UniTask Init()
    {
        var notificationViewPrefab = await _notificationViewPrefabAssetProvider.LoadNotificationPrefab();
        var outcomingMessagePrefab = await _outcomingMessagePrefabAssetProvider.LoadOutcomingMessagePrefab();
        var incomingMessagePrefab = await _incomingMessagePrefabAssetProvider.LoadIncomingMessagePrefab();
        var contactPrefab = await _contactPrefabAssetProvider.LoadContactPrefab();
        
        var checkMathSeriaIndex = new CheckMathSeriaIndex();
        _phoneContentProvider = new PhoneContentProvider(contactPrefab, incomingMessagePrefab, outcomingMessagePrefab, notificationViewPrefab);
        _phoneContactsHandler = new PhoneContactsHandler(_contactsToSeriaProviders.GetDatas, checkMathSeriaIndex);
        _phoneCreator = new PhoneCreator(_dataProviders.GetDatas, _phoneMessagesCustodian, checkMathSeriaIndex);
        
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

    public void Shutdown()
    {
        _dataProviders.Shutdown();
        _contactsToSeriaProviders.Shutdown();
        _notificationViewPrefabAssetProvider.UnloadAsset();
        _outcomingMessagePrefabAssetProvider.UnloadAsset();
        _incomingMessagePrefabAssetProvider.UnloadAsset();
        _contactPrefabAssetProvider.UnloadAsset();
    }

    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex)
    {
        if (_phones.Count == 0)
        {
            _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfRange(currentSeriaIndex); // группировка контактов
            _phones = _phoneCreator.CreatePhonesOnStart(currentSeriaIndex);
            _phoneContactsHandler.TryAddContacts(_phones);

                
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
            _phoneContactsHandler.TryAddContacts(_phones);
        }
        return _phones;
    }

    public IReadOnlyList<PhoneContact> GetContactsToAddInPhoneInPlot(int seriaIndex)
    {
        _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfMath(seriaIndex);
        return _phoneContactsHandler.GetContactsAddebleToPhoneBySeriaIndexInPlot(seriaIndex);
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
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> list = new List<LocalizationString>();
        foreach (var provider in _contactsToSeriaProviders.GetDatas)
        {
            foreach (var contact in provider.PhoneContacts)
            {
                list.Add(contact.NameLocalizationString);
            }
        }
        return list;
    }
}