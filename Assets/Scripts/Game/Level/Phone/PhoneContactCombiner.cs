
using System.Collections.Generic;

public class PhoneContactCombiner
{
    public List<PhoneContactDataLocalizable> CreateNewPhoneContactData(IReadOnlyList<PhoneContactData> phoneContactDatas, bool keyMessagesReadedBySeria)
    {
        List<PhoneContactDataLocalizable> contactDataLocalizables = new List<PhoneContactDataLocalizable>();
        for (int i = 0; i < phoneContactDatas.Count; i++)
        {
            var contactDataLocalizable = new PhoneContactDataLocalizable(phoneContactDatas[i].Name, phoneContactDatas[i].Icon);
            contactDataLocalizable.AddMessages(phoneContactDatas[i].PhoneMessages, keyMessagesReadedBySeria);
            contactDataLocalizables.Add(contactDataLocalizable);
        }
        return contactDataLocalizables;
    }
}