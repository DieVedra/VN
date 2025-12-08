using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class BlockScreenHandler : PhoneScreenBaseHandler
{
    private const float _startPosX = 0f;
    private const float _startPosY = 360f;
    private const float _offsetY = 260f;
    private readonly ReactiveCommand<PhoneContact> _switchToDialogScreenCommand;
    private readonly ReactiveCommand _switchToContactsScreenCommand;
    private readonly TextMeshProUGUI _time;
    private readonly TextMeshProUGUI _date;
    private readonly Button _blockScreenButton;
    private readonly Image _imageBackground;
    private LocalizationString _notificationNameLocalizationString;
    private CompositeDisposable _compositeDisposable;
    private PhoneContact _currentContact;
    private LocalizationString _dateLocStr;
    private List<NotificationView> _notificationViews;
    private PoolBase<NotificationView> _notificationViewPool;
    private IReadOnlyDictionary<string, PhoneContact> _phoneContacts;
    private IReadOnlyList<NotificationContactInfo> _notificationsInBlockScreen;
    private IReadOnlyList<OnlineContactInfo> _onlineContacts;
    private IReadOnlyList<ContactNodeCase> _phoneNodeCases;
    private Vector2 _nextPos = new Vector2();
    public BlockScreenHandler(PhoneMessagesExtractor phoneMessagesExtractor, PressDetector pressDetector, BlockScreenView blockScreenViewBackground, TopPanelHandler topPanelHandler, PoolBase<NotificationView> notificationViewPool,
        ReactiveCommand<PhoneContact> switchToDialogScreenCommand, LocalizationString notificationNameLocalizationString, ReactiveCommand switchToContactsScreenCommand)
    :base(blockScreenViewBackground.gameObject, topPanelHandler, blockScreenViewBackground.ImageBackground, blockScreenViewBackground.ColorTopPanel)
    {
        _notificationViewPool = notificationViewPool;
        _switchToDialogScreenCommand = switchToDialogScreenCommand;
        _switchToContactsScreenCommand = switchToContactsScreenCommand;
        _time = blockScreenViewBackground.Time;
        _date = blockScreenViewBackground.Data;
        _blockScreenButton = blockScreenViewBackground.BlockScreenButton;
        _imageBackground = blockScreenViewBackground.ImageBackground;
        _notificationNameLocalizationString = notificationNameLocalizationString;
        _blockScreenButton.enabled = false;
    }
    
    public void Enable(IReadOnlyList<ContactNodeCase> phoneNodeCases, IReadOnlyList<NotificationContactInfo> notificationsInBlockScreen,
        IReadOnlyList<OnlineContactInfo> onlineContacts, PhoneTime phoneTime, Phone phone, LocalizationString date,
        SetLocalizationChangeEvent setLocalizationChangeEvent, bool playModeKey)
    {
        _nextPos.x = _startPosX;
        _nextPos.y = _startPosY;
        _phoneNodeCases = phoneNodeCases;
        _dateLocStr = date;
        _notificationsInBlockScreen = notificationsInBlockScreen;
        _onlineContacts = onlineContacts;
        _phoneContacts = phone.PhoneContactDictionary;
        _imageBackground.sprite = phone.Background;
        _compositeDisposable = new CompositeDisposable();
        setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts, _compositeDisposable);
        Screen.SetActive(true);
        TopPanelHandler.SetColorAndMode(TopPanelColor, false);
        TrySetTime(phoneTime, playModeKey);
        if (TryShowNotifications(setLocalizationChangeEvent) == false)
        {
            TryActivateBlockScreen();
        }
        SetTexts();
    }

    private bool TryShowNotifications(SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        bool result = false;
        if (_notificationsInBlockScreen != null && _notificationsInBlockScreen.Count > 0)
        {
            for (int i = 0; i < _notificationsInBlockScreen.Count; i++)
            {
                for (int j = 0; j < _phoneNodeCases.Count; j++)
                {
                    if (_notificationsInBlockScreen[i].ContactKey == _phoneNodeCases[j].ContactKey)
                    {
                        CreateNotification(_phoneContacts[_phoneNodeCases[j].ContactKey], setLocalizationChangeEvent);
                        result = true;
                    }
                }
            }
        }
        return result;
    }

    private void CreateNotification(PhoneContact contact, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        var notificationView = _notificationViewPool.Get();
        notificationView.RectTransform.anchoredPosition = _nextPos;
        notificationView.Icon.sprite = contact.Icon;
        if (contact.IsEmptyIconKey)
        {
            notificationView.Icon.color = contact.Color;   
            notificationView.TextIcon.gameObject.SetActive(true);
        }
        else
        {
            notificationView.TextIcon.gameObject.SetActive(false);
        }
        notificationView.TextIcon.text = GetFistLetter(contact);
        notificationView.NameText.text = contact.NameLocalizationString;
        notificationView.NotificationText.text = _notificationNameLocalizationString;
        setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            notificationView.TextIcon.text = GetFistLetter(contact);

            notificationView.NameText.text = _notificationNameLocalizationString;
            notificationView.NotificationText.text = _notificationNameLocalizationString;
        }, _compositeDisposable);
        
        notificationView.Button.onClick.AddListener(() =>
        {
            for (int i = 0; i < _notificationViewPool.ActiveContent.Count; i++)
            {
                _notificationViewPool.ActiveContent[i].Button.onClick.RemoveAllListeners();
            }
            _switchToDialogScreenCommand.Execute(contact);
        });
        notificationView.gameObject.SetActive(true);
        _nextPos.y -= _offsetY;
    }
    private void TryActivateBlockScreen()
    {
        _blockScreenButton.enabled = true;
        _blockScreenButton.onClick.AddListener(() =>
        {
            _blockScreenButton.onClick.RemoveAllListeners();
            _switchToContactsScreenCommand.Execute();
        });
    }

    private void TrySetTime(PhoneTime phoneTime, bool playModeKey)
    {
        if (playModeKey == true)
        {
            Observable.EveryUpdate().Subscribe(_ => { _time.text = phoneTime.GetCurrentTime(); })
                .AddTo(_compositeDisposable);
        }
        else
        {
            _time.text = phoneTime.GetCurrentTime();
        }
    }

    public override void Disable()
    {
        _blockScreenButton.enabled = false;
        _blockScreenButton.onClick.RemoveAllListeners();
        base.Disable();
        _compositeDisposable?.Clear();
        _notificationViewPool.ReturnAll();
        
    }

    private void SetTexts()
    {
        _date.text = _dateLocStr;
    }
}