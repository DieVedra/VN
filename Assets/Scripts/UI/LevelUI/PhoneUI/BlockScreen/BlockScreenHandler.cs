using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using XNode;

public class BlockScreenHandler : PhoneScreenBaseHandler
{
    private const float _scaleValueMax = 1.02f;
    private const float _scaleAnimationDelay = 0.5f;
    private const float _duration = 1.5f;
    private const float _startPosX = 0f;
    private const float _startPosY = 360f;
    private const float _offsetY = 260f;
    private readonly ReactiveCommand<PhoneContact> _switchToDialogScreenCommand;
    private readonly ReactiveCommand _switchToContactsScreenCommand;
    private readonly Predicate<string> _getOnlineStatus;
    public readonly TextMeshProUGUI Time;
    public readonly TextMeshProUGUI Date;
    private readonly Button _blockScreenButton;
    private readonly Image _imageBackground;
    private readonly MessagesShower _messagesShower;
    private LocalizationString _notificationTextLocalizationString;
    private CompositeDisposable _compositeDisposable;
    private LocalizationString _dateLocStr;
    private PoolBase<NotificationView> _notificationViewPool;
    private Dictionary<string, NotificationContactInfo> _notificationsInBlockScreen;
    private Vector2 _nextPos = new Vector2();
    public bool NotificationPressed { get; private set; }
    public string NotificationsInBlockScreenKey { get; private set; }
    public BlockScreenHandler(Dictionary<string, NotificationContactInfo> notificationsInBlockScreen, MessagesShower messagesShower, BlockScreenView blockScreenViewBackground, PoolBase<NotificationView> notificationViewPool,
        ReactiveCommand<PhoneContact> switchToDialogScreenCommand, LocalizationString notificationTextLocalizationString, ReactiveCommand switchToContactsScreenCommand, Predicate<string> getOnlineStatus)
    :base(blockScreenViewBackground.gameObject, blockScreenViewBackground.ImageBackground)
    {
        _notificationsInBlockScreen = notificationsInBlockScreen;
        _messagesShower = messagesShower;
        _notificationViewPool = notificationViewPool;
        _switchToDialogScreenCommand = switchToDialogScreenCommand;
        _switchToContactsScreenCommand = switchToContactsScreenCommand;
        _getOnlineStatus = getOnlineStatus;
        Time = blockScreenViewBackground.Time;
        Date = blockScreenViewBackground.Data;
        _blockScreenButton = blockScreenViewBackground.BlockScreenButton;
        _imageBackground = blockScreenViewBackground.ImageBackground;
        _notificationTextLocalizationString = notificationTextLocalizationString;
        _blockScreenButton.enabled = false;
        NotificationPressed = false;
    }
    
    public void Enable(PhoneTime phoneTime, Phone phone, LocalizationString date,
        SetLocalizationChangeEvent setLocalizationChangeEvent, NodePort portFromSave, bool playModeKey,
        bool notificationPressed, string notificationsInBlockScreenKey)
    {
        NotificationsInBlockScreenKey = notificationsInBlockScreenKey;
        _nextPos.x = _startPosX;
        _nextPos.y = _startPosY;
        _dateLocStr = date;
        _imageBackground.sprite = phone.Background;
        _compositeDisposable = new CompositeDisposable();
        NotificationPressed = notificationPressed;
        setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetDateText, _compositeDisposable);
        Screen.SetActive(true);
        TrySetTime(phoneTime, playModeKey);
        CancellationTokenSource = new CancellationTokenSource();

        if (TryShowNotifications(setLocalizationChangeEvent) == false)
        {
            TryActivateBlockScreen();
        }
        else
        {
            if (notificationPressed == false)
            {
                StartScaleAnimation(_notificationViewPool.ActiveContent);
            }
        }

        if (notificationPressed == true)
        {
            var info = _notificationsInBlockScreen[NotificationsInBlockScreenKey];
            _messagesShower.InitFromBlockScreen(portFromSave.node.GetInputPort(GameSeriesHandler.InputPortName).Connection, () =>
            {
                _switchToDialogScreenCommand.Execute(info.Contact);
            } );
        }

        SetDateText();
    }
    
    private bool TryShowNotifications(SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        bool result = false;
        if (_notificationsInBlockScreen != null && _notificationsInBlockScreen.Count > 0)
        {
            foreach (var pair in _notificationsInBlockScreen)
            {
                CreateNotification(pair.Value, setLocalizationChangeEvent);
                result = true;
            }
        }
        return result;
    }

    private void CreateNotification(NotificationContactInfo notificationContactInfo, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        PhoneContact contact = notificationContactInfo.Contact;
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

        if (_getOnlineStatus.Invoke(notificationContactInfo.ContactKey))
        {
            notificationView.OnlineIndicator.gameObject.SetActive(true);
        }
        else
        {
            notificationView.OnlineIndicator.gameObject.SetActive(false);
        }
        SetTexts();
        setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts, _compositeDisposable);
        if (NotificationPressed == false)
        {
            notificationView.Button.onClick.AddListener(() =>
            {
                CancellationTokenSource?.Cancel();
                CancellationTokenSource = null;
                
                foreach (var t in _notificationViewPool.ActiveContent)
                {
                    t.Button.onClick.RemoveAllListeners();
                }
                if (notificationContactInfo.Port.IsConnected)
                {
                    NotificationsInBlockScreenKey = notificationContactInfo.ContactKey;
                    NotificationPressed = true;
                    _messagesShower.InitFromBlockScreen(notificationContactInfo.Port, () =>
                    {
                        _switchToDialogScreenCommand.Execute(contact);
                    } );
                }
                else
                {
                    _switchToDialogScreenCommand.Execute(contact);
                }
            });
        }

        notificationView.gameObject.SetActive(true);
        _nextPos.y -= _offsetY;

        void SetTexts()
        {
            notificationView.TextIcon.text = GetFistLetter(contact);
            notificationView.NameText.text = contact.NameLocalizationString;
            notificationView.NotificationText.text = _notificationTextLocalizationString;
        }
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
            Observable.EveryUpdate().Subscribe(_ => { Time.text = phoneTime.GetCurrentTime(); })
                .AddTo(_compositeDisposable);
        }
        else
        {
            Time.text = phoneTime.GetCurrentTime();
        }
    }

    public override void Shutdown()
    {
        NotificationPressed = false;
        _blockScreenButton.enabled = false;
        _blockScreenButton.onClick.RemoveAllListeners();
        base.Shutdown();
        _compositeDisposable?.Clear();
        _notificationViewPool.ReturnAll();
        _messagesShower.Shutdown();
    }

    private void SetDateText()
    {
        Date.text = _dateLocStr;
    }

    private void StartScaleAnimation<T>(IReadOnlyList<T> activeContent) where T : MonoBehaviour
    {
        if (activeContent.Count > 0)
        {
            float delay = _startPosX;
            foreach (var content in activeContent)
            {
                content.transform.DOScale(_scaleValueMax, _duration).SetDelay(delay)
                    .SetLoops(LoopsCount, LoopType.Yoyo).WithCancellation(CancellationTokenSource.Token);
                delay += _scaleAnimationDelay;
            }
        }
    }
}