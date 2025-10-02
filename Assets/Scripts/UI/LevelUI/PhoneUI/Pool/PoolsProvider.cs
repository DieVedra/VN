using UnityEngine;

public class PoolsProvider
{
    private const int _contactsCount = 7;
    private const int _messagesCount = 10;
    private readonly ContactView _contactPrefab;
    private readonly MessageView _incomingMessagePrefab;
    private readonly MessageView _outcomingMessagePrefab;
    public PoolBase<ContactView> ContactsPool { get; private set; }
    public PoolBase<MessageView> IncomingMessagePool { get; private set; }
    public PoolBase<MessageView> OutcomingMessagePool { get; private set; }
    private RectTransform _dialogParent, _contactsParent;
    private bool _isInited = false;

    public PoolsProvider(ContactView contactPrefab, MessageView incomingMessagePrefab, MessageView outcomingMessagePrefab)
    {
        _contactPrefab = contactPrefab;
        _incomingMessagePrefab = incomingMessagePrefab;
        _outcomingMessagePrefab = outcomingMessagePrefab;
    }

    public void TryInit(RectTransform dialogParent, RectTransform contactsParent)
    {
        if (_isInited == false)
        {
            _isInited = true;
            _dialogParent = dialogParent;
            _contactsParent = contactsParent;
            ContactsPool = new PoolBase<ContactView>(CreateContact, OnReturn,  _contactsCount);
            IncomingMessagePool = new PoolBase<MessageView>(CreateIncomingMessage, OnReturn, _messagesCount);
            OutcomingMessagePool = new PoolBase<MessageView>(CreateOutcomingMessage, OnReturn, _messagesCount);
        }
    }

    public MessageView GetIncomingMessageView()
    {
        return IncomingMessagePool.Get();
    }

    public MessageView GetOutcomingMessageView()
    {
        return OutcomingMessagePool.Get();
    }

    public void RemoveAllContacts()
    {
        ContactsPool.ReturnAll();
    }

    public void RemoveAllMessages()
    {
        IncomingMessagePool.ReturnAll();
        OutcomingMessagePool.ReturnAll();
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