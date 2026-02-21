using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PhoneProviderInBuildMode : IPhoneProvider, ILocalizable
{
    private readonly PhoneMessagesCustodian _phoneMessagesCustodian;
    private readonly Func<UniTask> _createPhoneView;
    private const string _nameDataProviderAsset = "PhoneProviderSeria";
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
    public IParticipiteInLoad PhoneDataProviderParticipiteInLoad => _dataProviders;
    public IParticipiteInLoad PhoneContactsProviderParticipiteInLoad => _contactsToSeriaProviders;
    public PhoneContentProvider PhoneContentProvider => _phoneContentProvider;
    public PhoneSaveHandler PhoneSaveHandler => _phoneSaveHandler;

    private bool _phoneSystemInitilized;


    public PhoneProviderInBuildMode(PhoneMessagesCustodian phoneMessagesCustodian, PhoneSaveHandler phoneSaveHandler, Func<UniTask> createPhoneView)
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
        _phoneSaveHandler = phoneSaveHandler;
        _phoneSystemInitilized = false;
    }

    public async UniTask Init()
    {
        var notificationViewPrefab = await _notificationViewPrefabAssetProvider.LoadNotificationPrefab();
        var outcomingMessagePrefab = await _outcomingMessagePrefabAssetProvider.LoadOutcomingMessagePrefab();
        var incomingMessagePrefab = await _incomingMessagePrefabAssetProvider.LoadIncomingMessagePrefab();
        var contactPrefab = await _contactPrefabAssetProvider.LoadContactPrefab();
        _phoneContentProvider = new PhoneContentProvider(contactPrefab, incomingMessagePrefab, outcomingMessagePrefab, notificationViewPrefab);

        var checkMathSeriaIndex = new CheckMathSeriaIndex();
        _phoneContactsHandler = new PhoneContactsHandler(_contactsToSeriaProviders.GetDatas, checkMathSeriaIndex);
        _phoneCreator = new PhoneCreator(_dataProviders.GetDatas, _phoneMessagesCustodian, checkMathSeriaIndex);
        await UniTask.WhenAll(_dataProviders.CreateNames(_nameDataProviderAsset),
            _contactsToSeriaProviders.CreateNames(_nameContactsToSeriaProviderAsset));
    }

    public void FillPhoneSaveInfo(StoryData data)
    {
        _phoneSaveHandler.FillPhoneSaveInfo(data, _phones);
    }

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

    public IReadOnlyList<PhoneContact> GetContactsToAddInPhoneInPlot(int seriaIndex)
    {
        _phoneContactsHandler.TryCollectAllContactsBySeriaIndexOfMath(seriaIndex);
        var a = _phoneContactsHandler.GetContactsAddebleToPhoneBySeriaIndexInPlot(seriaIndex);
        return a;
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