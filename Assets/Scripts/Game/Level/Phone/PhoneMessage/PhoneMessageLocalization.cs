
public class PhoneMessageLocalization
{
    private readonly LocalizationString _textMessage;
    private readonly PhoneMessageType _phoneMessageType;
    public bool IsReaded;

    public LocalizationString TextMessageLocalizationString => _textMessage;
    public PhoneMessageType MessageType => _phoneMessageType;

    public PhoneMessageLocalization(string text, PhoneMessageType phoneMessageType, bool isReaded = false)
    {
        _textMessage = new LocalizationString(text);
        _phoneMessageType = phoneMessageType;
        IsReaded = isReaded;
    }
}