using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ContactsShower
{
    private readonly Predicate<string> _getOnlineStatus;
    private readonly VerticalLayoutGroup _verticalLayoutGroup;
    private readonly ContentSizeFitter _contentSizeFitter;
    private readonly RectTransform _contactsTransform;
    private PoolBase<ContactView> _contactsPool;
    private CompositeDisposable _compositeDisposable;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private Func<PhoneContact, string> _getFistLetter;
    private Action _activateExitButton;
    private bool _newMessagesNotFound;
    public ContactsShower(VerticalLayoutGroup verticalLayoutGroup, ContentSizeFitter contentSizeFitter, RectTransform contactsTransform, Predicate<string> getOnlineStatus)
    {
        _verticalLayoutGroup = verticalLayoutGroup;
        _contentSizeFitter = contentSizeFitter;
        _contactsTransform = contactsTransform;
        _getOnlineStatus = getOnlineStatus;
    }

    public void Init(IReadOnlyList<PhoneContact> phoneContacts, IReadOnlyList<ContactNodeCase> sortedPhoneNodeCases,
        HashSet<string> nonReadedContacts,
        PoolBase<ContactView> contactsPool, SetLocalizationChangeEvent setLocalizationChangeEvent,
        ReactiveCommand<PhoneContact> switchToDialogScreenCommand,
        Func<PhoneContact, string> getFistLetter, Action activateExitButton)
    {
        _getFistLetter = getFistLetter;
        _activateExitButton = activateExitButton;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _compositeDisposable = new CompositeDisposable();
        _contactsPool = contactsPool;
        _verticalLayoutGroup.enabled = true;
        _contentSizeFitter.enabled = true;
        _newMessagesNotFound = true;
        for (int i = 0; i < phoneContacts.Count; i++)
        {
            
            CreateContact(phoneContacts[i], nonReadedContacts, switchToDialogScreenCommand);
        }
        if (_newMessagesNotFound == true)
        {
            _activateExitButton.Invoke();
        }

        Observable.Timer(TimeSpan.FromSeconds(Time.deltaTime)).Subscribe(_ =>
        {
            _contentSizeFitter.enabled = false;
            _verticalLayoutGroup.enabled = false;
        }).AddTo(_compositeDisposable);
    }

    public void Dispose()
    {
        _compositeDisposable?.Clear();
        _contactsPool?.ReturnAll();
    }
    
    private void CreateContact(PhoneContact phoneContact, HashSet<string> nonReadedContacts, ReactiveCommand<PhoneContact> switchToDialogScreenCommand)
    {
        var view = _contactsPool.Get();
        view.transform.SetParent(_contactsTransform);
        TrySetIcon(phoneContact, view.TextIcon, view.Image);
        view.TextName.text = phoneContact.NameLocalizationString.DefaultText;
        _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            view.TextName.text = phoneContact.NameLocalizationString.DefaultText;
        }, _compositeDisposable);
        SetOnlineStatus(phoneContact, view);
        TryIndicateNewMessages(nonReadedContacts, view.NewMessageIndicatorImage.gameObject, phoneContact.NameLocalizationString.Key);
        view.ContactButton.onClick.AddListener(() =>
        {
            UnsubscribeAllButtons();
            switchToDialogScreenCommand.Execute(phoneContact);
            nonReadedContacts.Remove(phoneContact.NameLocalizationString.Key);
        });
        view.gameObject.SetActive(true);
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

    private void TryIndicateNewMessages(HashSet<string> nonReadedContacts, GameObject newMessageIndicator, string key)
    {
        if (nonReadedContacts.Contains(key))
        {
            newMessageIndicator.SetActive(true);
            _newMessagesNotFound = false;
        }
    }
}