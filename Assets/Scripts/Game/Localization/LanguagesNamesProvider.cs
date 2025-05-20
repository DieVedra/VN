using System.Collections.Generic;

public class LanguagesNamesProvider
{
    private const string _russian = "Русский";
    private const string _english = "English";

    private Dictionary<LocalizationLanguageCode, string> _names;

    public LanguagesNamesProvider()
    {
        _names = new Dictionary<LocalizationLanguageCode, string>()
        {
            {LocalizationLanguageCode.ru, _russian},
            {LocalizationLanguageCode.en, _english}
        };
    }

    public string GetName(LocalizationLanguageCode code)
    {
        return _names[code];
    }
}