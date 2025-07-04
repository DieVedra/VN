
using System.Collections.Generic;

public interface ILocalizable
{
    public IReadOnlyList<LocalizationString> GetLocalizableContent();
}