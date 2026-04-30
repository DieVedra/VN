

public interface IPhoneContentProviderToDialogScreen
{
    public MessageView ContactBlockNotificationPrefab { get; }
    public PoolBase<MessageView> IncomingMessagePool { get; }
    public PoolBase<MessageView> OutcomingMessagePool { get; }
}