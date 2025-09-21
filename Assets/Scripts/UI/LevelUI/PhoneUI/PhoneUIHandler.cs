
using Cysharp.Threading.Tasks;

public class PhoneUIHandler
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
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorImage, phoneUIView.TimeText, phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        _blockScreenHandler = new BlockScreenHandler(phoneUIView.BlockScreenViewBackground, _topPanelHandler);
        _contactsScreenHandler = new ContactsScreenHandler(phoneUIView.ContactsScreenViewBackground, _topPanelHandler);
        _dialogScreenHandler = new DialogScreenHandler(phoneUIView.DialogScreenViewBackground, _topPanelHandler);
    }

    public void SetBackground(PhoneBackgroundScreen typeBackgroundScreen, bool restartPhoneTimeKey, int startHour, int startMinute, int data, int butteryPercent)
    {
        TryStartPhoneTime(startHour, startMinute, data, restartPhoneTimeKey);
        switch (typeBackgroundScreen)
        {
            case PhoneBackgroundScreen.BlockScreen:
                SetBlockScreenBackground(butteryPercent);
                break;
            case PhoneBackgroundScreen.ContactsScreen:
                SetContactsScreenBackground(butteryPercent);
                break;
            case PhoneBackgroundScreen.DialogScreen:
                SetDialogScreenBackground(butteryPercent);
                break;
        }
    }

    public void DisposeBackgrounds()
    {
        _phoneTime?.Stop();
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
    }

    public void SetBlockScreenBackground(int butteryPercent)
    {

        // _blockScreenHandler.Enable(_phoneTime, butteryPercent);
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
    }
    public void SetContactsScreenBackground(int butteryPercent)
    {
        _blockScreenHandler.Disable();
        // _contactsScreenHandler.Enable(_phoneTime, butteryPercent);
        _dialogScreenHandler.Disable();
    }
    public void SetDialogScreenBackground(int butteryPercent)
    {
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        // _dialogScreenHandler.Enable(_phoneTime, butteryPercent);
    }

    private void TryStartPhoneTime(int startHour, int startMinute, int data, bool restartPhoneTimeKey)
    {
        if (_phoneTime == null)
        {
            _phoneTime = new PhoneTime();
        }

        if (_phoneTime.IsStarted == true && restartPhoneTimeKey == true)
        {
            _phoneTime.Stop();
            _phoneTime.Start(startHour, startMinute, data).Forget();

        }
        
        if (_phoneTime.IsStarted == false)
        {
            _phoneTime.Start(startHour, startMinute, data).Forget();
        }
    }
}

public enum PhoneBackgroundScreen
{
    BlockScreen = 0, ContactsScreen = 1, DialogScreen = 2 
}