using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class PhoneUIHandler : ILocalizable
{
    public const int PhoneSiblingIndex = 5;
    private const int _caseCount = 8;
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
    private readonly PhoneMessagesExtractor _phoneMessagesExtractor;
    private readonly ReactiveCommand _tryShowReactiveCommand;
    private LocalizationString _notificationNameLocalizationString = "Получено новое сообщение!";
    private List<ContactNodeCase> _sortedPhoneNodeCases;
    private List<ContactInfo> _sortedOnlineContacts;
    private List<ContactInfo> _sortedNotifications;
    private Action _initOperation;
    private TopPanelHandler _topPanelHandler;
    private BlockScreenHandler _blockScreenHandler;
    private ContactsScreenHandler _contactsScreenHandler;
    private DialogScreenHandler _dialogScreenHandler;
    private ContactPrintStatusHandler _contactPrintStatusHandler;
    private PhoneTime _phoneTime;
    private SwitchToNextNodeEvent _switchToNextNodeEvent;
    private Phone _currentPhone;
    private GameObject _phoneUIGameObject;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private LocalizationString _date;
    private int _phoneSiblingIndex;
    private int _seriaIndex;
    private bool _playModeKey;
    public PhoneTime PhoneTime => _phoneTime;
    public PhoneUIHandler(PhoneContentProvider phoneContentProvider, NarrativePanelUIHandler narrativePanelUI,
        CustomizationCurtainUIHandler curtainUIHandler, CompositeDisposable compositeDisposable, Action initOperation)
    {
        _phoneContentProvider = phoneContentProvider;
        _narrativePanelUI = narrativePanelUI;
        _curtainUIHandler = curtainUIHandler;
        _initOperation = initOperation;
        _sortedPhoneNodeCases = new List<ContactNodeCase>(_caseCount);
        _sortedOnlineContacts = new List<ContactInfo>(_caseCount);
        _sortedNotifications = new List<ContactInfo>(_caseCount);
        _tryShowReactiveCommand = new ReactiveCommand();
        _phoneMessagesExtractor = new PhoneMessagesExtractor(_tryShowReactiveCommand);
        _switchToContactsScreenCommand = new ReactiveCommand().AddTo(compositeDisposable);
        _switchToDialogScreenCommand = new ReactiveCommand<PhoneContact>().AddTo(compositeDisposable);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_notificationNameLocalizationString, _contactPrintStatusHandler.PrintLocalizationString };
    }

    public void Init(PhoneUIView phoneUIView)
    {
        _switchToContactsScreenCommand.Subscribe(_=>
        {
            SetContactsScreenBackgroundFromAnotherScreen();
        });
        _switchToDialogScreenCommand.Subscribe(SetDialogScreenBackgroundFromAnotherScreen);
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
        _contactPrintStatusHandler = new ContactPrintStatusHandler(phoneUIView.DialogScreenViewBackground.PrintsImages,
            phoneUIView.DialogScreenViewBackground.ContactOnlineStatus.transform.parent.gameObject,
            phoneUIView.DialogScreenViewBackground.PrintsText);
        var messagesShower = new MessagesShower(phoneUIView.DialogScreenViewBackground.DialogTransform, _contactPrintStatusHandler, _phoneMessagesExtractor,
            phoneUIView.DialogScreenViewBackground.ReadDialogButtonButton, _tryShowReactiveCommand);
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
        _phoneSiblingIndex = phoneUIView.transform.GetSiblingIndex();
    }

    public void DisposeScreensBackgrounds()
    {
        _phoneTime?.Stop();
        _blockScreenHandler.Disable();
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _phoneUIGameObject.SetActive(false);
        _topPanelHandler.Dispose();
        _phoneMessagesExtractor?.Dispose();
        _contactPrintStatusHandler?.Dispose();
    }
    public int ConstructFromNode(IReadOnlyList<ContactNodeCase> phoneNodeCases, IReadOnlyList<ContactInfo> onlineContacts,
        IReadOnlyList<ContactInfo> notificationsInBlockScreen,
        Phone phone, SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        LocalizationString date,
        bool playModeKey, int seriaIndex, int butteryPercent, int startHour, int startMinute)
    {
        _initOperation?.Invoke();
        SortingCases(phone.PhoneContactDictionary, phoneNodeCases, onlineContacts, notificationsInBlockScreen);
        _initOperation = null;
        _currentPhone = phone;
        _date = date;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _switchToNextNodeEvent = switchToNextNodeEvent;
        _seriaIndex = seriaIndex;
        _playModeKey = playModeKey;
        TryStartPhoneTime(startHour, startMinute, playModeKey);
        _topPanelHandler.Init(_phoneTime, playModeKey, butteryPercent);
        _phoneUIGameObject.SetActive(true);
        _contactsScreenHandler.Init(_sortedPhoneNodeCases);
        _dialogScreenHandler.Init(_sortedPhoneNodeCases);
        SetBlockScreenBackgroundFromNode();
        return _phoneSiblingIndex;
    }
    public void SetBlockScreenBackgroundFromNode()
    {
        DisableScreens();
        
        
        _blockScreenHandler.Enable(_sortedPhoneNodeCases, _sortedNotifications, _sortedOnlineContacts, _phoneTime, _currentPhone, _date,
            _setLocalizationChangeEvent, _playModeKey);
    }

    private void SetDialogScreenBackgroundFromAnotherScreen(PhoneContact contact)
    {
        //as
        DisableScreens();
        _dialogScreenHandler.Enable(contact, _setLocalizationChangeEvent, SetOnlineStatus, GetOnlineStatus(contact.NameLocalizationString.Key), _seriaIndex);
    }

    private void SetContactsScreenBackgroundFromAnotherScreen()
    {
        //as

        DisableScreens();
        _contactsScreenHandler.Enable(_currentPhone.PhoneContactDictionary, _setLocalizationChangeEvent, _switchToNextNodeEvent);
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
        for (int i = 0; i < _sortedOnlineContacts.Count; i++)
        {
            if (_sortedOnlineContacts[i].ContactKey == nameKey)
            {
                result = true;
            }
        }
        return result;
    }
    private void SetOnlineStatus(string nameKey, bool key = false)
    {
        for (int i = _sortedOnlineContacts.Count - 1; i >= 0; i++)
        {
            if (_sortedOnlineContacts[i].ContactKey == nameKey)
            {
                _sortedOnlineContacts.RemoveAt(i);
            }
        }
    }
    private void DisableScreens()
    {
        _contactsScreenHandler.Disable();
        _dialogScreenHandler.Disable();
        _blockScreenHandler.Disable();
    }

    private void SortingCases(IReadOnlyDictionary<string, PhoneContact> phoneContacts, IReadOnlyList<ContactNodeCase> phoneNodeCases, 
        IReadOnlyList<ContactInfo> onlineContacts, IReadOnlyList<ContactInfo> notifications)
    {
        _sortedPhoneNodeCases.Clear();
        _sortedOnlineContacts.Clear();
        _sortedNotifications.Clear();
        foreach (var contact in phoneContacts)
        {
            Sorting(phoneNodeCases, _sortedPhoneNodeCases, contact.Key);
            Sorting(onlineContacts, _sortedOnlineContacts, contact.Key);
            Sorting(notifications, _sortedNotifications, contact.Key);
        }
        
        void Sorting<T>(IEnumerable<T> info, ICollection<T> sortedInfo, string key) where T : ContactInfo
        {
            foreach (var contactInfo in info)
            {
                if (contactInfo.ContactKey == key)
                {
                    sortedInfo.Add(contactInfo);
                }
            }
        }
    }
}