using System.Collections.Generic;

public class PhoneContactCombiner
{
    public List<PhoneContactDataLocalizable> CreateNewPhoneContactData(IReadOnlyList<PhoneContactData> phoneContactDatas, bool keyMessagesReadedBySeria)
    {
        List<PhoneContactDataLocalizable> contactDataLocalizables = new List<PhoneContactDataLocalizable>();
        for (int i = 0; i < phoneContactDatas.Count; i++)
        {
            var contactDataLocalizable = new PhoneContactDataLocalizable(
                phoneContactDatas[i].Name, phoneContactDatas[i].NikName, phoneContactDatas[i].Icon, phoneContactDatas[i].Color,  phoneContactDatas[i].IsEmptyIconKey);
            
            // contactDataLocalizable.AddMessages(phoneContactDatas[i].PhoneMessages, keyMessagesReadedBySeria);
            
            contactDataLocalizables.Add(contactDataLocalizable);
        }
        return contactDataLocalizables;
    }
    //собирает в список имена контактов с первыми сообщениями
    //нужны только для первого добавления 
    public void TryCreateAddebleContactsDataLocalizable(Dictionary<string, PhoneContactDataLocalizable> toContactDatas, IReadOnlyList<PhoneContactData> fromPhoneContactDatas)
    {
        for (int i = 0; i < fromPhoneContactDatas.Count; i++)
        {
            string key = LocalizationString.GenerateStableHash(fromPhoneContactDatas[i].Name);

            if (toContactDatas.ContainsKey(key) == false)
            {
                toContactDatas.Add(key, new PhoneContactDataLocalizable(fromPhoneContactDatas[i].Name, fromPhoneContactDatas[i].NikName,
                    fromPhoneContactDatas[i].Icon, fromPhoneContactDatas[i].Color, fromPhoneContactDatas[i].IsEmptyIconKey, true));
                // toContactDatas[key].AddMessages(fromPhoneContactDatas[i].PhoneMessages);
            }
        }
    }
}