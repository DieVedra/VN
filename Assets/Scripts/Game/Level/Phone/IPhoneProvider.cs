
using System.Collections.Generic;

public interface IPhoneProvider
{
    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex);
    public IReadOnlyList<PhoneContactDataLocalizable> GetContactsToSeria(int seriaIndex);
}