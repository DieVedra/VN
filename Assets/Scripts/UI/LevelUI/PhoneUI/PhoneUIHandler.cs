using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUIHandler : ILocalizable
{
    public const int PhoneSiblingIndex = 5;
    private const float _scale = 1f;
    private const float _left = 439f;
    private const float _top = -1010f;
    private const float _right = -439f;
    private const float _bottom = 1010f;
    private readonly PhoneContentProvider _phoneContentProvider;
    private readonly NarrativePanelUIHandler _narrativePanelUI;
    private readonly CustomizationCurtainUIHandler _curtainUIHandler;
    private readonly ReactiveCommand _switchToContactsScreenCommand;
    private readonly ReactiveCommand<PhoneContact> _switchToDialogScreenCommand;
    private LocalizationString _notificationNameLocalizationString = "Получено новое сообщение!";
    private IReadOnlyDictionary<string, ContactInfoToGame> _contactsInfoToGame;
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
    private NodeGraphInitializer _nodeGraphInitializer;
    private int _seriaIndex;
    public PhoneTime PhoneTime => _phoneTime;
    public PhoneUIHandler(PhoneContentProvider phoneContentProvider, NarrativePanelUIHandler narrativePanelUI,
        CustomizationCurtainUIHandler curtainUIHandler, CompositeDisposable compositeDisposable, Action initOperation)
    {
        _phoneContentProvider = phoneContentProvider;
        _narrativePanelUI = narrativePanelUI;
        _curtainUIHandler = curtainUIHandler;
        _initOperation = initOperation;
        _switchToContactsScreenCommand = new ReactiveCommand().AddTo(compositeDisposable);
        _switchToDialogScreenCommand = new ReactiveCommand<PhoneContact>().AddTo(compositeDisposable);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_notificationNameLocalizationString};
    }

    public void Init(PhoneUIView phoneUIView, NodeGraphInitializer nodeGraphInitializer)
    {
        _nodeGraphInitializer = nodeGraphInitializer;
        _switchToContactsScreenCommand.Subscribe(_=>
        {
            SetContactsScreenBackgroundFromAnotherScreen();
        });
        // _switchToDialogScreenCommand.Subscribe(SetDialogScreenBackgroundFromAnotherScreen);
        _phoneUIGameObject = phoneUIView.gameObject;
        if (phoneUIView.transform is RectTransform rectTransform)
        {
            var pos = new Vector3(_scale, _scale, _scale);
            rectTransform.localScale = pos;
            pos.x = _left;
            pos.y = _bottom;
            rectTransform.offsetMin = pos; //left, bottom
            pos.x = _right;
            pos.y = _top;
            rectTransform.offsetMax = pos; //right, top
            var localPosition = rectTransform.localPosition;
            pos.x = localPosition.x;
            pos.y = localPosition.y;
            pos.z = 0;
            localPosition = pos;
            rectTransform.localPosition = localPosition;
        }

        var contactsShower = new ContactsShower(
            phoneUIView.ContactsScreenViewBackground.ContactsTransform.GetComponent<VerticalLayoutGroup>(),
            phoneUIView.ContactsScreenViewBackground.ContactsTransform.GetComponent<ContentSizeFitter>(),
            phoneUIView.ContactsScreenViewBackground.ContactsTransform, GetOnlineStatus);
        var messagesShower = new MessagesShower(phoneUIView.DialogScreenViewBackground.ReadDialogButtonButton , _curtainUIHandler, _narrativePanelUI, phoneUIView.transform.GetSiblingIndex());
        _phoneContentProvider.Init(phoneUIView.DialogScreenViewBackground.DialogTransform, phoneUIView.ContactsScreenViewBackground.transform,
            phoneUIView.BlockScreenViewBackground.transform);
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorRectTransform, phoneUIView.SignalIndicatorImage, phoneUIView.TimeText,
            phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        // _blockScreenHandler = new BlockScreenHandler(phoneUIView.BlockScreenViewBackground, _topPanelHandler, _phoneContentProvider.NotificationViewPool,
        //     _switchToDialogScreenCommand, _notificationNameLocalizationString, _switchToContactsScreenCommand);
        // _contactsScreenHandler = new ContactsScreenHandler(phoneUIView.ContactsScreenViewBackground, contactsShower,
        //     _topPanelHandler, _switchToDialogScreenCommand, _phoneContentProvider.ContactsPool);
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
    public void ConstructFromNode(IReadOnlyDictionary<string, ContactInfoToGame> contactsInfoToGame,
        Phone phone, SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        bool playModeKey, int seriaIndex, int butteryPercent, int startHour, int startMinute)
    {
        _initOperation?.Invoke();
        _initOperation = null;
        _contactsInfoToGame = contactsInfoToGame;
        _currentPhone = phone;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _switchToNextNodeEvent = switchToNextNodeEvent;
        _seriaIndex = seriaIndex;
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
        // _contactsScreenHandler.Enable(
        //     _currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable, _setLocalizationChangeEvent, _switchToNextNodeEvent);
    }

    public void SetDialogScreenBackgroundFromNode(int startScreenCharacterIndex)
    {
        DisableScreens();
        // PhoneContactDataLocalizable contact =
        //     _currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable[startScreenCharacterIndex];
        // _dialogScreenHandler.Enable(contact, _setLocalizationChangeEvent, _nodeGraphInitializer, SetOnlineStatus, GetOnlineStatus(contact.NameContact.Key), _seriaIndex);
    }

    private void SetDialogScreenBackgroundFromAnotherScreen(PhoneContact contact)
    {
        DisableScreens();
        // _dialogScreenHandler.Enable(contact, _setLocalizationChangeEvent, _nodeGraphInitializer, SetOnlineStatus, GetOnlineStatus(contact.NameContact.Key), _seriaIndex);
    }

    private void SetContactsScreenBackgroundFromAnotherScreen()
    {
        DisableScreens();
        // _contactsScreenHandler.Enable(_currentPhone.PhoneDataLocalizable.PhoneContactDatasLocalizable, _setLocalizationChangeEvent, _switchToNextNodeEvent);
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