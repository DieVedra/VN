

public interface IPhoneContentProviderToDialogScreen
{
    public PoolBase<MessageView> BlockMessagePool { get; }
    public PoolBase<MessageView> IncomingMessagePool { get; }
    public PoolBase<MessageView> OutcomingMessagePool { get; }
}