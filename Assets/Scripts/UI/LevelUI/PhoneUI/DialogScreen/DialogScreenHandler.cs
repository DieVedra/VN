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
    private readonly Button _backArrow;
    private readonly Button _readDialog;
    private readonly GameObject _contactStatus;
    private readonly RectTransform _dialogTransform;
    private CompositeDisposable _compositeDisposableLocalization;
    private CompositeDisposable _compositeDisposable;

    public DialogScreenHandler(DialogScreenView dialogScreenView, TopPanelHandler topPanelHandler, ReactiveCommand switchToContactsScreenCommand)
        :base(dialogScreenView.gameObject, topPanelHandler, dialogScreenView.GradientImage, dialogScreenView.ColorTopPanel)
    {
        _switchToContactsScreenCommand = switchToContactsScreenCommand;
        _contactImage = dialogScreenView.ContactImage;
        _contactStatus = dialogScreenView.ContactStatusGameObject;
        _contactName = dialogScreenView.ContactName;
        _contactStatusText = dialogScreenView.ContactStatus;
        _backArrow = dialogScreenView.BackArrow;
        _dialogTransform = dialogScreenView.DialogTransform;
    }
    public void Enable(PhoneTime phoneTime, PhoneContactDataLocalizable contact,
        SetLocalizationChangeEvent setLocalizationChangeEvent, int butteryPercent, bool playModeKey, bool characterOnlineKey)
    {
        BaseEnable(phoneTime, butteryPercent, playModeKey);
        _contactNameLS = contact.NameContactLocalizationString;
        if (characterOnlineKey)
        {
            _contactStatus.SetActive(true);
        }
        else
        {
            _contactStatus.SetActive(false);
        }
        SubscribeButtons();
        SetTexts();
        _compositeDisposableLocalization = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
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

        SubscribeButtons();
        SetTexts();
        _compositeDisposableLocalization = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
    }
    public override void Disable()
    {
        base.Disable();
        _compositeDisposableLocalization?.Clear();
        _compositeDisposable?.Clear();
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