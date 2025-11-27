using System.Collections.Generic;

public class PhoneSaveHandler
{
    //поставляется список добавленных контактов
    //в элементе списка содержатся: 1. имя телефона к которому контакт
    //2. ключ имени контакта
    //3. список индексов прочитанных сообщений последней серии
    //4. индекс серии в которой он был добавлен 
    //нужно добавить в телефон контакты с диапазоном контента от  серии добавления контакта до текущей
    // в сообщениях текущей серии должны быть проставлены актуальные ключи прочитанных

    // public IReadOnlyList<PhoneAddedContact> GetSaveData(List<Phone> phones)
    // {
    //     List<PhoneAddedContact> datas = new List<PhoneAddedContact>();
    //     PhoneContactDataLocalizable phoneContactDataLocalizable;
    //     int count;
    //     for (int i = 0; i < phones.Count; i++)
    //     {
    //         count = phones[i].PhoneDataLocalizable.PhoneContactDatasLocalizable.Count;
    //         for (int j = 0; j < count; j++)
    //         {
    //             phoneContactDataLocalizable = phones[i].PhoneDataLocalizable.PhoneContactDatasLocalizable[j];
    //             PhoneAddedContact data = new PhoneAddedContact
    //             {
    //                 PhoneName = phones[i].NamePhone, ContactIndex = j, ContactNameKey = phoneContactDataLocalizable.NameContact.Key,
    //                 IndexSeriaInWhichContactWasAdded = phoneContactDataLocalizable.IndexSeriaInWhichContactWasAdded,
    //                 LastSeriaReadedMessagesIndexes = GetLastSeriaReadedMessagesIndexes(phoneContactDataLocalizable)
    //             };
    //             datas.Add(data);
    //         }
    //     }
    //     return datas;
    // }
    
    // public void AddContactsToPhoneFromSaveData(List<Phone> phones, IReadOnlyList<PhoneContactsProvider> contactsToSeriaProviders,
    //     IReadOnlyList<PhoneAddedContact> saveDataContacts, int currentSeriaIndex)
    // {
    //     Phone currentPhone = null;
    //     PhoneAddedContact currentSaveData = null;
    //     for (int i = 0; i < saveDataContacts.Count; i++)
    //     {
    //         currentSaveData = saveDataContacts[i];
    //         var contact = TryCreatePhoneContactDataLocalizable(contactsToSeriaProviders, currentSaveData, currentSeriaIndex);
    //         if (contact != null)
    //         {
    //             currentPhone = GetCurrentPhone(phones, currentSaveData.PhoneName);
    //             currentPhone.PhoneDataLocalizable.InsertPhoneContact(contact, currentSaveData.ContactIndex);
    //         }
    //     }
    // }
    private Phone GetCurrentPhone(List<Phone> phones, string namePhone)
    {
        Phone result = null;
        for (int j = 0; j < phones.Count; j++)
        {
            if (namePhone == phones[j].NamePhone)
            {
                result = phones[j];
                break;
            }
        }
        return result;
    }

    // private PhoneContactDataLocalizable TryCreatePhoneContactDataLocalizable(
    //     IReadOnlyList<PhoneContactsProvider> contactsProviders, PhoneAddedContact data, int currentSeriaIndex)
    // {
    //     string nameKey;
    //     int indexSeriaInWhichContactWasAdded = data.IndexSeriaInWhichContactWasAdded;
    //     PhoneContact phoneContact;
    //     PhoneContactDataLocalizable contactDataLocalizable = null;
    //     for (int i = 0; i < contactsProviders.Count; i++)
    //     {
    //         if (contactsProviders[i].SeriaIndex >= indexSeriaInWhichContactWasAdded && contactsProviders[i].SeriaIndex <= currentSeriaIndex)
    //         {
    //             for (int j = 0; j < contactsProviders[i].PhoneContacts.Count; j++)
    //             {
    //                 phoneContact = contactsProviders[i].PhoneContacts[j];
    //                 nameKey = LocalizationString.GenerateStableHash(phoneContact.Name);
    //                 if (data.ContactNameKey == nameKey)
    //                 {
    //                     if (contactDataLocalizable == null)
    //                     {
    //                         contactDataLocalizable = GetPhoneContactDataLocalizable(phoneContact);
    //                         contactDataLocalizable.IndexSeriaInWhichContactWasAdded = indexSeriaInWhichContactWasAdded;
    //                         TryAddMessages(data.LastSeriaReadedMessagesIndexes, contactDataLocalizable,
    //                             phoneContact, contactsProviders[i].SeriaIndex < currentSeriaIndex);
    //                     }
    //                     else
    //                     {
    //                         TryAddMessages(data.LastSeriaReadedMessagesIndexes, contactDataLocalizable,
    //                             phoneContact, contactsProviders[i].SeriaIndex < currentSeriaIndex);
    //                     }
    //                 }
    //             }
    //         }
    //     }
    //     return contactDataLocalizable;
    // }

    // private void TryAddMessages(int[] lastSeriaReadedMessagesIndexes, PhoneContactDataLocalizable contactDataLocalizable, PhoneContact phoneContact, bool key)
    // {
    //     if (key)
    //     {
    //         // contactDataLocalizable.AddMessages(phoneContactData.PhoneMessages, true);
    //     }
    //     else
    //     {
    //         // contactDataLocalizable.AddMessages(phoneContactData.PhoneMessages, lastSeriaReadedMessagesIndexes);
    //     }
    // }

    // private PhoneContact GetPhoneContactDataLocalizable(PhoneContact phoneContact)
    // {
    //     var contact = new PhoneContact(phoneContact.Name, phoneContact.NikName,
    //         phoneContact.Icon, phoneContact.Color, phoneContact.IsEmptyIconKey, true);
    //     return contact;
    // }

    private int[] GetLastSeriaReadedMessagesIndexes(PhoneContact phoneContactDataLocalizable)
    {
        List<int> indexes = new List<int>();
        // int count = phoneContactDataLocalizable.PhoneMessagesLocalization.Count;
        // PhoneMessageLocalization messageLocalization;
        // for (int i = 0; i < count; i++)
        // {
        //     messageLocalization = phoneContactDataLocalizable.PhoneMessagesLocalization[i];
        //     if (messageLocalization.IsReaded == true)
        //     {
        //         indexes.Add(i);
        //     }
        // }
        return indexes.ToArray();
    }
}