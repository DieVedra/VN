using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUIHandler : ILocalizable
{
    private readonly PhoneContentProvider _phoneContentProvider;
    private readonly NarrativePanelUIHandler _narrativePanelUI;
    private readonly CustomizationCurtainUIHandler _curtainUIHandler;
    private readonly ReactiveCommand _switchToContactsScreenCommand;
    private readonly ReactiveCommand<PhoneContactDataLocalizable> _switchToDialogScreenCommand;
    private LocalizationString _notificationNameLocalizationString = "Получено новое сообщение!";
    private Action _initOperation;
    private TopPanelHandler _topPanelHandler;
    private BlockScreenHandler _blockScreenHandler;
    private ContactsScreenHandler _contactsScreenHandler;
    private DialogScreenHandler _dialogScreenHandler;
    private PhoneTime _phoneTime;
    private SwitchToNextNodeEvent _switchToNextNodeEvent;
    private Phone _currentPhone;
    private GameObject _phoneUIGameObject;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private Dictionary<string, ContactInfoToGame> _contactsInfoToGame;
    public PhoneTime PhoneTime => _phoneTime;
    public PhoneUIHandler(PhoneContentProvider phoneContentProvider, NarrativePanelUIHandler narrativePanelUI,
        CustomizationCurtainUIHandler curtainUIHandler, CompositeDisposable compositeDisposable, Action initOperation)
    {
        _phoneContentProvider = phoneContentProvider;
        _narrativePanelUI = narrativePanelUI;
        _curtainUIHandler = curtainUIHandler;
        _initOperation = initOperation;
        _switchToContactsScreenCommand = new ReactiveCommand().AddTo(compositeDisposable);
        _switchToDialogScreenCommand = new ReactiveCommand<PhoneContactDataLocalizable>().AddTo(compositeDisposable);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_notificationNameLocalizationString};
        // return _blockScreenHandler.GetLocalizableContent();
    }

    public void Init(PhoneUIView phoneUIView)
    {
        _switchToContactsScreenCommand.Subscribe(_=>
        {
            SetContactsScreenBackgroundFromAnotherScreen();
        });
        _switchToDialogScreenCommand.Subscribe(SetDialogScreenBackgroundFromAnotherScreen);
        _phoneUIGameObject = phoneUIView.gameObject;
        var contactsShower = new ContactsShower(
            phoneUIView.ContactsScreenViewBackground.ContactsTransform.GetComponent<VerticalLayoutGroup>(),
            phoneUIView.ContactsScreenViewBackground.ContactsTransform.GetComponent<ContentSizeFitter>(),
            phoneUIView.ContactsScreenViewBackground.ContactsTransform, GetOnlineStatus);
        var messagesShower = new MessagesShower(_curtainUIHandler, _narrativePanelUI);
        _phoneContentProvider.Init(phoneUIView.DialogScreenViewBackground.DialogTransform, phoneUIView.ContactsScreenViewBackground.transform,
            phoneUIView.BlockScreenViewBackground.transform);
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorRectTransform, phoneUIView.SignalIndicatorImage, phoneUIView.TimeText,
            phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        _blockScreenHandler = new BlockScreenHandler(phoneUIView.BlockScreenViewBackground, _topPanelHandler, _phoneContentProvider.NotificationViewPool,
            _switchToDialogScreenCommand, _notificationNameLocalizationString, _switchToContactsScreenCommand);
        _contactsScreenHandler = new ContactsScreenHandler(phoneUIView.ContactsScreenViewBackground, contactsShower,
            _topPanelHandler, _switchToDialogScreenCommand, _phoneContentProvider.ContactsPool);
        _dialogScreenHandler = new DialogScreenHandler(phoneUIView.DialogScreenViewBackground, messagesShower, _topPanelHandler,
            _phoneContentProvider.IncomingMessagePool, _phoneContentProvider.OutcomingMessagePool, _switchToContactsScreenCommand);
    }

    public void DisposeScreensBackgrounds()
    {
        _phoneTime?.Stop();
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _phoneUIGameObject.SetActive(false);
        _topPanelHandler.Dispose();
    }

    public void ConstructFromNode(IReadOnlyList<ContactInfoToGame> contactsInfoToGame,
        Phone phone, SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        bool playModeKey, int butteryPercent, int startHour, int startMinute)
    {
        _initOperation?.Invoke();
        _initOperation = null;
        if (contactsInfoToGame != null)
        {
            _contactsInfoToGame = contactsInfoToGame.ToDictionary(x=>x.KeyName);
        }

        _currentPhone = phone;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _switchToNextNodeEvent = switchToNextNodeEvent;
        TryStartPhoneTime(startHour, startMinute, playModeKey);
        _topPanelHandler.Init(_phoneTime, playModeKey, butteryPercent);
        _phoneUIGameObject.SetActive(true);
    }
    public void SetBlockScreenBackgroundFromNode(LocalizationString date,
        int startScreenCharacterIndex, bool playModeKey)
    {
        DisableScreens();
        _blockScreenHandler.Enable(_contactsInfoToGame, _phoneTime, _currentPhone, date,
            _setLocalizationChangeEvent, startScreenCharacterIndex, playModeKey);
    }

    public void SetContactsScreenBackgroundFromNode()
    {
        DisableScreens();
        _contactsScreenHandler.Enable(
            _currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable, _setLocalizationChangeEvent, _switchToNextNodeEvent);
    }

    public void SetDialogScreenBackgroundFromNode(int startScreenCharacterIndex)
    {
        DisableScreens();
        PhoneContactDataLocalizable contact =
            _currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable[startScreenCharacterIndex];
        _dialogScreenHandler.Enable(contact, _setLocalizationChangeEvent, _switchToNextNodeEvent, SetOnlineStatus, GetOnlineStatus(contact.NameContact.Key));
    }

    private void SetDialogScreenBackgroundFromAnotherScreen(PhoneContactDataLocalizable contact)
    {
        DisableScreens();
        _dialogScreenHandler.Enable(contact, _setLocalizationChangeEvent, _switchToNextNodeEvent, SetOnlineStatus, GetOnlineStatus(contact.NameContact.Key));
    }

    private void SetContactsScreenBackgroundFromAnotherScreen()
    {
        DisableScreens();
        _contactsScreenHandler.Enable(_currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable, _setLocalizationChangeEvent, _switchToNextNodeEvent);
    }

    public void TryRestartPhoneTime(int startMinute)
    {
        if (_phoneTime != null)
        {
            if (startMinute > 0 && _phoneTime.IsStarted)
            {
                _phoneTime.Stop();
                _phoneTime.Restart(startMinute);
            }
        }
    }
    private void TryStartPhoneTime(int startHour, int startMinute, bool playModeKey)
    {
        if (_phoneTime == null)
        {
            _phoneTime = new PhoneTime();
        }
        if (_phoneTime.IsStarted == false)
        {
            _phoneTime.Start(startHour, startMinute, playModeKey).Forget();
        }
    }

    private bool GetOnlineStatus(string nameKey)
    {
        bool result = false;
        if (_contactsInfoToGame.TryGetValue(nameKey, out ContactInfoToGame contactInfoToGame))
        {
            result = contactInfoToGame.KeyOnline;
        }
        return result;
    }
    private void SetOnlineStatus(string nameKey, bool key = false)
    {
        if (_contactsInfoToGame.TryGetValue(nameKey, out ContactInfoToGame contactInfoToGame))
        {
            contactInfoToGame.KeyOnline = key;
        }
    }
    private void DisableScreens()
    {
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _blockScreenHandler.Disable();
    }
}

public enum PhoneBackgroundScreen
{
    BlockScreen = 0, ContactsScreen = 1, DialogScreen = 2 
}