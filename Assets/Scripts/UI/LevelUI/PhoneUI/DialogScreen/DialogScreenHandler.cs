using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private readonly PoolBase<MessageView> _incomingMessagePool;
    private readonly PoolBase<MessageView> _outcomingMessagePool;
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
    private readonly MessagesShower _messagesShower;
    private PhoneContactDataLocalizable _currentContact;
    private SwitchToNextNodeEvent _switchToNextNodeEvent;
    private CompositeDisposable _compositeDisposable;

    public DialogScreenHandler(DialogScreenView dialogScreenView, MessagesShower messagesShower, TopPanelHandler topPanelHandler,
        PoolBase<MessageView> incomingMessagePool, PoolBase<MessageView> outcomingMessagePool, ReactiveCommand switchToContactsScreenCommand)
        :base(dialogScreenView.gameObject, topPanelHandler, dialogScreenView.GradientImage, dialogScreenView.ColorTopPanel)
    {
        _incomingMessagePool = incomingMessagePool;
        _outcomingMessagePool = outcomingMessagePool;
        _switchToContactsScreenCommand = switchToContactsScreenCommand;
        _contactImage = dialogScreenView.ContactImage;
        _contactStatus = dialogScreenView.ContactStatusGameObject;
        _contactName = dialogScreenView.ContactName;
        _contactStatusText = dialogScreenView.ContactStatus;
        _backArrow = dialogScreenView.BackArrowButton;
        _readDialog = dialogScreenView.ReadDialogButtonButton;
        _iconText = dialogScreenView.IconText;
        _messagesShower = messagesShower;
    }
    public void Enable(PhoneContactDataLocalizable contact, SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent,
        Action<string, bool> setOnlineStatus, bool characterOnlineKey)
    {
        _switchToNextNodeEvent = switchToNextNodeEvent;
        _currentContact = contact;
        _contactNameLS = contact.NikNameContact;
        Screen.SetActive(true);
        TopPanelHandler.SetColorAndMode(TopPanelColor);
        SetContactImage();
        SetOnlineKey(characterOnlineKey);
        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
        SubscribeButtons();
        SetTexts();
        _messagesShower.Init(_currentContact.PhoneMessagesLocalization, _incomingMessagePool, _outcomingMessagePool, setLocalizationChangeEvent,
            () =>
            {
                setOnlineStatus.Invoke(contact.NameContact.Key, false);
                SetOnlineKey(false);
            });
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

    public override void Disable()
    {
        base.Disable();
        _compositeDisposable?.Clear();
        _readDialog.onClick.RemoveAllListeners();
        _messagesShower.Dispose();
    }

    private void SubscribeButtons()
    {
        _readDialog.interactable = true;
        _backArrow.interactable = false;
        _readDialog.onClick.AddListener(() =>
        {
            if (_messagesShower.ShowNext() == false)
            {
                _readDialog.interactable = false;
                _readDialog.onClick.RemoveAllListeners();
                _backArrow.interactable = true;
                _backArrow.onClick.AddListener(() =>
                    {
                        _backArrow.onClick.RemoveAllListeners();
                        _switchToContactsScreenCommand.Execute();
                    });
            }
        });
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