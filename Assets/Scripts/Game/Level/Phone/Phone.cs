using System;
using System.Collections.Generic;

[Serializable]
public class Phone
{
    public int LastSeriaIndex { get; private set; }

    private PhoneDataLocalizable _phoneDataLocalizable;
    public string NamePhone { get; private set; }
    public Phone(PhoneDataLocalizable phoneDataLocalizable, string namePhone, int lastSeriaIndex)
    {
        LastSeriaIndex = lastSeriaIndex;
        _phoneDataLocalizable = phoneDataLocalizable;
        NamePhone = namePhone;
    }

    public void AddPhoneData(IReadOnlyList<PhoneContactDataLocalizable> contactDataLocalizables, int seriaIndex)
    {
        _phoneDataLocalizable.AddPhoneContactAndContactData(contactDataLocalizables);
        LastSeriaIndex = seriaIndex;
    }
    public void AddPhoneData(int seriaIndex, params PhoneContactDataLocalizable[] phoneContactDataLocalizable)
    {
        _phoneDataLocalizable.AddPhoneContactAndContactData(phoneContactDataLocalizable);
        LastSeriaIndex = seriaIndex;
    }
}