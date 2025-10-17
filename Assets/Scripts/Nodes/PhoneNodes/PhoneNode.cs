﻿using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#07B715")]
public class PhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private int _phoneIndex;
    [SerializeField] private PhoneBackgroundScreen _phoneStartScreen;
    [SerializeField] private int _butteryPercent;
    [SerializeField] private int _startHour;
    [SerializeField] private int _startMinute;
    [SerializeField] private LocalizationString _date;
    [SerializeField] private int _startScreenCharacterIndex;
    [SerializeField] private List<ContactInfoToGame> _contactsInfoToGame;
    private PhoneUIHandler _phoneUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    public IReadOnlyList<Phone> Phones { get; private set; }
    public IReadOnlyList<PhoneContactDataLocalizable> Contacts { get; private set; }
    public IReadOnlyList<PhoneContactDataLocalizable> PhoneContactDatasLocalizable =>
        Phones[_phoneIndex].PhoneDataLocalizable.PhoneContactDatasLocalizable;
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContactDataLocalizable> contacts,
        PhoneUIHandler phoneUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler)
    {
        Phones = phones;
        _phoneUIHandler = phoneUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        Contacts = contacts;
        if (IsPlayMode() == false)
        {
            CreateContactsToOnlineAndNotifications(contacts);
        }
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        SetInfoToView();
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        await _customizationCurtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
    }

    public override async UniTask Exit()
    {
        await _customizationCurtainUIHandler.CurtainCloses(CancellationTokenSource.Token);
        CancellationTokenSource = null;
        _phoneUIHandler.DisposeScreensBackgrounds();
        ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
    }

    protected override void SetInfoToView()
    {
        _phoneUIHandler.ConstructFromNode(_contactsInfoToGame, Phones[_phoneIndex], SetLocalizationChangeEvent, SwitchToNextNodeEvent, IsPlayMode(),
            _butteryPercent,_startHour, _startMinute);
        switch (_phoneStartScreen)
        {
            case PhoneBackgroundScreen.BlockScreen:
                _phoneUIHandler.SetBlockScreenBackgroundFromNode(_date, _startScreenCharacterIndex, IsPlayMode());
                break;
            case PhoneBackgroundScreen.ContactsScreen:
                _phoneUIHandler.SetContactsScreenBackgroundFromNode();
                break;
            case PhoneBackgroundScreen.DialogScreen:
                _phoneUIHandler.SetDialogScreenBackgroundFromNode(_startScreenCharacterIndex);
                break;
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_date};
    }
    private void CreateContactsToOnlineAndNotifications(IReadOnlyList<PhoneContactDataLocalizable> contacts)
    {
        Dictionary<string, ContactInfoToGame> contactsDictionary = new Dictionary<string, ContactInfoToGame>();
        for (int i = 0; i < contacts.Count; i++)
        {
            TryAdd(contacts[i]);
        }
        int count;
        PhoneDataLocalizable dataLocalizable;
        for (int i = 0; i < Phones.Count; i++)
        {
            count = Phones[i].PhoneDataLocalizable.PhoneContactDatasLocalizable.Count;
            dataLocalizable = Phones[i].PhoneDataLocalizable;
            for (int j = 0; j < count; j++)
            {
                TryAdd(dataLocalizable.PhoneContactDatasLocalizable[j]);
            }
        }
    
        if (_contactsInfoToGame != null)
        {
            TransferringKeys(_contactsInfoToGame);
        }
    
        void TryAdd(PhoneContactDataLocalizable phoneContactDataLocalizable, bool statusKey = false, bool notificationKey = false)
        {
            if (contactsDictionary.ContainsKey(phoneContactDataLocalizable.NameContact.Key) == false)
            {
                contactsDictionary.Add(
                    phoneContactDataLocalizable.NameContact.Key,
                    new ContactInfoToGame(
                        phoneContactDataLocalizable.NameContact.DefaultText,
                        phoneContactDataLocalizable.NameContact.Key,
                        statusKey, notificationKey));
            }
        }
    
        void TransferringKeys(List<ContactInfoToGame> from)
        {
            for (int i = 0; i < from.Count; i++)
            {
                if (contactsDictionary.ContainsKey(from[i].KeyName))
                {
                    var oldInfoValue = contactsDictionary[from[i].KeyName];
                    oldInfoValue.KeyNotification = from[i].KeyNotification;
                    oldInfoValue.KeyOnline = from[i].KeyOnline;
                    contactsDictionary[from[i].KeyName] = oldInfoValue;
                }
            }
            from.Clear();
            foreach (var pair in contactsDictionary)
            {
                from.Add(new ContactInfoToGame(pair.Value.Name, pair.Value.KeyName, pair.Value.KeyOnline, pair.Value.KeyNotification));
            }
            from.TrimExcess();
        }
    }
}