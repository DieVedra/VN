using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private const float _alphaMin1 = 0.65f;
    private const float _scaleValueMax = 1.1f;
    private readonly List<OnlineContactInfo> _sortedOnlineContacts;
    private readonly PoolBase<MessageView> _incomingMessagePool;
    private readonly PoolBase<MessageView> _outcomingMessagePool;
    private readonly ReactiveCommand _switchToContactsScreenCommand;
    private readonly Color _backArrowImageColor;
    private LocalizationString _contactNameLS;
    private LocalizationString _contactStatusLS = "Онлайн";
    private readonly Image _contactImage;
    private readonly Image _backArrowImage;
    private readonly TextMeshProUGUI _contactName;
    private readonly TextMeshProUGUI _contactStatusText;
    private readonly TextMeshProUGUI _iconText;
    private readonly Button _backArrow;
    // private readonly Button _readDialog;
    private readonly GameObject _contactStatus;
    private readonly MessagesShower _messagesShower;
    private readonly PressDetector _pressDetector;
    private PhoneContact _currentContact;
    private CompositeDisposable _compositeDisposable;
    private CancellationTokenSource _cancellationTokenSource;
    private IReadOnlyList<ContactNodeCase> _sortedPhoneNodeCases;

    public DialogScreenHandler(List<OnlineContactInfo> sortedOnlineContacts, DialogScreenView dialogScreenView, MessagesShower messagesShower,
        TopPanelHandler topPanelHandler, PoolBase<MessageView> incomingMessagePool, PoolBase<MessageView> outcomingMessagePool,
        ReactiveCommand switchToContactsScreenCommand)
        :base(dialogScreenView.gameObject, topPanelHandler, dialogScreenView.GradientImage, dialogScreenView.ColorTopPanel)
    {
        _sortedOnlineContacts = sortedOnlineContacts;
        _incomingMessagePool = incomingMessagePool;
        _outcomingMessagePool = outcomingMessagePool;
        _switchToContactsScreenCommand = switchToContactsScreenCommand;
        _contactImage = dialogScreenView.ContactImage;
        _contactStatus = dialogScreenView.ContactStatusGameObject;
        _contactName = dialogScreenView.ContactName;
        _contactStatusText = dialogScreenView.ContactOnlineStatus;
        _backArrow = dialogScreenView.BackArrowButton;
        // _readDialog = dialogScreenView.ReadDialogButtonButton;
        _iconText = dialogScreenView.IconText;
        _messagesShower = messagesShower;
        _backArrowImage = dialogScreenView.BackArrowImage;
        _backArrowImageColor = _backArrowImage.color;
    }

    public void Init(IReadOnlyList<ContactNodeCase> sortedPhoneNodeCases)
    {
        _sortedPhoneNodeCases = sortedPhoneNodeCases;
    }
    public void Enable(PhoneContact contact, OnlineContactInfo onlineContactInfo, SetLocalizationChangeEvent setLocalizationChangeEvent, int seriaIndex)
    {
        _currentContact = contact;
        _contactNameLS = contact.NameLocalizationString;
        Screen.SetActive(true);
        TopPanelHandler.SetColorAndMode(TopPanelColor);
        SetContactImage();
        ChangeOnlineIndicator(onlineContactInfo);
        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
        _backArrow.interactable = false;
        _backArrowImage.color = new Color(_backArrowImageColor.r, _backArrowImageColor.g, _backArrowImageColor.b, _alphaMin1);
        SetTexts();
        ContactNodeCase contactNodeCase = GetContactNodeCase(contact.NameLocalizationString.Key);
        _messagesShower.InitFromDialogScreen(contactNodeCase , _incomingMessagePool, _outcomingMessagePool, setLocalizationChangeEvent,
            () =>
            {
                if (onlineContactInfo != null && onlineContactInfo.IsOfflineOnEndKey == true)
                {
                    _sortedOnlineContacts.Remove(onlineContactInfo);
                    ChangeOnlineIndicator(null);
                }

                contactNodeCase.IsReaded = true;
                ActivateBackButton();
            });
    }

    private ContactNodeCase GetContactNodeCase(string key)
    {
        for (int i = 0; i < _sortedPhoneNodeCases.Count; i++)
        {
            if (_sortedPhoneNodeCases[i].ContactKey == key && _sortedPhoneNodeCases[i].IsReaded == false)
            {
                return _sortedPhoneNodeCases[i];
            }
        }

        return null;
    }
    private void ChangeOnlineIndicator(OnlineContactInfo onlineContactInfo)
    {
        if (onlineContactInfo != null)
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
            _contactImage.color = _currentContact.Color;
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
        _messagesShower.Dispose();
        _cancellationTokenSource?.Cancel();
    }

    private void ActivateBackButton()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _backArrow.interactable = true;
        _backArrow.onClick.AddListener(() =>
        {
            _backArrow.onClick.RemoveAllListeners();
            _switchToContactsScreenCommand.Execute();
        });
        _backArrowImage.color = new Color(_backArrowImageColor.r, _backArrowImageColor.g, _backArrowImageColor.b, _alphaMin1);
        _backArrowImage.DOFade(AlphaMax, Duration).SetLoops(LoopsCount, LoopType.Yoyo).WithCancellation(_cancellationTokenSource.Token);
        _backArrowImage.transform.DOScale(_scaleValueMax, Duration).SetLoops(LoopsCount, LoopType.Yoyo).WithCancellation(_cancellationTokenSource.Token);
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