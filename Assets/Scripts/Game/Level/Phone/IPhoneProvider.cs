using System.Collections.Generic;

public interface IPhoneProvider
{
    public IReadOnlyList<Phone> GetPhones(int currentSeriaIndex);
    public IReadOnlyList<PhoneContact> GetContactsToAddInPhoneInPlot(int seriaIndex);
}