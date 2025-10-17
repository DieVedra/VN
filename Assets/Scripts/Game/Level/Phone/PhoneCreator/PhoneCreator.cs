using System.Collections.Generic;

public class PhoneCreator
{
    private readonly PhoneContactCombiner _phoneContactCombiner;
    private readonly IReadOnlyList<PhoneDataProvider> _dataProviders;
    private readonly IReadOnlyList<PhoneContactsProvider> _contactsToSeriaProviders;
    private readonly IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    public PhoneCreator(IReadOnlyList<PhoneDataProvider> dataProviders,
        IReadOnlyList<PhoneContactsProvider> contactsToSeriaProviders,
        IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians, PhoneContactCombiner phoneContactCombiner)
    {
        _dataProviders = dataProviders;
        _contactsToSeriaProviders = contactsToSeriaProviders;
        _phoneContactCombiner = phoneContactCombiner;
        _customizableCharacterIndexesCustodians = customizableCharacterIndexesCustodians;
    }
    //телефоны создаются при первом обращении
    //при повторных обращениях идет проверка последней серии в дате телефона и если она меньше текущей то добавляется текущая и обновляется индекс
    //так же идет проверка наличия новых дат новых телефонов и если они есть то создаются новые
    // public void TryCreatePhonesOrAddData(List<Phone> phones, int currentSeriaIndex)
    // {
    //     Dictionary<string, PhoneDataLocalizable> phoneDatas;
    //     if (phones.Count > 0)
    //     {
    //         //собирает все даты по сериям в список который идет для создания телефона
    //         phoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
    //             GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
    //     }
    //     else
    //     {
    //         //собирает даты по текущей серии и дабвляет к существующим котнактам
    //         phoneDatas = TryGetDataProvidersByCurrentSeria(currentSeriaIndex);
    //         TryAddIntegratedContent(phones, currentSeriaIndex, phoneDatas);
    //     }
    //     TryCreatePhones(phones, phoneDatas, currentSeriaIndex);
    // }

    // public void CreatePhonesOnStart(List<Phone> phones, int currentSeriaIndex)
    // {
    //     if (phones.Count > 0)
    //     {
    //         //собирает все даты по сериям в список который идет для создания телефона
    //         Dictionary<string, PhoneDataLocalizable> phoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
    //             GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
    //         TryCreatePhones(phones, phoneDatas, currentSeriaIndex);
    //     }
    // }

    public void TryAddDataToIntegratedContactsAndTryCreateNewPhones(List<Phone> phones, int currentSeriaIndex)
    {
        Dictionary<string, PhoneDataLocalizable> phoneDatas = TryGetDataProvidersByCurrentSeria(currentSeriaIndex);
        TryAddIntegratedContent(phones, currentSeriaIndex, phoneDatas);
        TryCreatePhones(phones, phoneDatas, currentSeriaIndex); // создаст новые если что то останется в списке

    }
    private void TryAddIntegratedContent(List<Phone> phones, int currentSeriaIndex, Dictionary<string, PhoneDataLocalizable> phoneDatas)
    {
        if (phoneDatas.Count > 0)
        {
            for (int i = 0; i < phones.Count; i++)
            {
                if (phones[i].LastSeriaIndex < currentSeriaIndex)
                {
                    if (phoneDatas.TryGetValue(phones[i].NamePhone, out PhoneDataLocalizable phoneDataLocalizable))
                    {
                        phones[i].AddPhoneData(phoneDataLocalizable.PhoneContactDatasLocalizable, currentSeriaIndex, true);
                        phoneDatas.Remove(phones[i].NamePhone);
                    }
                }
            }
        }
    }

    protected List<PhoneData> GetAllDataProvidersWithContentFromPreviousSeries(int seriaIndex)
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

    protected Dictionary<string, PhoneDataLocalizable> CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(List<PhoneData> dataProviders, int currentSeriaIndex)
    {
        Dictionary<string, PhoneDataLocalizable> phoneDatas = new Dictionary<string, PhoneDataLocalizable>();
        PhoneData phoneData = null;
        for (int i = 0; i < dataProviders.Count; i++)
        {
            phoneData = dataProviders[i];
            if (phoneDatas.TryGetValue(phoneData.NamePhone, out PhoneDataLocalizable value))
            {
                value.AddContactData(_phoneContactCombiner.CreateNewPhoneContactData(phoneData.PhoneContactDatas, phoneData.SeriaIndex < currentSeriaIndex));
                TryAddNewContent(phoneData, value);
            }
            else
            {
                var newPhoneData = GetNewPhoneData(phoneData, currentSeriaIndex);
                TryAddNewContent(phoneData, newPhoneData);
                phoneDatas.Add(phoneData.NamePhone, newPhoneData);
            }
        }

        return phoneDatas;
    }

    private Dictionary<string, PhoneDataLocalizable> TryGetDataProvidersByCurrentSeria(int currentSeriaIndex)
    {
        Dictionary<string, PhoneDataLocalizable> phoneDatas = new Dictionary<string, PhoneDataLocalizable>();
        for (int i = 0; i < _dataProviders.Count; i++)
        {
            if (_dataProviders[i].SeriaIndex == currentSeriaIndex)
            {
                PhoneData phoneData;
                for (int j = 0; j < _dataProviders[i].PhoneDatas.Count; j++)
                {
                    phoneData = _dataProviders[i].PhoneDatas[j];
                    var newPhoneData = GetNewPhoneData(phoneData, currentSeriaIndex);
                    TryAddNewContent(phoneData, newPhoneData);
                    phoneDatas.Add(phoneData.NamePhone, newPhoneData);
                }
                break;
            }
        }
        return phoneDatas;
    }

    private PhoneDataLocalizable GetNewPhoneData(PhoneData phoneData, int currentSeriaIndex)
    {
        //создать новые и пометить прочитанными если индекс серии меньше текущей
        var contactDataLocalizable = _phoneContactCombiner.CreateNewPhoneContactData(phoneData.PhoneContactDatas, phoneData.SeriaIndex < currentSeriaIndex);
        return new PhoneDataLocalizable(contactDataLocalizable);
    }

    protected void TryCreatePhones(List<Phone> phones, Dictionary<string, PhoneDataLocalizable> newPhoneDatas, int currentSeriaIndex)
    {
        foreach (var pair in newPhoneDatas)
        {
            Phone phone = new Phone(pair.Value, pair.Key, currentSeriaIndex);
            phones.Add(phone);
        }
    }
    private int GetIndexBodyCustomizableCharacter(string nameKey)
    {
        if (_customizableCharacterIndexesCustodians.TryGetValue(nameKey, out CustomizableCharacterIndexesCustodian value))
        {
            return value.BodyIndexRP.Value;
        }
        else
        {
            return 0;
        }
    }

    private void TryAddNewContent(PhoneData phoneData , PhoneDataLocalizable phoneDataLocalizable)
    {
        if (phoneData.NewContentAdded == true)
        {
            var sprite = phoneData.Hands[GetIndexBodyCustomizableCharacter(phoneData.CharacterNameKey)];
            if (sprite != null)
            {
                phoneDataLocalizable.SetHandSprite(sprite);
            }
            sprite = phoneData.PhoneFrame;
            if (sprite != null)
            {
                phoneDataLocalizable.SetPhoneFrameSprite(sprite);
            }

            sprite = phoneData.Background;
            if (sprite != null)
            {
                phoneDataLocalizable.SetPhoneBackgroundSprite(sprite);
            }
        }
    }
}