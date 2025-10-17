
public class PhoneMessageLocalization
{
    public readonly LocalizationString TextMessage;
    private readonly PhoneMessageType _phoneMessageType;
    public bool IsReaded;

    public PhoneMessageType MessageType => _phoneMessageType;

    public PhoneMessageLocalization(string text, PhoneMessageType phoneMessageType, bool isReaded = false)
    {
        TextMessage = new LocalizationString(text);
        _phoneMessageType = phoneMessageType;
        IsReaded = isReaded;
    }
}