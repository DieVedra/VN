using System;
using System.Collections.Generic;

[Serializable]
public class Phone
{
    private const int _defaultIndexSeriaInWhichContactWasAdded = -1;
    public int LastSeriaIndex { get; private set; }
    public PhoneDataLocalizable PhoneDataLocalizable { get; private set; }
    public string NamePhone { get; private set; }
    public Phone(PhoneDataLocalizable phoneDataLocalizable, string namePhone, int lastSeriaIndex)
    {
        LastSeriaIndex = lastSeriaIndex;
        PhoneDataLocalizable = phoneDataLocalizable;
        NamePhone = namePhone;
    }

    public void AddPhoneData(int seriaIndex, params PhoneContactDataLocalizable[] phoneContactDataLocalizable)
    {
        AddPhoneData(phoneContactDataLocalizable, seriaIndex, false);
    }

    public void AddPhoneData(IReadOnlyList<PhoneContactDataLocalizable> contactDataLocalizables, int seriaIndex, bool isIntergatedInPhoneData)
    {
        Dictionary<string, PhoneContactDataLocalizable> dictionary;
        if (PhoneDataLocalizable.AddContactData(out dictionary, contactDataLocalizables))
        {
            PhoneDataLocalizable.AddPhoneContacts(dictionary, isIntergatedInPhoneData == true ? _defaultIndexSeriaInWhichContactWasAdded : seriaIndex);
        }
        LastSeriaIndex = seriaIndex;
    }
}