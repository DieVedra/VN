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
    private ReactiveCommand<PhoneBackgroundScreen> _switchPanelCommand;
    public PhoneUIHandler()
    {
        _switchPanelCommand = new ReactiveCommand<PhoneBackgroundScreen>();
    }

    public void Init(PhoneUIView phoneUIView)
    {
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorRectTransform, phoneUIView.SignalIndicatorImage, phoneUIView.TimeText, phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        _blockScreenHandler = new BlockScreenHandler(phoneUIView.BlockScreenViewBackground, _topPanelHandler, _switchPanelCommand);
        _contactsScreenHandler = new ContactsScreenHandler(phoneUIView.ContactsScreenViewBackground, _topPanelHandler, _switchPanelCommand);
        _dialogScreenHandler = new DialogScreenHandler(phoneUIView.DialogScreenViewBackground, _topPanelHandler, _switchPanelCommand);
    }

    public void DisposeBackgrounds()
    {
        _phoneTime?.Stop();
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
    }

    public void SetBlockScreenBackground(Phone phone, LocalizationString date, SetLocalizationChangeEvent setLocalizationChangeEvent,
        int startScreenCharacterIndex, int butteryPercent, int startHour, int startMinute,
        bool playModeKey, bool blockScreenNotificationKey, bool restartPhoneTimeKey = true)
    {
        TryStartPhoneTime(startHour, startMinute, restartPhoneTimeKey, playModeKey);
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _blockScreenHandler.Enable(_phoneTime, phone, date,
            setLocalizationChangeEvent, butteryPercent, startScreenCharacterIndex, playModeKey, blockScreenNotificationKey);
    }
    public void SetContactsScreenBackground(IReadOnlyList<PhoneContactDataLocalizable> phoneContactDatasLocalizable, SetLocalizationChangeEvent setLocalizationChangeEvent,
        int butteryPercent, int startHour, int startMinute, bool playModeKey, bool restartPhoneTimeKey = true)
    {
        TryStartPhoneTime(startHour, startMinute, restartPhoneTimeKey, playModeKey);
        _blockScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _contactsScreenHandler.Enable(_phoneTime, phoneContactDatasLocalizable, setLocalizationChangeEvent, butteryPercent, playModeKey);
    }
    public void SetDialogScreenBackground(PhoneContactDataLocalizable contact, SetLocalizationChangeEvent setLocalizationChangeEvent,
        int butteryPercent, int startHour, int startMinute, bool characterOnlineKey, bool playModeKey, bool restartPhoneTimeKey = true)
    {
        TryStartPhoneTime(startHour, startMinute, restartPhoneTimeKey, playModeKey);
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Enable(_phoneTime, contact, setLocalizationChangeEvent, butteryPercent, playModeKey, characterOnlineKey);
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

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return _blockScreenHandler.GetLocalizableContent();
    }
}

public enum PhoneBackgroundScreen
{
    BlockScreen = 0, ContactsScreen = 1, DialogScreen = 2 
}