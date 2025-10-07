using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class PhoneUIHandler : ILocalizable
{
    private readonly PhoneContentProvider _phoneContentProvider;
    private readonly NarrativePanelUIHandler _narrativePanelUI;
    private readonly CustomizationCurtainUIHandler _curtainUIHandler;
    private TopPanelHandler _topPanelHandler;
    private BlockScreenHandler _blockScreenHandler;
    private ContactsScreenHandler _contactsScreenHandler;
    private DialogScreenHandler _dialogScreenHandler;
    private PhoneTime _phoneTime;
    private ReactiveCommand _switchToContactsScreenCommand;
    private SwitchToNextNodeEvent _switchToNextNodeEvent;
    private ReactiveCommand<PhoneContactDataLocalizable> _switchToDialogScreenCommand;
    private CompositeDisposable _compositeDisposable;
    private Phone _currentPhone;
    private GameObject _phoneUIGameObject;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private IReadOnlyList<ContactInfoToGame> _onlineContacts;

    public PhoneUIHandler(PhoneContentProvider phoneContentProvider, NarrativePanelUIHandler narrativePanelUI, CustomizationCurtainUIHandler curtainUIHandler)
    {
        _phoneContentProvider = phoneContentProvider;
        _narrativePanelUI = narrativePanelUI;
        _curtainUIHandler = curtainUIHandler;
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return _blockScreenHandler.GetLocalizableContent();
    }

    public void Init(PhoneUIView phoneUIView)
    {
        _compositeDisposable = new CompositeDisposable();
        _switchToContactsScreenCommand = new ReactiveCommand().AddTo(_compositeDisposable);
        _switchToDialogScreenCommand = new ReactiveCommand<PhoneContactDataLocalizable>().AddTo(_compositeDisposable);
        _switchToContactsScreenCommand.Subscribe(_=>
        {
            SetContactsScreenBackgroundFromAnotherScreen();
        });
        _switchToDialogScreenCommand.Subscribe(SetDialogScreenBackgroundFromAnotherScreen);
        _phoneUIGameObject = phoneUIView.gameObject;
        var contactsShower = new ContactsShower(GetOnlineStatus);
        var messagesShower = new MessagesShower(_curtainUIHandler, _narrativePanelUI);
        _phoneContentProvider.Init(phoneUIView.DialogScreenViewBackground.DialogTransform, phoneUIView.ContactsScreenViewBackground.ContactsTransform);
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorRectTransform, phoneUIView.SignalIndicatorImage, phoneUIView.TimeText, phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        _blockScreenHandler = new BlockScreenHandler(phoneUIView.BlockScreenViewBackground, _topPanelHandler, _phoneContentProvider.NotificationViewPrefab, _switchToDialogScreenCommand, _switchToContactsScreenCommand);
        _contactsScreenHandler = new ContactsScreenHandler(phoneUIView.ContactsScreenViewBackground, contactsShower, _topPanelHandler, _switchToDialogScreenCommand, _phoneContentProvider.ContactsPool);
        _dialogScreenHandler = new DialogScreenHandler(phoneUIView.DialogScreenViewBackground, messagesShower, _topPanelHandler,
            _phoneContentProvider.IncomingMessagePool, _phoneContentProvider.OutcomingMessagePool, _switchToContactsScreenCommand);
    }

    public void DisposeScreensBackgrounds()
    {
        _phoneTime?.Stop();
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _compositeDisposable?.Clear();
        _switchToContactsScreenCommand = null;
        _switchToDialogScreenCommand = null;
        _phoneUIGameObject.SetActive(false);
        _topPanelHandler.Dispose();
    }

    public void ConstructFromNode(IReadOnlyList<string> onlineContacts,
        Phone phone, SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        bool playModeKey, int butteryPercent, int startHour, int startMinute)
    {
        // _onlineContacts = onlineContacts.ToList();
        _currentPhone = phone;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _switchToNextNodeEvent = switchToNextNodeEvent;
        TryStartPhoneTime(startHour, startMinute, playModeKey);
        _topPanelHandler.Init(_phoneTime, playModeKey, butteryPercent);
        _phoneUIGameObject.SetActive(true);
    }
    public void SetBlockScreenBackgroundFromNode(LocalizationString date,
        int startScreenCharacterIndex, bool playModeKey, bool blockScreenNotificationKey)
    {
        DisableScreens();
        _blockScreenHandler.Enable(_phoneTime, _currentPhone, date,
            _setLocalizationChangeEvent, startScreenCharacterIndex, playModeKey, blockScreenNotificationKey);
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
        _dialogScreenHandler.Enable(contact, _setLocalizationChangeEvent, _switchToNextNodeEvent, SetOnlineStatus, GetOnlineStatus(contact.NameContactLocalizationString.Key));
    }

    private void SetDialogScreenBackgroundFromAnotherScreen(PhoneContactDataLocalizable contact)
    {
        DisableScreens();
        _dialogScreenHandler.Enable(contact, _setLocalizationChangeEvent, _switchToNextNodeEvent, SetOnlineStatus, GetOnlineStatus(contact.NameContactLocalizationString.Key));
    }

    private void SetContactsScreenBackgroundFromAnotherScreen()
    {
        DisableScreens();
        _contactsScreenHandler.Enable(_currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable, _setLocalizationChangeEvent, _switchToNextNodeEvent);
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
        for (int i = 0; i < _onlineContacts.Count; i++)
        {
            if (_onlineContacts[i].KeyName == nameKey)
            {
                result = _onlineContacts[i].Key;
            }
        }
        return result;
    }
    private void SetOnlineStatus(string nameKey, bool key = false)
    {
        for (int i = 0; i < _onlineContacts.Count; i++)
        {
            if (_onlineContacts[i].KeyName == nameKey)
            {
                // _onlineContacts[i].Key = key;
            }
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