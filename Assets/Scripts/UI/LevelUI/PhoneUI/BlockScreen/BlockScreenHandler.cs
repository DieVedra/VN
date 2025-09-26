
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class BlockScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private LocalizationString _notificationNameLocalizationString = "Получено новое соощение!";
    private TextMeshProUGUI _time;
    private TextMeshProUGUI _date;
    private Image _notificationContactIcon;
    private TextMeshProUGUI _contactName;
    private TextMeshProUGUI _notificationText;
    private CompositeDisposable _compositeDisposable;
    private CompositeDisposable _compositeDisposableLocalization;
    private PhoneContactDataLocalizable _currentContact;
    private LocalizationString _dateLocStr;
    public BlockScreenHandler(BlockScreenView blockScreenViewBackground, TopPanelHandler topPanelHandler)
    :base(blockScreenViewBackground.gameObject, topPanelHandler, blockScreenViewBackground.ImageBackground, blockScreenViewBackground.ColorTopPanel)
    {
        _time = blockScreenViewBackground.Time;
        _date = blockScreenViewBackground.Data;
        _notificationContactIcon = blockScreenViewBackground.NotificationContactIcon;
        _contactName = blockScreenViewBackground.ContactName;
        _notificationText = blockScreenViewBackground.NotificationText;
    }
    
    public void Enable(PhoneTime phoneTime, PhoneContactDataLocalizable contact, LocalizationString date, SetLocalizationChangeEvent setLocalizationChangeEvent, int butteryPercent, bool playModeKey)
    {
        _currentContact = contact;
        _dateLocStr = date;
        BaseEnable(phoneTime, butteryPercent, playModeKey);
        Screen.SetActive(true);
        TopPanelHandler.Init(TopPanelColor, phoneTime, playModeKey, butteryPercent, false);
        SetTexts();
        if (playModeKey == true)
        {
            _compositeDisposableLocalization = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
            _compositeDisposable = new CompositeDisposable();
            Observable.EveryUpdate().Subscribe(_ =>
            {
                _time.text = phoneTime.GetCurrentTime();
            }).AddTo(_compositeDisposable);
        }
        else
        {
            _time.text = phoneTime.GetCurrentTime();
        }

        if (_currentContact.Icon == null)
        {
            _notificationContactIcon.gameObject.SetActive(false);
        }
        else
        {
            _notificationContactIcon.gameObject.SetActive(true);
            _notificationContactIcon.sprite = _currentContact.Icon;
        }
    }

    public override void Disable()
    {
        base.Disable();
        _compositeDisposableLocalization?.Clear();
        _compositeDisposable?.Clear();
    }

    private void SetTexts()
    {
        _date.text = _dateLocStr;
        _notificationText.text = _notificationNameLocalizationString;
        _contactName.text = _currentContact.NameContactLocalizationString;
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_notificationNameLocalizationString};
    }
}