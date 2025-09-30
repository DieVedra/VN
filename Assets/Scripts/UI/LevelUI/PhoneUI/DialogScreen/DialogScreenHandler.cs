using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private readonly ReactiveCommand _switchToContactsScreenCommand;
    private LocalizationString _contactNameLS;
    private LocalizationString _contactStatusLS = "Онлайн";
    private readonly Image _contactImage;
    private readonly TextMeshProUGUI _contactName;
    private readonly TextMeshProUGUI _contactStatusText;
    private readonly TextMeshProUGUI _iconText;
    private readonly Button _backArrow;
    private readonly Button _readDialog;
    private readonly GameObject _contactStatus;
    private readonly RectTransform _dialogTransform;
    private PhoneContactDataLocalizable _currentContact;
    private CompositeDisposable _compositeDisposable;

    public DialogScreenHandler(DialogScreenView dialogScreenView, TopPanelHandler topPanelHandler, ReactiveCommand switchToContactsScreenCommand)
        :base(dialogScreenView.gameObject, topPanelHandler, dialogScreenView.GradientImage, dialogScreenView.ColorTopPanel)
    {
        _switchToContactsScreenCommand = switchToContactsScreenCommand;
        _contactImage = dialogScreenView.ContactImage;
        _contactStatus = dialogScreenView.ContactStatusGameObject;
        _contactName = dialogScreenView.ContactName;
        _contactStatusText = dialogScreenView.ContactStatus;
        _backArrow = dialogScreenView.BackArrowButton;
        _readDialog = dialogScreenView.ReadDialogButtonButton;
        _dialogTransform = dialogScreenView.DialogTransform;
        _iconText = dialogScreenView.IconText;
    }
    public void Enable(PhoneTime phoneTime, PhoneContactDataLocalizable contact,
        SetLocalizationChangeEvent setLocalizationChangeEvent, int butteryPercent, bool playModeKey, bool characterOnlineKey)
    {
        _currentContact = contact;
        _contactNameLS = contact.NameContactLocalizationString;
        BaseEnable(phoneTime, butteryPercent, playModeKey);
        SetContactImage();
        SetOnlineKey(characterOnlineKey);

        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
        SubscribeButtons();
        SetTexts();
    }

    private void SetOnlineKey(bool characterOnlineKey)
    {
        if (characterOnlineKey)
        {
            _contactStatus.SetActive(true);
        }
        else
        {
            _contactStatus.SetActive(false);
        }
    }

    private void SetContactImage()
    {
        _contactImage.sprite = _currentContact.Icon;
        if (_currentContact.IsEmptyIconKey == true)
        {
            _contactImage.color = _currentContact.ColorIcon;
            _iconText.text = GetFistLetter(_currentContact);
            _iconText.gameObject.SetActive(true);
        }
        else
        {
            _contactImage.color = Color.white;
            _iconText.gameObject.SetActive(false);
        }
    }

    public void Enable(PhoneContactDataLocalizable contact, SetLocalizationChangeEvent setLocalizationChangeEvent, bool characterOnlineKey)
    {
        _contactNameLS = contact.NameContactLocalizationString;
        if (characterOnlineKey)
        {
            _contactStatus.SetActive(true);
        }
        else
        {
            _contactStatus.SetActive(false);
        }

        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
        SubscribeButtons();
        SetTexts();
    }
    public override void Disable()
    {
        base.Disable();
        _compositeDisposable?.Clear();
        _readDialog.onClick.RemoveAllListeners();
    }

    private void SubscribeButtons()
    {
        _backArrow.onClick.AddListener(() =>
        {
            _backArrow.onClick.RemoveAllListeners();
            _switchToContactsScreenCommand.Execute();
        });
        // _readDialog.onClick.AddListener();
    }
    private void SetTexts()
    {
        _contactName.text = _contactNameLS;
        _contactStatusText.text = _contactStatusLS;
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_contactStatusLS};
    }
}