
using System.Collections.Generic;

public class PhoneContactCombiner
{
    private readonly IReadOnlyList<PhoneContactsProvider> _contactsProviders;

    public PhoneContactCombiner(IReadOnlyList<PhoneContactsProvider> contactsProviders)
    {
        _contactsProviders = contactsProviders;
    }

    public IReadOnlyList<PhoneContactData> GetContactsToSeria(int seriaIndex)
    {
        List<PhoneContactData> contactDatas = new List<PhoneContactData>();

        
        return contactDatas;
    }
}