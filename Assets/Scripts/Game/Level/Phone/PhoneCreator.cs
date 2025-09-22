using System.Collections.Generic;
using System.Linq;

public class PhoneCreator
{
    private readonly IReadOnlyList<PhoneDataProvider> _dataProviders;
    // private Dictionary<string, Phone> _phones;

    public PhoneCreator(IReadOnlyList<PhoneDataProvider> dataProviders)
    {
        _dataProviders = dataProviders;
        // _phones = new Dictionary<string, Phone>();
    }

    public IReadOnlyList<Phone> CreatePhones(int seriaIndex)
    {
        List<Phone> phones = new List<Phone>();
        // var phone = new Phone();
        CombineIntoOneNewPhoneData(GetDataProvidersBySeria(seriaIndex), seriaIndex);

        return phones;
    }

    private List<PhoneData> GetDataProvidersBySeria(int seriaIndex)
    {
        List<PhoneData> phoneDatas = new List<PhoneData>();
        for (int i = 0; i < _dataProviders.Count; i++)
        {
            if (_dataProviders[i].SeriaIndex <= seriaIndex)
            {
                phoneDatas.AddRange(_dataProviders[i].PhoneDatas);
            }
        }
        return phoneDatas;
    }
    private Dictionary<string, PhoneData> CombineIntoOneNewPhoneData(List<PhoneData> dataProviders, int currentSeriaIndex)
    {
        Dictionary<string, PhoneData> phoneDatas = new Dictionary<string, PhoneData>();
        PhoneData data = null;
        for (int i = 0; i < dataProviders.Count; i++)
        {
            data = dataProviders[i];
            if (phoneDatas.TryGetValue(data.NamePhone, out PhoneData phoneData))
            {
                phoneData.AddDataToPhoneContact(data.PhoneContactDatas.ToDictionary(x =>x.LocalizationString.Key));
            }
            else
            {
                phoneDatas.Add(data.NamePhone, GetNewPhoneData(data, currentSeriaIndex));
            }
        }

        return phoneDatas;
    }

    private PhoneData GetNewPhoneData(PhoneData phoneData, int currentSeriaIndex)
    {
        var newPhoneData = new PhoneData(currentSeriaIndex, phoneData.NamePhone, phoneData.KeyCharacterName);
        var dic = phoneData.PhoneContactDatas.ToDictionary(x => x.LocalizationString.Key);
        if (phoneData.SeriaIndex < currentSeriaIndex)
        {
            // SetMessagesReadedBySeria()
        }
        
        newPhoneData.AddDataToPhoneContact(dic);
        return newPhoneData;
    }

    private void SetMessagesReadedBySeria(PhoneData phoneData, int currentSeriaIndex)
    {
        
    }

    private PhoneContactData GetNewPhoneContactData(PhoneContactData data, bool seriaKey)
    {
        PhoneContactData newPhoneContactData = new PhoneContactData(data.Name, data.Icon);
        List<PhoneMessage> newPhoneMessages = new List<PhoneMessage>();
        PhoneMessage message;
        for (int i = 0; i < data.PhoneMessages.Count; i++)
        {
            // message = data.PhoneMessages[i];
            // if (seriaKey == true)
            // {
            //     newPhoneMessages.Add(new PhoneMessage(message.LocalizationString.DefaultText, message.IsReaded));
            //
            // }
            // else
            // {
            //     newPhoneMessages.Add(new PhoneMessage());
            //
            // }
        }
        // data
        // newPhoneContactData.AddMessages();
        return newPhoneContactData;
    }
}