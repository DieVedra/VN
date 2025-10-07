using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BlockScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private readonly NotificationView _notificationViewPrefab;
    private readonly ReactiveCommand<PhoneContactDataLocalizable> _switchToDialogScreenCommand;
    private readonly ReactiveCommand _switchToContactsScreenCommand;
    private readonly TextMeshProUGUI _time;
    private readonly TextMeshProUGUI _date;
    private readonly Image _notificationContactIcon;
    private readonly Image _imageBackground;
    private readonly TextMeshProUGUI _contactName;
    private readonly TextMeshProUGUI _notificationText;
    private readonly TextMeshProUGUI _iconText;
    private LocalizationString _notificationNameLocalizationString = "Получено новое сообщение!";
    private CompositeDisposable _compositeDisposable;
    private PhoneContactDataLocalizable _currentContact;
    private LocalizationString _dateLocStr;
    private List<NotificationView> _notificationViews;
    public BlockScreenHandler(BlockScreenView blockScreenViewBackground, TopPanelHandler topPanelHandler, NotificationView notificationViewPrefab,
        ReactiveCommand<PhoneContactDataLocalizable> switchToDialogScreenCommand, ReactiveCommand switchToContactsScreenCommand)
    :base(blockScreenViewBackground.gameObject, topPanelHandler, blockScreenViewBackground.ImageBackground, blockScreenViewBackground.ColorTopPanel)
    {
        _notificationViewPrefab = notificationViewPrefab;
        _switchToDialogScreenCommand = switchToDialogScreenCommand;
        _switchToContactsScreenCommand = switchToContactsScreenCommand;
        _time = blockScreenViewBackground.Time;
        _date = blockScreenViewBackground.Data;
        _notificationContactIcon = blockScreenViewBackground.NotificationContactIcon;
        _imageBackground = blockScreenViewBackground.ImageBackground;
        _contactName = blockScreenViewBackground.ContactName;
        _notificationText = blockScreenViewBackground.NotificationText;
        _iconText = blockScreenViewBackground.IconText;
    }
    
    public void Enable(PhoneTime phoneTime, Phone phone, LocalizationString date, SetLocalizationChangeEvent setLocalizationChangeEvent,
        int startScreenCharacterIndex, bool playModeKey, bool blockScreenNotificationKey)
    {
        _dateLocStr = date;
        _currentContact = phone.PhoneDataLocalizable.PhoneContactDatasLocalizable[startScreenCharacterIndex];
        _imageBackground.sprite = phone.PhoneDataLocalizable.Background;
        Screen.SetActive(true);
        TopPanelHandler.SetColorAndMode(TopPanelColor, false);
        SetTexts();
        TrySetTime(phoneTime, setLocalizationChangeEvent, playModeKey);
        
        TryActivateBlockScreen(blockScreenNotificationKey);
    }

    private void TryActivateBlockScreen(bool blockScreenNotificationKey)
    {
        if (blockScreenNotificationKey == false)
        {
            _notificationContactIcon.transform.parent.gameObject.SetActive(false);
            SubscribeButton(() => { _switchToContactsScreenCommand.Execute();});
        }
        else
        {
            TrySetIcon();
            _notificationContactIcon.transform.parent.gameObject.SetActive(true);
            SubscribeButton(() => { _switchToDialogScreenCommand.Execute(_currentContact);});
        }
    }

    private void TrySetIcon()
    {
        if (_currentContact.IsEmptyIconKey == true)
        {
            _iconText.text = GetFistLetter(_currentContact);
            _notificationContactIcon.color = _currentContact.ColorIcon;
            _iconText.gameObject.SetActive(true);
        }
        else
        {
            _iconText.gameObject.SetActive(false);
            _notificationContactIcon.color = Color.white;
        }

        _notificationContactIcon.sprite = _currentContact.Icon;
        _notificationContactIcon.gameObject.SetActive(true);
    }

    private void TrySetTime(PhoneTime phoneTime, SetLocalizationChangeEvent setLocalizationChangeEvent, bool playModeKey)
    {
        if (playModeKey == true)
        {
            _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
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
        base.Disable();
        _compositeDisposable?.Clear();
    }

    private void SetTexts()
    {
        _date.text = _dateLocStr;
        _notificationText.text = _notificationNameLocalizationString;
        if (_currentContact != null)
        {
            _contactName.text = _currentContact.NameContactLocalizationString;
        }
    }

    private void SubscribeButton(Action operation)
    {
        // _buttonInteraction.onClick.AddListener(() =>
        // {
        //     _buttonInteraction.onClick.RemoveAllListeners();
        //     operation.Invoke();
        // });
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_notificationNameLocalizationString};
    }
}