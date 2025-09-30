using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

public class PhoneUIHandler : ILocalizable
{
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
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private IReadOnlyList<ContactInfoToOnlineStatus> _onlineContacts;

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
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorRectTransform, phoneUIView.SignalIndicatorImage, phoneUIView.TimeText, phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        _blockScreenHandler = new BlockScreenHandler(phoneUIView.BlockScreenViewBackground, _topPanelHandler, _switchToDialogScreenCommand, _switchToContactsScreenCommand);
        _contactsScreenHandler = new ContactsScreenHandler(phoneUIView.ContactsScreenViewBackground, _topPanelHandler, _switchToDialogScreenCommand, GetOnlineStatus);
        _dialogScreenHandler = new DialogScreenHandler(phoneUIView.DialogScreenViewBackground, _topPanelHandler, _switchToContactsScreenCommand);
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
    }

    public void ConstructFromNode(IReadOnlyList<ContactInfoToOnlineStatus> onlineContacts,
        Phone phone, SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent)
    {
        _onlineContacts = onlineContacts;
        _currentPhone = phone;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _switchToNextNodeEvent = switchToNextNodeEvent;
    }
    public void SetBlockScreenBackgroundFromNode(LocalizationString date,
        int startScreenCharacterIndex, int butteryPercent, int startHour, int startMinute,
        bool playModeKey, bool blockScreenNotificationKey, bool restartPhoneTimeKey = false)
    {
        TryStartPhoneTime(startHour, startMinute, restartPhoneTimeKey, playModeKey);
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _blockScreenHandler.Enable(_phoneTime, _currentPhone, date,
            _setLocalizationChangeEvent, butteryPercent, startScreenCharacterIndex, playModeKey, blockScreenNotificationKey);
    }

    public void SetContactsScreenBackgroundFromNode(
        int butteryPercent, int startHour, int startMinute, bool playModeKey, bool restartPhoneTimeKey = false)
    {
        TryStartPhoneTime(startHour, startMinute, restartPhoneTimeKey, playModeKey);
        _blockScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _contactsScreenHandler.Enable(_phoneTime, _currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable,
            _setLocalizationChangeEvent, _switchToNextNodeEvent, butteryPercent, playModeKey);
    }

    public void SetDialogScreenBackgroundFromNode(int startScreenCharacterIndex,
        int butteryPercent, int startHour, int startMinute, bool playModeKey, bool restartPhoneTimeKey = false)
    {
        TryStartPhoneTime(startHour, startMinute, restartPhoneTimeKey, playModeKey);
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        PhoneContactDataLocalizable contact =
            _currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable[startScreenCharacterIndex];
        _dialogScreenHandler.Enable(_phoneTime, contact,
            _setLocalizationChangeEvent, butteryPercent, playModeKey, GetOnlineStatus(contact.NameContactLocalizationString.Key));
    }

    private void SetDialogScreenBackgroundFromAnotherScreen(PhoneContactDataLocalizable contact)
    {
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Enable(contact, _setLocalizationChangeEvent, GetOnlineStatus(contact.NameContactLocalizationString.Key));
    }

    private void SetContactsScreenBackgroundFromAnotherScreen()
    {
        _blockScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _contactsScreenHandler.Enable(_currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable, _setLocalizationChangeEvent, _switchToNextNodeEvent);
    }

    private void TryStartPhoneTime(int startHour, int startMinute, bool restartPhoneTimeKey, bool playModeKey)
    {
        if (_phoneTime == null)
        {
            _phoneTime = new PhoneTime();
        }

        if (_phoneTime.IsStarted == true && restartPhoneTimeKey == true)
        {
            _phoneTime.Stop();
            _phoneTime.Start(startHour, startMinute, playModeKey).Forget();

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
            if (_onlineContacts[i].Key == nameKey)
            {
                result = _onlineContacts[i].OnlineKey;
            }
        }
        return result;
    }
}

public enum PhoneBackgroundScreen
{
    BlockScreen = 0, ContactsScreen = 1, DialogScreen = 2 
}