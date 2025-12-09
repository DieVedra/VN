using System;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ContactsShower
{
    private const float _startPosY = -72f;
    private const float _startPosX = 0f;
    private const float _offset = 20f;
    private readonly Predicate<string> _getOnlineStatus;
    private readonly RectTransform _contactsTransform;
    private PoolBase<ContactView> _contactsPool;
    private CompositeDisposable _compositeDisposable;
    private CancellationTokenSource _cancellationTokenSource;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private Func<PhoneContact, string> _getFistLetter;
    private Action _activateExitButton;
    private Vector2 _bufer;
    private bool _newMessagesNotFound;
    public ContactsShower(RectTransform contactsTransform, Predicate<string> getOnlineStatus)
    {
        _contactsTransform = contactsTransform;
        _getOnlineStatus = getOnlineStatus;
    }

    public void Init(IReadOnlyDictionary<string, PhoneContact> phoneContacts,
        HashSet<string> unreadebleContacts,
        PoolBase<ContactView> contactsPool, SetLocalizationChangeEvent setLocalizationChangeEvent,
        ReactiveCommand<PhoneContact> switchToDialogScreenCommand,
        Func<PhoneContact, string> getFistLetter, Action activateExitButton)
    {
        _getFistLetter = getFistLetter;
        _activateExitButton = activateExitButton;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _compositeDisposable = new CompositeDisposable();
        _contactsPool = contactsPool;
        _newMessagesNotFound = true;
        _cancellationTokenSource = new CancellationTokenSource();

        _contactsTransform.sizeDelta = Vector2.zero;
        _bufer = Vector2.zero;
        foreach (var contact in phoneContacts)
        {
            CreateContact(contact.Value, unreadebleContacts, switchToDialogScreenCommand);
        }
        
        // _bufer.x = _contactsTransform.sizeDelta.x;
        // _contactsTransform.sizeDelta = _bufer;
        //
        // _bufer.x = _startPosX;
        // RectTransform rectTransform;
        // for (int i = 0; i < _contactsPool.ActiveContent.Count; i++)
        // {
        //     rectTransform = _contactsPool.ActiveContent[i].RectTransform;
        //     _bufer.y = rectTransform.sizeDelta.y + _offset;
        //     rectTransform.anchoredPosition = _bufer;
        // }
        if (_newMessagesNotFound == true)
        {
            _activateExitButton.Invoke();
        }
    }

    public void Dispose()
    {
        _compositeDisposable?.Clear();
        _contactsPool?.ReturnAll();
    }
    
    private void CreateContact(PhoneContact phoneContact, HashSet<string> unreadebleContacts, ReactiveCommand<PhoneContact> switchToDialogScreenCommand)
    {
        ContactView view = _contactsPool.Get();
        view.RectTransform.SetParent(_contactsTransform);

        ExpansionSizeContactsFrame(view);


        TrySetIcon(phoneContact, view.TextIcon, view.Image);
        view.TextName.text = phoneContact.NameLocalizationString.DefaultText;
        _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            view.TextName.text = phoneContact.NameLocalizationString.DefaultText;
        }, _compositeDisposable);
        SetOnlineStatus(phoneContact, view);
        TryIndicateNewMessages(unreadebleContacts, view, phoneContact.NameLocalizationString.Key);
        view.ContactButton.onClick.AddListener(() =>
        {
            UnsubscribeAllButtons();
            switchToDialogScreenCommand.Execute(phoneContact);
            unreadebleContacts.Remove(phoneContact.NameLocalizationString.Key);
            _cancellationTokenSource?.Cancel();
        });
        view.gameObject.SetActive(true);
    }

    private void ExpansionSizeContactsFrame(ContactView view)
    {
        _bufer = _contactsTransform.sizeDelta;
        _bufer.y += view.RectTransform.sizeDelta.y + _offset;
        _contactsTransform.sizeDelta = _bufer;
        _bufer.y = -_bufer.y;
        view.RectTransform.anchoredPosition = _bufer;
    }

    private void SetOnlineStatus(PhoneContact phoneContact, ContactView view)
    {
        if (_getOnlineStatus.Invoke(phoneContact.NameLocalizationString.Key))
        {
            view.OnlineStatusImage.gameObject.SetActive(true);
        }
        else
        {
            view.OnlineStatusImage.gameObject.SetActive(false);
        }
    }


    private void TrySetIcon(PhoneContact phoneContact, TextMeshProUGUI textComponent, Image image)
    {
        if (phoneContact.IsEmptyIconKey == true)
        {
            textComponent.text = _getFistLetter.Invoke(phoneContact);
            image.color = phoneContact.Color;
            textComponent.gameObject.SetActive(true);
        }
        else
        {
            textComponent.gameObject.SetActive(false);
            image.color = Color.white;
        }
        image.sprite = phoneContact.Icon;
    }

    private void UnsubscribeAllButtons()
    {
        for (int i = 0; i < _contactsPool.ActiveContent.Count; i++)
        {
            _contactsPool.ActiveContent[i].ContactButton.onClick.RemoveAllListeners();
        }
    }

    private void TryIndicateNewMessages(HashSet<string> unreadebleContacts, ContactView view, string key)
    {
        if (unreadebleContacts.Contains(key))
        {
            view.NewMessageIndicatorImage.gameObject.SetActive(true);
            // view.transform.DOScale(1.3f, PhoneScreenBaseHandler.Duration).SetLoops(PhoneScreenBaseHandler.LoopsCount, LoopType.Yoyo).WithCancellation(_cancellationTokenSource.Token);
            _newMessagesNotFound = false;
        }
    }
}