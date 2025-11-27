using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BlockScreenHandler : PhoneScreenBaseHandler/*, ILocalizable*/
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
    private IReadOnlyList<ContactInfoToGame> _notificationContacts;
    private IReadOnlyDictionary<string, ContactInfoToGame> _contactsInfoToGame;
    private IReadOnlyList<PhoneContact> _phoneContactDatasLocalizable;
    private Vector2 _nextPos = new Vector2();
    public BlockScreenHandler(BlockScreenView blockScreenViewBackground, TopPanelHandler topPanelHandler, PoolBase<NotificationView> notificationViewPool,
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
    
    public void Enable(IReadOnlyDictionary<string, ContactInfoToGame> contactsInfoToGame, PhoneTime phoneTime, Phone phone, LocalizationString date,
        SetLocalizationChangeEvent setLocalizationChangeEvent,
        int startScreenCharacterIndex, bool playModeKey)
    {
        _nextPos.x = _startPosX;
        _nextPos.y = _startPosY;
        _contactsInfoToGame = contactsInfoToGame;
        _dateLocStr = date;
        // _currentContact = phone.PhoneDataLocalizable.PhoneContactDatasLocalizable[startScreenCharacterIndex];
        // _imageBackground.sprite = phone.PhoneDataLocalizable.Background;
        // _phoneContactDatasLocalizable = phone.PhoneDataLocalizable.PhoneContactDatasLocalizable;
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
        if (_contactsInfoToGame != null)
        {
            for (int i = 0; i < _phoneContactDatasLocalizable.Count; i++)
            {
                // if (_contactsInfoToGame.TryGetValue(_phoneContactDatasLocalizable[i].NameContact.Key, out ContactInfoToGame contactInfoToGame))
                // {
                //     if (contactInfoToGame.KeyNotification == true)
                //     {
                //         CreateNotification(_phoneContactDatasLocalizable[i], setLocalizationChangeEvent);
                //         result = true;
                //     }
                // }
            }
        }
        return result;
    }

    private void CreateNotification(PhoneContact dataLocalizable, SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        var notificationView = _notificationViewPool.Get();
        notificationView.RectTransform.anchoredPosition = _nextPos;
        notificationView.Icon.sprite = dataLocalizable.Icon;
        if (dataLocalizable.IsEmptyIconKey)
        {
            // notificationView.Icon.color = dataLocalizable.ColorIcon;   
            notificationView.TextIcon.gameObject.SetActive(true);
        }
        else
        {
            notificationView.TextIcon.gameObject.SetActive(false);
        }
        notificationView.TextIcon.text = GetFistLetter(dataLocalizable);
        // notificationView.NameText.text = dataLocalizable.NikNameContact;
        notificationView.NotificationText.text = _notificationNameLocalizationString;
        setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            notificationView.TextIcon.text = GetFistLetter(dataLocalizable);

            // notificationView.NameText.text = dataLocalizable.NikNameContact;
            notificationView.NotificationText.text = _notificationNameLocalizationString;
        }, _compositeDisposable);
        
        notificationView.Button.onClick.AddListener(() =>
        {
            for (int i = 0; i < _notificationViewPool.ActiveContent.Count; i++)
            {
                _notificationViewPool.ActiveContent[i].Button.onClick.RemoveAllListeners();
            }
            _switchToDialogScreenCommand.Execute(dataLocalizable);
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
    // public IReadOnlyList<LocalizationString> GetLocalizableContent()
    // {
    //     
    //     return new[] {_notificationNameLocalizationString};
    // }
}