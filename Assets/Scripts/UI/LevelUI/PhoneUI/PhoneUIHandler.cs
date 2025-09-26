
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PhoneUIHandler : ILocalizable
{
    private TopPanelHandler _topPanelHandler;
    private BlockScreenHandler _blockScreenHandler;
    private ContactsScreenHandler _contactsScreenHandler;
    private DialogScreenHandler _dialogScreenHandler;
    private PhoneTime _phoneTime;
    public PhoneUIHandler()
    {
        
    }

    public void Init(PhoneUIView phoneUIView)
    {
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorRectTransform, phoneUIView.SignalIndicatorImage, phoneUIView.TimeText, phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        _blockScreenHandler = new BlockScreenHandler(phoneUIView.BlockScreenViewBackground, _topPanelHandler);
        _contactsScreenHandler = new ContactsScreenHandler(phoneUIView.ContactsScreenViewBackground, _topPanelHandler);
        _dialogScreenHandler = new DialogScreenHandler(phoneUIView.DialogScreenViewBackground, _topPanelHandler);
    }

    // public void SetBackground(PhoneBackgroundScreen typeBackgroundScreen, bool restartPhoneTimeKey, int startHour, int startMinute, int date, int butteryPercent)
    // {
    //     TryStartPhoneTime(startHour, startMinute, date, restartPhoneTimeKey);
    //     switch (typeBackgroundScreen)
    //     {
    //         case PhoneBackgroundScreen.BlockScreen:
    //             SetBlockScreenBackground(butteryPercent);
    //             break;
    //         case PhoneBackgroundScreen.ContactsScreen:
    //             SetContactsScreenBackground(butteryPercent);
    //             break;
    //         case PhoneBackgroundScreen.DialogScreen:
    //             SetDialogScreenBackground(butteryPercent);
    //             break;
    //     }
    // }

    public void DisposeBackgrounds()
    {
        _phoneTime?.Stop();
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
    }

    public void SetBlockScreenBackground(PhoneContactDataLocalizable contact, LocalizationString date, SetLocalizationChangeEvent setLocalizationChangeEvent,
        int butteryPercent, int startHour, int startMinute, bool playModeKey, bool restartPhoneTimeKey)
    {
        TryStartPhoneTime(startHour, startMinute, date, restartPhoneTimeKey, playModeKey);
        _blockScreenHandler.Enable(_phoneTime, contact, date, setLocalizationChangeEvent, butteryPercent, playModeKey);
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
    }
    public void SetContactsScreenBackground(int butteryPercent, bool playModeKey)
    {
        // TryStartPhoneTime(startHour, startMinute, data, restartPhoneTimeKey);

        _blockScreenHandler.Disable();
        // _contactsScreenHandler.Enable(_phoneTime, butteryPercent);
        _dialogScreenHandler.Disable();
    }
    public void SetDialogScreenBackground(int butteryPercent, bool playModeKey)
    {
        // TryStartPhoneTime(startHour, startMinute, data, restartPhoneTimeKey);

        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        // _dialogScreenHandler.Enable(_phoneTime, butteryPercent);
    }

    private void TryStartPhoneTime(int startHour, int startMinute, string date, bool restartPhoneTimeKey, bool playModeKey)
    {
        if (_phoneTime == null)
        {
            _phoneTime = new PhoneTime();
        }

        if (_phoneTime.IsStarted == true && restartPhoneTimeKey == true)
        {
            _phoneTime.Stop();
            _phoneTime.Start(startHour, startMinute, date, playModeKey).Forget();

        }
        
        if (_phoneTime.IsStarted == false)
        {
            _phoneTime.Start(startHour, startMinute, date, playModeKey).Forget();
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