using UnityEngine;

public class PoolsProvider
{
    private const int _contactsCount = 7;
    private const int _messagesCount = 10;
    private readonly ContactView _contactPrefab;
    private readonly MessageView _incomingMessagePrefab;
    private readonly MessageView _outcomingMessagePrefab;
    private PoolBase<ContactView> _contactsPool;
    private PoolBase<MessageView> _incomingMessagePool;
    private PoolBase<MessageView> _outcomingMessagePool;

    public PoolsProvider(ContactView contactPrefab, MessageView incomingMessagePrefab, MessageView outcomingMessagePrefab)
    {
        _contactPrefab = contactPrefab;
        _incomingMessagePrefab = incomingMessagePrefab;
        _outcomingMessagePrefab = outcomingMessagePrefab;
    }

    public void Init()
    {
        _contactsPool = new PoolBase<ContactView>(CreateContact, _contactsCount);
        _incomingMessagePool = new PoolBase<MessageView>(CreateIncomingMessage, _messagesCount);
        _outcomingMessagePool = new PoolBase<MessageView>(CreateOutcomingMessage, _messagesCount);
    }

    public ContactView GetContactView()
    {
        return _contactsPool.Get();
    }
    public MessageView GetIncomingMessageView()
    {
        return _incomingMessagePool.Get();
    }
    public MessageView GetOutcomingMessageView()
    {
        return _outcomingMessagePool.Get();
    }
    public void RemoveAllContacts()
    {
        _contactsPool.ReturnAll();
    }

    public void RemoveAllMessages()
    {
        _incomingMessagePool.ReturnAll();
        _outcomingMessagePool.ReturnAll();
    }
    private ContactView CreateContact()
    {
        return Object.Instantiate(_contactPrefab);
    }
    private MessageView CreateIncomingMessage()
    {
        return Object.Instantiate(_incomingMessagePrefab);
    }
    private MessageView CreateOutcomingMessage()
    {
        return Object.Instantiate(_outcomingMessagePrefab);
    }
}