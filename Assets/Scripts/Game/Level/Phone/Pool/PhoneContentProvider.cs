using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class PhoneContentProvider
{
    private const int _contactsCount = 7;
    private const int _messagesCount = 5;
    private const int _notificationsCount = 1;
    private ContactView _contactPrefab;
    private MessageView _incomingMessagePrefab;
    private MessageView _outcomingMessagePrefab;
    private NotificationView _notificationViewPrefab;
    private PoolBase<ContactView> _contactsPool;
    private PoolBase<MessageView> _incomingMessagePool;
    private PoolBase<MessageView> _outcomingMessagePool;
    private PoolBase<NotificationView> _notificationViewPool;
    private RectTransform _dialogParent;
    private Transform _notificationParent, _contactsParent;
    private Vector3 _messageScale;
    private Vector3 _notificationScale;
    private Vector3 _contactScale;

    public PoolBase<ContactView> ContactsPool => _contactsPool;
    public PoolBase<MessageView> IncomingMessagePool => _incomingMessagePool;
    public PoolBase<MessageView> OutcomingMessagePool => _outcomingMessagePool;
    public PoolBase<NotificationView> NotificationViewPool => _notificationViewPool;

    public PhoneContentProvider(ContactView contactPrefab, MessageView incomingMessagePrefab,
        MessageView outcomingMessagePrefab, NotificationView notificationViewPrefab)
    {
        _contactPrefab = contactPrefab;
        _incomingMessagePrefab = incomingMessagePrefab;
        _outcomingMessagePrefab = outcomingMessagePrefab;
        _notificationViewPrefab = notificationViewPrefab;
    }
    
#if UNITY_EDITOR
    
    private readonly Action<GameObject> _addView;

    public PhoneContentProvider(ContactView contactPrefab, MessageView incomingMessagePrefab, MessageView outcomingMessagePrefab, NotificationView notificationViewPrefab,
        Action<GameObject> addView)
    {
        _contactPrefab = contactPrefab;
        _incomingMessagePrefab = incomingMessagePrefab;
        _outcomingMessagePrefab = outcomingMessagePrefab;
        _notificationViewPrefab = notificationViewPrefab;
        _addView = addView;
        _notificationScale = notificationViewPrefab.transform.localScale;
        _contactScale = _contactPrefab.transform.localScale;
        _messageScale = _outcomingMessagePrefab.transform.localScale;
    }

    public void AddPoolsViews()
    {
        foreach (var view in _contactsPool.Pool)
        {
            _addView.Invoke(view.gameObject);
        }
        foreach (var view in _incomingMessagePool.Pool)
        {
            _addView.Invoke(view.gameObject);
        }
        foreach (var view in _outcomingMessagePool.Pool)
        {
            _addView.Invoke(view.gameObject);
        }
        foreach (var view in _notificationViewPool.Pool)
        {
            _addView.Invoke(view.gameObject);
        }
    }
#endif

    public void Init(RectTransform dialogParent, Transform contactsParent, Transform notificationParent)
    {
        _dialogParent = dialogParent;
        _contactsParent = contactsParent;
        _notificationParent = notificationParent;
        _contactsPool = new PoolBase<ContactView>(CreateContact, null, OnReturn, _contactsCount);
        _incomingMessagePool = new PoolBase<MessageView>(CreateIncomingMessage, null, OnReturn, _messagesCount);
        _outcomingMessagePool = new PoolBase<MessageView>(CreateOutcomingMessage, null, OnReturn, _messagesCount);
        _notificationViewPool = new PoolBase<NotificationView>(CreateNotification, null, OnReturn, _notificationsCount);
#if UNITY_EDITOR

        if (Application.isPlaying == false)
        {
            AddPoolsViews();
        }
#endif

    }

    private ContactView CreateContact()
    {
        return Object.Instantiate(_contactPrefab, _contactsParent);
    }

    private MessageView CreateIncomingMessage()
    {
        return Object.Instantiate(_incomingMessagePrefab, _dialogParent);
    }

    private MessageView CreateOutcomingMessage()
    {
        return Object.Instantiate(_outcomingMessagePrefab, _dialogParent);
    }
    private NotificationView CreateNotification()
    {
        return Object.Instantiate(_notificationViewPrefab, _notificationParent);
    }
    private void OnReturn(MessageView view)
    {
        view.gameObject.SetActive(false);
        view.transform.localScale = _messageScale;
    }
    private void OnReturn(ContactView view)
    {
        view.NewMessageIndicatorImage.gameObject.SetActive(false);
        view.OnlineStatusImage.gameObject.SetActive(false);
        view.gameObject.SetActive(false);
        Transform transform;
        (transform = view.transform).SetParent(_contactsParent);
        transform.localScale = _contactScale;
    }
    private void OnReturn(NotificationView view)
    {
        view.Button.onClick.RemoveAllListeners();
        view.TextIcon.gameObject.SetActive(false);
        view.gameObject.SetActive(false);
        view.transform.localScale = _notificationScale;
    }
}