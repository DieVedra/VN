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
    private Func<PhoneContactDataLocalizable, string> _getFistLetter;
    private Action _activateExitButton;
    private bool _newMessagesNotFound;
    public ContactsShower(VerticalLayoutGroup verticalLayoutGroup, ContentSizeFitter contentSizeFitter, RectTransform contactsTransform, Predicate<string> getOnlineStatus)
    {
        _verticalLayoutGroup = verticalLayoutGroup;
        _contentSizeFitter = contentSizeFitter;
        _contactsTransform = contactsTransform;
        _getOnlineStatus = getOnlineStatus;
    }

    public void Init(IReadOnlyList<PhoneContactDataLocalizable> phoneContactDatasLocalizable,
        PoolBase<ContactView> contactsPool, SetLocalizationChangeEvent setLocalizationChangeEvent,
        ReactiveCommand<PhoneContactDataLocalizable> switchToDialogScreenCommand,
        Func<PhoneContactDataLocalizable, string> getFistLetter, Action activateExitButton)
    {
        _getFistLetter = getFistLetter;
        _activateExitButton = activateExitButton;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _compositeDisposable = new CompositeDisposable();
        _contactsPool = contactsPool;
        _verticalLayoutGroup.enabled = true;
        _contentSizeFitter.enabled = true;
        for (int i = 0; i < phoneContactDatasLocalizable.Count; i++)
        {
            CreateContact(phoneContactDatasLocalizable[i], switchToDialogScreenCommand);
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
    private void CreateContact(PhoneContactDataLocalizable contactDataLocalizable, ReactiveCommand<PhoneContactDataLocalizable> switchToDialogScreenCommand)
    {
        var view = _contactsPool.Get();
        view.transform.SetParent(_contactsTransform);
        TrySetIcon(contactDataLocalizable, view.TextIcon, view.Image);
        view.TextName.text = contactDataLocalizable.NikNameContact;
        _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            view.TextName.text = contactDataLocalizable.NikNameContact;
        }, _compositeDisposable);
        SetOnlineStatus(contactDataLocalizable, view);
        TryIndicateNewMessages(contactDataLocalizable.PhoneMessagesLocalization, view.NewMessageIndicatorImage.gameObject);
        view.ContactButton.onClick.AddListener(() =>
        {
            UnsubscribeAllButtons();
            switchToDialogScreenCommand.Execute(contactDataLocalizable);
        });
        view.gameObject.SetActive(true);
    }

    private void SetOnlineStatus(PhoneContactDataLocalizable contactDataLocalizable, ContactView view)
    {
        if (_getOnlineStatus.Invoke(contactDataLocalizable.NameContact.Key))
        {
            view.OnlineStatusImage.gameObject.SetActive(true);
        }
        else
        {
            view.OnlineStatusImage.gameObject.SetActive(false);
        }
    }


    private void TrySetIcon(PhoneContactDataLocalizable contactDataLocalizable, TextMeshProUGUI textComponent, Image image)
    {
        if (contactDataLocalizable.IsEmptyIconKey == true)
        {
            textComponent.text = _getFistLetter.Invoke(contactDataLocalizable);
            image.color = contactDataLocalizable.ColorIcon;
            textComponent.gameObject.SetActive(true);
        }
        else
        {
            textComponent.gameObject.SetActive(false);
            image.color = Color.white;
        }
        image.sprite = contactDataLocalizable.Icon;
    }

    private void UnsubscribeAllButtons()
    {
        for (int i = 0; i < _contactsPool.ActiveContent.Count; i++)
        {
            _contactsPool.ActiveContent[i].ContactButton.onClick.RemoveAllListeners();
        }
    }

    private void TryIndicateNewMessages(IReadOnlyList<PhoneMessageLocalization> phoneMessagesLocalization, GameObject newMessageIndicator)
    {
        int count = phoneMessagesLocalization.Count;
        _newMessagesNotFound = true;
        for (int i = count - 1; i >= 0; i--)
        {
            if (phoneMessagesLocalization[i].IsReaded == false)
            {
                newMessageIndicator.SetActive(true);
                _newMessagesNotFound = false;
                break;
            }
        }
    }
}