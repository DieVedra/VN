using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ContactsScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private readonly ContactsShower _contactsShower;
    private readonly ReactiveCommand<PhoneContactDataLocalizable> _switchToDialogScreenCommand;
    private readonly PoolBase<ContactView> _contactsPool;
    private LocalizationString _textFindLS = "Люди, группы, сообщения";
    private LocalizationString _textCallsLS = "Звонки";
    private LocalizationString _textExitLS = "Выход";
    private LocalizationString _textContactsLS = "Контакты";
    private LocalizationString _textAddContactLS = "Новый номер";
    private readonly TextMeshProUGUI _textFind;
    private readonly TextMeshProUGUI _textCalls;
    private readonly TextMeshProUGUI _textExit;
    private readonly TextMeshProUGUI _textContacts;
    private readonly RectTransform _contactsTransform;
    private readonly Button _buttonExit;
    private SwitchToNextNodeEvent _switchToNextNodeEvent;
    private CompositeDisposable _compositeDisposable;
    public ContactsScreenHandler(ContactsScreenView contactsScreenViewBackground, ContactsShower contactsShower, TopPanelHandler topPanelHandler,
        ReactiveCommand<PhoneContactDataLocalizable> switchToDialogScreenCommand, PoolBase<ContactView> contactsPool)
        :base(contactsScreenViewBackground.gameObject, topPanelHandler, contactsScreenViewBackground.ImageBackground,
            contactsScreenViewBackground.ColorTopPanel)
    {
        _contactsShower = contactsShower;
        _switchToDialogScreenCommand = switchToDialogScreenCommand;
        _contactsPool = contactsPool;
        _textFind = contactsScreenViewBackground.TextFind;
        _textCalls = contactsScreenViewBackground.TextCalls;
        _textExit = contactsScreenViewBackground.TextExit;
        _textContacts = contactsScreenViewBackground.TextContacts;
        _contactsTransform = contactsScreenViewBackground.ContactsTransform;
        _buttonExit = contactsScreenViewBackground.ButtonExit;
    }

    public void Enable(PhoneTime phoneTime, IReadOnlyList<PhoneContactDataLocalizable> phoneContactDatasLocalizable,
        SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent, int butteryPercent, bool playModeKey)
    {
        BaseEnable(phoneTime, butteryPercent, playModeKey);
        Enable(phoneContactDatasLocalizable, setLocalizationChangeEvent, switchToNextNodeEvent);
    }
    public void Enable(IReadOnlyList<PhoneContactDataLocalizable> phoneContactDatasLocalizable,
        SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent)
    {
        _switchToNextNodeEvent = switchToNextNodeEvent;
        SubscribeButtons();
        SetTexts();
        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
        _contactsShower.Init(phoneContactDatasLocalizable, _contactsPool, setLocalizationChangeEvent, _switchToDialogScreenCommand, GetFistLetter);
    }
    public override void Disable()
    {
        base.Disable();
        _contactsShower.Dispose();
        _compositeDisposable?.Clear();
    }
    private void SubscribeButtons()
    {
        _buttonExit.onClick.AddListener(() =>
        {
            _buttonExit.onClick.RemoveAllListeners();
            Disable();
            _switchToNextNodeEvent.Execute();
        });
    }
    private void SetTexts()
    {
        _textFind.text = _textFindLS;
        _textCalls.text = _textCallsLS;
        _textExit.text = _textExitLS;
        _textContacts.text = _textContactsLS;
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_textFindLS, _textCallsLS, _textExitLS, _textContactsLS, _textAddContactLS};
    }
}