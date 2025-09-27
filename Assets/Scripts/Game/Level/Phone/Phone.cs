using System;
using System.Collections.Generic;

[Serializable]
public class Phone
{
    public int LastSeriaIndex { get; private set; }
    public PhoneDataLocalizable PhoneDataLocalizable { get; private set; }
    public string NamePhone { get; private set; }
    public Phone(PhoneDataLocalizable phoneDataLocalizable, string namePhone, int lastSeriaIndex)
    {
        LastSeriaIndex = lastSeriaIndex;
        PhoneDataLocalizable = phoneDataLocalizable;
        NamePhone = namePhone;
    }
    
    public void AddPhoneData(IReadOnlyList<PhoneContactDataLocalizable> contactDataLocalizables, int seriaIndex)
    {
        PhoneDataLocalizable.AddPhoneContactAndContactData(contactDataLocalizables);
        LastSeriaIndex = seriaIndex;
    }
    public void AddPhoneData(int seriaIndex, params PhoneContactDataLocalizable[] phoneContactDataLocalizable)
    {
        PhoneDataLocalizable.AddPhoneContactAndContactData(phoneContactDataLocalizable);
        LastSeriaIndex = seriaIndex;
    }
}