using System.Collections.Generic;
using System.Linq;

public class PhoneDataLocalizable
{
    private List<PhoneContactDataLocalizable> _phoneContactDatasLocalizable;

    public IReadOnlyList<PhoneContactDataLocalizable> PhoneContactDatasLocalizable => _phoneContactDatasLocalizable;

    public PhoneDataLocalizable(List<PhoneContactDataLocalizable> phoneContactDatasLocalizable)
    {
        _phoneContactDatasLocalizable = phoneContactDatasLocalizable;
    }
    public void AddPhoneContactAndContactData(IReadOnlyList<PhoneContactDataLocalizable> contactDataLocalizables)
    {
        var dictionary = contactDataLocalizables.ToDictionary(x => x.NameContactLocalizationString.Key);
        PhoneContactDataLocalizable contact;
        for (int i = 0; i < _phoneContactDatasLocalizable.Count; i++)
        {
            contact = _phoneContactDatasLocalizable[i];
            if (dictionary.TryGetValue(contact.NameContactLocalizationString.Key, out PhoneContactDataLocalizable value))
            {
                contact.AddMessages(value.PhoneMessagesLocalization);
                dictionary.Remove(contact.NameContactLocalizationString.Key);
            }
        }
        if (dictionary.Count > 0)
        {
            foreach (var pair in dictionary)
            {
                _phoneContactDatasLocalizable.Add(pair.Value);
            }
        }
    }
}