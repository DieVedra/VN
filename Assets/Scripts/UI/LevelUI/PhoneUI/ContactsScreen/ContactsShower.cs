using System;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ContactsShower
{
    private const float _startPosX = 0f;
    private const float _startPosY = 520f;
    private const float _offsetY = 155f;
    private readonly Predicate<string> _getOnlineStatus;
    private PoolBase<ContactView> _contactsPool;
    private Vector2 _pos;
    private CompositeDisposable _compositeDisposable;
    private SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private Func<PhoneContactDataLocalizable, string> _getFistLetter;
    public ContactsShower(Predicate<string> getOnlineStatus)
    {
        _getOnlineStatus = getOnlineStatus;
    }

    public void Init(IReadOnlyList<PhoneContactDataLocalizable> phoneContactDatasLocalizable,
        PoolBase<ContactView> contactsPool, SetLocalizationChangeEvent setLocalizationChangeEvent,
        ReactiveCommand<PhoneContactDataLocalizable> switchToDialogScreenCommand,
        Func<PhoneContactDataLocalizable, string> getFistLetter)
    {
        _pos = new Vector2(_startPosX, _startPosY);
        _getFistLetter = getFistLetter;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _compositeDisposable = new CompositeDisposable();
        _contactsPool = contactsPool;
        for (int i = 0; i < phoneContactDatasLocalizable.Count; i++)
        {
            CreateContact(phoneContactDatasLocalizable[i], switchToDialogScreenCommand, _offsetY * i);
        }
    }

    public void Dispose()
    {
        _compositeDisposable?.Clear();
        _contactsPool?.ReturnAll();
    }
    private void CreateContact(PhoneContactDataLocalizable contactDataLocalizable, ReactiveCommand<PhoneContactDataLocalizable> switchToDialogScreenCommand, float offset)
    {
        var view = _contactsPool.Get();
        _pos.y -= offset;
        view.RectTransform.anchoredPosition = _pos;
        TrySetIcon(contactDataLocalizable, view.TextIcon, view.Image);
        view.TextName.text = contactDataLocalizable.NameContactLocalizationString;
        _setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            view.TextName.text = contactDataLocalizable.NameContactLocalizationString;
        }, _compositeDisposable);
        SetOnlineStatus(contactDataLocalizable, view);
        view.ContactButton.onClick.AddListener(() =>
        {
            UnsubscribeAllButtons();
            switchToDialogScreenCommand.Execute(contactDataLocalizable);
        });
        view.gameObject.SetActive(true);
    }

    private void SetOnlineStatus(PhoneContactDataLocalizable contactDataLocalizable, ContactView view)
    {
        if (_getOnlineStatus.Invoke(contactDataLocalizable.NameContactLocalizationString.Key))
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
}