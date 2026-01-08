using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class PhoneUIHandler : ILocalizable
{
    public const int PhoneSiblingIndex = 5;
    private const int _caseCount = 8;
    private const float _end1Value = 0.3f;
    private const float _end2Value = 0f;
    private const float _durationValue = 0.05f;
    private const float _scale = 1f;
    private const float _left = 439f;
    private const float _top = -1010f;
    private const float _right = -439f;
    private const float _bottom = 1010f;
    private readonly PhoneContentProvider _phoneContentProvider;
    private readonly ReactiveCommand _switchToContactsScreenCommand;
    private readonly ReactiveCommand<PhoneContact> _switchToDialogScreenCommand;
    private readonly PhoneMessagesExtractor _phoneMessagesExtractor;
    private readonly ReactiveCommand _tryShowReactiveCommand;

    private LocalizationString _notificationTextLocalizationString = "Получено новое сообщение!";
    private LocalizationString _printLocalizationString = "печатает";

    private List<ContactNodeCase> _sortedPhoneNodeCases;
    private Dictionary<string, OnlineContactInfo> _sortedOnlineContacts;
    private Dictionary<string, NotificationContactInfo> _sortedNotifications;
    private Action _initOperation;
    private Func<int, NodePort> _onGetNodePort;
    private TopPanelHandler _topPanelHandler;
    private PhoneSaveHandler _phoneSaveHandler;
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
    private PressDetector _pressDetector;
    private HashSet<string> _unreadebleContacts;
    private CancellationTokenSource _cancellationTokenSource;
    private Image _curtainImage;
    private Image _handImage;
    private Image _frameImage;
    private PhoneScreen _currentPhoneScreen;
    private int _phoneSiblingIndex;
    private bool _playModeKey;
    private bool _loadFromSave;
    
    public PhoneTime PhoneTime => _phoneTime;
    public PhoneUIHandler(PhoneContentProvider phoneContentProvider,
        CompositeDisposable compositeDisposable, Action initOperation)
    {
        _phoneContentProvider = phoneContentProvider;
        _initOperation = initOperation;
        _sortedPhoneNodeCases = new List<ContactNodeCase>(_caseCount);
        _sortedOnlineContacts = new Dictionary<string, OnlineContactInfo>(_caseCount);
        _sortedNotifications = new Dictionary<string, NotificationContactInfo>(_caseCount);
        _tryShowReactiveCommand = new ReactiveCommand();
        _phoneMessagesExtractor = new PhoneMessagesExtractor(_tryShowReactiveCommand);
        _switchToContactsScreenCommand = new ReactiveCommand().AddTo(compositeDisposable);
        _switchToDialogScreenCommand = new ReactiveCommand<PhoneContact>().AddTo(compositeDisposable);
        _unreadebleContacts = new HashSet<string>();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_notificationTextLocalizationString, _printLocalizationString };
    }

    public void Init(PhoneUIView phoneUIView, PhoneMessagesCustodian phoneMessagesCustodian, PhoneSaveHandler phoneSaveHandler, Func<int, NodePort> onGetNodePort)
    {
        _switchToContactsScreenCommand.Subscribe(_=>
        {
            SetContactsScreenBackgroundFromAnotherScreenWithTransitionAnim();
        });
        _phoneSaveHandler = phoneSaveHandler;
        _switchToDialogScreenCommand.Subscribe(SetDialogScreenBackgroundFromAnotherScreenWithTransitionAnim);
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
        _pressDetector = new PressDetector(phoneUIView.DialogScreenViewBackground.PressInputAction, phoneUIView.DialogScreenViewBackground.PositionInputAction);
        var contactsShower = new ContactsShower(phoneUIView.ContactsScreenViewBackground.ContactsTransform, GetOnlineStatus);
        _contactPrintStatusHandler = new ContactPrintStatusHandler(phoneUIView.DialogScreenViewBackground.PrintsImages,
            phoneUIView.DialogScreenViewBackground.ContactOnlineStatus.transform.parent.gameObject,
            phoneUIView.DialogScreenViewBackground.PrintsText, _printLocalizationString);
        var messagesShower = new MessagesShower(phoneUIView.DialogScreenViewBackground.DialogTransform, _contactPrintStatusHandler, _phoneMessagesExtractor,
            phoneMessagesCustodian, _pressDetector, _tryShowReactiveCommand);
        _phoneContentProvider.Init(phoneUIView.DialogScreenViewBackground.DialogTransform, phoneUIView.ContactsScreenViewBackground.transform,
            phoneUIView.BlockScreenViewBackground.transform);
        _topPanelHandler = new TopPanelHandler(phoneUIView.SignalIndicatorRectTransform, phoneUIView.SignalIndicatorImage, phoneUIView.TimeText,
            phoneUIView.ButteryText, phoneUIView.ButteryImage, phoneUIView.ButteryIndicatorImage);
        _blockScreenHandler = new BlockScreenHandler(_sortedNotifications, messagesShower, phoneUIView.BlockScreenViewBackground, _phoneContentProvider.NotificationViewPool,
            _switchToDialogScreenCommand, _notificationTextLocalizationString, _switchToContactsScreenCommand, GetOnlineStatus);
        _contactsScreenHandler = new ContactsScreenHandler(phoneUIView.ContactsScreenViewBackground.ScrollRect, _unreadebleContacts, phoneUIView.ContactsScreenViewBackground, contactsShower,
            _switchToDialogScreenCommand, _phoneContentProvider.ContactsPool);
        _dialogScreenHandler = new DialogScreenHandler(_sortedOnlineContacts, _unreadebleContacts, phoneUIView.DialogScreenViewBackground, messagesShower,
            _phoneContentProvider.IncomingMessagePool, _phoneContentProvider.OutcomingMessagePool, _switchToContactsScreenCommand);
        _phoneSiblingIndex = phoneUIView.transform.GetSiblingIndex();
        _curtainImage = phoneUIView.CurtainImage;
        _handImage = phoneUIView.HandImage;
        _frameImage = phoneUIView.FrameImage;
        _handImage.gameObject.SetActive(true);
        _frameImage.gameObject.SetActive(true);
        _cancellationTokenSource = new CancellationTokenSource();
        _initOperation = null;
        _onGetNodePort = onGetNodePort;
        phoneSaveHandler.OnSave += GetInfoToSave;
    }

    public void ShutdownScreensBackgrounds()
    {
        _phoneTime?.Stop();
        ShutdownScreens();
        _phoneUIGameObject.SetActive(false);
        _topPanelHandler.Shutdown();
        _phoneMessagesExtractor?.Shutdown();
        _contactPrintStatusHandler?.Shutdown();
        _cancellationTokenSource?.Cancel();
    }
    public int ConstructFromNode(IReadOnlyList<ContactNodeCase> phoneNodeCases, IReadOnlyList<OnlineContactInfo> onlineContacts,
        IReadOnlyList<NotificationContactInfo> notificationsInBlockScreen,
        Phone phone, SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        LocalizationString date,
        bool playModeKey, int butteryPercent, int startHour, int startMinute, int handIndex)
    {
        _initOperation?.Invoke();
        _loadFromSave = _phoneSaveHandler.LoadFromSaveKey;
        SortingCases(phone.PhoneContactDictionary, phoneNodeCases, onlineContacts, notificationsInBlockScreen);
        _currentPhone = phone;
        _date = date;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _switchToNextNodeEvent = switchToNextNodeEvent;
        _playModeKey = playModeKey;
        if (_loadFromSave == true)
        {
            TryStartPhoneTime(startHour, _phoneSaveHandler.CurrentPhoneMinute, playModeKey);
            foreach (var index in _phoneSaveHandler.ReadedContactNodeCaseIndexes)
            {
                _sortedPhoneNodeCases[index].IsReaded = true;
            }

            UpdateNotificationsFromSave();
            UpdateOnlineContactsFromSave();
            FillUnreadebleContacts();
            switch (_phoneSaveHandler.GetPhoneScreenIndex)
            {
                case (int)PhoneScreen.Block:
                    SetBlockScreenBackgroundFromNode(_onGetNodePort?.Invoke(_phoneSaveHandler.PhoneContentNodeIndex),
                        _phoneSaveHandler.NotificationsInBlockScreenKey, _phoneSaveHandler.NotificationPressed);
                    break;
                case (int)PhoneScreen.Contacts:
                    SetContactsScreenBackgroundFromAnotherScreen();
                    break;
                case (int)PhoneScreen.Dialog:
                    SetDialogScreenBackgroundFromAnotherScreen(
                        _currentPhone.PhoneContactDictionary[_phoneSaveHandler.DialogContactKey],
                        _phoneSaveHandler.PhoneContentNodeIndex == PhoneSaveHandler.NoneIndex ? null : _onGetNodePort?.Invoke(_phoneSaveHandler.PhoneContentNodeIndex));
                    break;
            }
        }
        else
        {
            TryStartPhoneTime(startHour, startMinute, playModeKey);
            FillUnreadebleContacts();
            SetBlockScreenBackgroundFromNode();
        }

        _topPanelHandler.Init(_phoneTime, playModeKey, butteryPercent);
        _phoneUIGameObject.SetActive(true);
        _handImage.sprite = phone.Hands[handIndex];
        _frameImage.sprite = phone.PhoneFrame;
        return _phoneSiblingIndex;
    }

    private void UpdateNotificationsFromSave()
    {
        UpdateFromSave(_sortedNotifications, _phoneSaveHandler.NotificationsKeys);
    }

    private void UpdateOnlineContactsFromSave()
    {
        UpdateFromSave(_sortedOnlineContacts, _phoneSaveHandler.OnlineContactsKeys);
    }

    private void UpdateFromSave<T>(Dictionary<string, T> sortedInfo, IReadOnlyList<string> keysFromSave) where T : ContactInfo
    {
        bool result = false;
        Action delete = null;
        foreach (var pair in sortedInfo)
        {
            foreach (var key in keysFromSave)
            {
                if (pair.Key == key)
                {
                    result = true;
                    break;
                }
            }
            if (result == false)
            {
                delete += () => { sortedInfo.Remove(pair.Key);};
            }
        }
        delete?.Invoke();
    }

    private void FillUnreadebleContacts()
    {
        _unreadebleContacts.Clear();
        foreach (var contactNodeCase in _sortedPhoneNodeCases)
        {
            if (contactNodeCase.IsReaded == false)
            {
                _unreadebleContacts.Add(contactNodeCase.ContactKey);
            }
        }
    }

    private OnSaveInfo GetInfoToSave()
    {
        _phoneSaveHandler.OnSave -= GetInfoToSave;
        OnSaveInfo info = new OnSaveInfo();
        info.NotificationsKeys = _sortedNotifications.Select(x=>x.Value.ContactKey).ToList();
        info.OnlineContactsKeys = _sortedOnlineContacts.Select(x=>x.Value.ContactKey).ToList();
        int index = 0;
        List<int> indexes = new List<int>(_sortedPhoneNodeCases.Count);
        foreach (var nodeCase in _sortedPhoneNodeCases)
        {
            if (nodeCase.IsReaded)
            {
                indexes.Add(index);
            }
            index++;
        }
        indexes.TrimExcess();
        info.ReadedContactNodeCaseIndexes = indexes;
        info.DialogContactKey = null;
        info.GetPhoneScreenIndex = (int)_currentPhoneScreen;
        info.CurrentPhoneMinute = PhoneTime.CurrentMinute;
        switch (_currentPhoneScreen)
        {
            case PhoneScreen.Block:
                info.NotificationPressed = _blockScreenHandler.NotificationPressed;
                if (_blockScreenHandler.NotificationPressed)
                {
                    info.PhoneContentNodeIndex = _phoneMessagesExtractor.GetCurrentNodeIndex();
                    info.NotificationsInBlockScreenKey = _blockScreenHandler.NotificationsInBlockScreenKey;
                }
                break;
            
            case PhoneScreen.Contacts:
                info.PhoneContentNodeIndex = PhoneSaveHandler.NoneIndex;
                break;
            
            case PhoneScreen.Dialog:
                info.PhoneContentNodeIndex = _phoneMessagesExtractor.GetCurrentNodeIndex();
                info.DialogContactKey = _dialogScreenHandler.GetCurrentContactKey;
                break;
        }

        return info;
    }
    
    private void SetBlockScreenBackgroundFromNode(NodePort portFromSave = null, string notificationsInBlockScreenKey = null, bool notificationPressed = false)
    {
        _currentPhoneScreen = PhoneScreen.Block;
        ShutdownScreens();
        _topPanelHandler.SetColorAndMode(_currentPhone.BlockScreenTopPanelColor, false);
        _blockScreenHandler.Time.color = _currentPhone.BlockScreenTopPanelColor;
        _blockScreenHandler.Date.color = _currentPhone.BlockScreenTopPanelColor;
        _blockScreenHandler.Enable(_phoneTime, _currentPhone, _date,
            _setLocalizationChangeEvent, portFromSave, _playModeKey, notificationPressed, notificationsInBlockScreenKey);
    }

    private void SetDialogScreenBackgroundFromAnotherScreenWithTransitionAnim(PhoneContact contact)
    {
        _currentPhoneScreen = PhoneScreen.Dialog;
        ContactNodeCase contactNodeCase = GetContactNodeCase(contact.NameLocalizationString.Key);
        TransitionAnim(() =>
        {
            ShutdownScreens();
            _topPanelHandler.SetColorAndMode(_currentPhone.DialogScreenTopPanelColor);
            _dialogScreenHandler.Enable(contact, contactNodeCase, contactNodeCase.Port,
                GetOnlineContactInfo(contact.NameLocalizationString.Key), _setLocalizationChangeEvent);
        }).Forget();
    }
    private void SetDialogScreenBackgroundFromAnotherScreen(PhoneContact contact, NodePort nodePort)
    {
        _currentPhoneScreen = PhoneScreen.Dialog;
        ContactNodeCase contactNodeCase = GetContactNodeCase(contact.NameLocalizationString.Key);
        ShutdownScreens();
        _topPanelHandler.SetColorAndMode(_currentPhone.DialogScreenTopPanelColor);
        _dialogScreenHandler.Enable(contact, contactNodeCase, nodePort,
            GetOnlineContactInfo(contact.NameLocalizationString.Key), _setLocalizationChangeEvent);
    }
    private void SetContactsScreenBackgroundFromAnotherScreenWithTransitionAnim()
    {
        _currentPhoneScreen = PhoneScreen.Contacts;
        TransitionAnim(() =>
        {
            ShutdownScreens();
            _topPanelHandler.SetColorAndMode(_currentPhone.ContactsScreenTopPanelColor);
            _contactsScreenHandler.Enable(_currentPhone.PhoneContactDictionary, _setLocalizationChangeEvent, _switchToNextNodeEvent);
        }).Forget();
    }

    private void SetContactsScreenBackgroundFromAnotherScreen()
    {
        _currentPhoneScreen = PhoneScreen.Contacts;
        ShutdownScreens();
        _topPanelHandler.SetColorAndMode(_currentPhone.ContactsScreenTopPanelColor);
        _contactsScreenHandler.Enable(_currentPhone.PhoneContactDictionary, _setLocalizationChangeEvent, _switchToNextNodeEvent);
    }
    
    private ContactNodeCase GetContactNodeCase(string key)
    {
        foreach (var contactNodeCase in _sortedPhoneNodeCases)
        {
            if (contactNodeCase.ContactKey == key)
            {
                return contactNodeCase;
            }
        }
        return null;
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
        if (_sortedOnlineContacts.ContainsKey(nameKey))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private OnlineContactInfo GetOnlineContactInfo(string nameKey)
    {
        if (_sortedOnlineContacts.ContainsKey(nameKey))
        {
            return _sortedOnlineContacts[nameKey];
        }
        else
        {
            return null;
        }
    }
    private void ShutdownScreens()
    {
        _contactsScreenHandler.Shutdown();
        _dialogScreenHandler.Shutdown();
        _blockScreenHandler.Shutdown();
    }

    private void SortingCases(IReadOnlyDictionary<string, PhoneContact> phoneContacts, IReadOnlyList<ContactNodeCase> phoneNodeCases, 
        IReadOnlyList<OnlineContactInfo> onlineContacts, IReadOnlyList<NotificationContactInfo> notifications)
    {
        _sortedPhoneNodeCases.Clear();
        _sortedOnlineContacts.Clear();
        _sortedNotifications.Clear();
        foreach (var contact in phoneContacts)
        {
            Sorting1(phoneNodeCases, _sortedPhoneNodeCases, contact.Key);
            
            Sorting3(onlineContacts, _sortedOnlineContacts, contact.Key);
            Sorting3(notifications, _sortedNotifications, contact.Key);
        }
        bool result = false;
        Sorting2(_sortedOnlineContacts);
        Sorting2(_sortedNotifications);
        void Sorting1<T>(IEnumerable<T> info, ICollection<T> sortedInfo, string key) where T : ContactInfo
        {
            foreach (var contactInfo in info)
            {
                if (contactInfo.ContactKey == key)
                {
                    sortedInfo.Add(contactInfo);
                    break;
                }
            }
        }
        
        void Sorting2<T>(Dictionary<string, T> info) where T : ContactInfo
        {
            Action delete = null;
            foreach (var pair in info)
            {
                result = false;
                for (int j = 0; j < _sortedPhoneNodeCases.Count; j++)
                {
                    if (pair.Key == _sortedPhoneNodeCases[j].ContactKey)
                    {
                        result = true;
                    }
                }
                if (result == false)
                {
                    delete += () => { info.Remove(pair.Key);};
                }
            }
            delete?.Invoke();
        }
        void Sorting3<T>(IEnumerable<T> info, Dictionary<string, T> sortedInfo, string key) where T : ContactInfo
        {
            foreach (var contactInfo in info)
            {
                if (contactInfo.ContactKey == key)
                {
                    sortedInfo.Add(key, contactInfo);
                    break;
                }
            }
        }
    }
    private async UniTask TransitionAnim(Action operation)
    {
        _curtainImage.raycastTarget = true;
        _curtainImage.gameObject.SetActive(true);
        await _curtainImage.DOFade(_end1Value, _durationValue).WithCancellation(_cancellationTokenSource.Token);
        operation?.Invoke();
        await _curtainImage.DOFade(_end2Value, _durationValue).WithCancellation(_cancellationTokenSource.Token);
        _curtainImage.gameObject.SetActive(false);
    }
}