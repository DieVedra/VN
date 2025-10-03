using System;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolsProvider
{
    private const int _contactsCount = 7;
    private const int _messagesCount = 5;
    private ContactView _contactPrefab;
    private MessageView _incomingMessagePrefab;
    private MessageView _outcomingMessagePrefab;

    private PoolBase<ContactView> _contactsPool;
    private PoolBase<MessageView> _incomingMessagePool;
    private PoolBase<MessageView> _outcomingMessagePool;
    public PoolBase<ContactView> ContactsPool => _contactsPool;
    public PoolBase<MessageView> IncomingMessagePool => _incomingMessagePool;
    public PoolBase<MessageView> OutcomingMessagePool => _outcomingMessagePool;
    private RectTransform _dialogParent, _contactsParent;

    public PoolsProvider(ContactView contactPrefab, MessageView incomingMessagePrefab, MessageView outcomingMessagePrefab)
    {
        _contactPrefab = contactPrefab;
        _incomingMessagePrefab = incomingMessagePrefab;
        _outcomingMessagePrefab = outcomingMessagePrefab;
    }
    
#if UNITY_EDITOR
    
    private readonly Action<GameObject> _addView;

    public PoolsProvider(ContactView contactPrefab, MessageView incomingMessagePrefab, MessageView outcomingMessagePrefab,
        Action<GameObject> addView)
    {
        _contactPrefab = contactPrefab;
        _incomingMessagePrefab = incomingMessagePrefab;
        _outcomingMessagePrefab = outcomingMessagePrefab;
        _addView = addView;
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
    }
#endif

    public void Init(RectTransform dialogParent, RectTransform contactsParent)
    {
        _dialogParent = dialogParent;
        _contactsParent = contactsParent;
        _contactsPool = new PoolBase<ContactView>(CreateContact, OnReturn, _contactsCount);
        _incomingMessagePool = new PoolBase<MessageView>(CreateIncomingMessage, OnReturn, _messagesCount);
        _outcomingMessagePool = new PoolBase<MessageView>(CreateOutcomingMessage, OnReturn, _messagesCount);
        
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

    private void OnReturn(MessageView view)
    {
        view.gameObject.SetActive(false);
    }
    private void OnReturn(ContactView view)
    {
        view.gameObject.SetActive(false);
    }
}