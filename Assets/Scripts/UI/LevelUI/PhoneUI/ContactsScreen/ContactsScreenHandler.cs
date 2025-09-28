using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

public class ContactsScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private readonly ReactiveCommand<PhoneContactDataLocalizable> _switchToDialogScreenCommand;
    private LocalizationString _textFindLS = "Люди, группы, сообщения";
    private LocalizationString _textCallsLS = "Звонки";
    private LocalizationString _textExitLS = "Выход";
    private LocalizationString _textContactsLS = "Контакты";
    private LocalizationString _textAddContactLS = "Новый номер";
    private readonly TextMeshProUGUI _textFind;
    private readonly TextMeshProUGUI _textCalls;
    private readonly TextMeshProUGUI _textExit;
    private readonly TextMeshProUGUI _textContacts;
    private readonly TextMeshProUGUI _textAddContact;
    private readonly RectTransform _contactsTransform;
    private CompositeDisposable _compositeDisposableLocalization;
    public ContactsScreenHandler(ContactsScreenView contactsScreenViewBackground, TopPanelHandler topPanelHandler, ReactiveCommand<PhoneContactDataLocalizable> switchToDialogScreenCommand)
        :base(contactsScreenViewBackground.gameObject, topPanelHandler, contactsScreenViewBackground.ImageBackground, contactsScreenViewBackground.ColorTopPanel)
    {
        _switchToDialogScreenCommand = switchToDialogScreenCommand;
        _textFind = contactsScreenViewBackground.TextFind;
        _textCalls = contactsScreenViewBackground.TextCalls;
        _textExit = contactsScreenViewBackground.TextExit;
        _textContacts = contactsScreenViewBackground.TextContacts;
        _textAddContact = contactsScreenViewBackground.TextAddContact;
        _contactsTransform = contactsScreenViewBackground.ContactsTransform;
    }

    public void Enable(PhoneTime phoneTime, IReadOnlyList<PhoneContactDataLocalizable> phoneContactDatasLocalizable,
        SetLocalizationChangeEvent setLocalizationChangeEvent, int butteryPercent, bool playModeKey)
    {
        SubscribeButtons();
        BaseEnable(phoneTime, butteryPercent, playModeKey);
        SetTexts();
        _compositeDisposableLocalization = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
    }
    public void Enable(IReadOnlyList<PhoneContactDataLocalizable> phoneContactDatasLocalizable,
        SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        SubscribeButtons();
        SetTexts();
        _compositeDisposableLocalization = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
    }
    public override void Disable()
    {
        base.Disable();
        _compositeDisposableLocalization?.Clear();
        // _compositeDisposable?.Clear();
    }
    private void SubscribeButtons()
    {
        // _backArrow.onClick.AddListener(() =>
        // {
        //     _backArrow.onClick.RemoveAllListeners();
        //     _switchToContactsScreenCommand.Execute();
        // });
        // _readDialog.onClick.AddListener();
    }
    private void SetTexts()
    {
        _textFind.text = _textFindLS;
        _textCalls.text = _textCallsLS;
        _textExit.text = _textExitLS;
        _textContacts.text = _textContactsLS;
        _textAddContact.text = _textAddContactLS;
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_textFindLS, _textCallsLS, _textExitLS, _textContactsLS, _textAddContactLS};
    }
}