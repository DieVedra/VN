
public class PhoneMessageLocalization
{
    private readonly LocalizationString _textMessage;
    public bool IsReaded;


    public LocalizationString TextMessageLocalizationString => _textMessage;

    public PhoneMessageLocalization(string text, bool isReaded = false)
    {
        _textMessage = new LocalizationString(text);
        IsReaded = isReaded;
    }
}