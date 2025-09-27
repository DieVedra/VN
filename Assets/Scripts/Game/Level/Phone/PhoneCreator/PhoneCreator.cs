using System.Collections.Generic;

public class PhoneCreator
{
    private readonly PhoneContactCombiner _phoneContactCombiner;
    private readonly IReadOnlyList<PhoneDataProvider> _dataProviders;
    private readonly IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    public PhoneCreator(IReadOnlyList<PhoneDataProvider> dataProviders, IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians, PhoneContactCombiner phoneContactCombiner)
    {
        _dataProviders = dataProviders;
        _phoneContactCombiner = phoneContactCombiner;
        _customizableCharacterIndexesCustodians = customizableCharacterIndexesCustodians;
    }
    //телефоны создаются при первом обращении
    //при повторных обращениях идет проверка последней серии в дате телефона и если она меньше текущей то добавляется текущая и обновляется индекс
    //так же идет проверка наличия новых дат новых телефонов и если они есть то создаются новые
    public void TryCreatePhones(List<Phone> phones, int currentSeriaIndex)
    {
        if (phones.Count > 0)
        {
            var newPhoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
                GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
            if (newPhoneDatas.Count > 0)
            {
                CreatePhones(phones, currentSeriaIndex, newPhoneDatas);
            }
        }
        else
        {
            var phoneDatas = TryGetDataProvidersByCurrentSeria(currentSeriaIndex);
            if (phoneDatas.Count > 0)
            {
                for (int i = 0; i < phones.Count; i++)
                {
                    if (phones[i].LastSeriaIndex < currentSeriaIndex)
                    {
                        if (phoneDatas.TryGetValue(phones[i].NamePhone, out PhoneDataLocalizable phoneDataLocalizable))
                        {
                            phones[i].AddPhoneData(phoneDataLocalizable.PhoneContactDatasLocalizable, currentSeriaIndex);
                            phoneDatas.Remove(phones[i].NamePhone);
                        }
                    }
                }

                if (phoneDatas.Count > 0)
                {
                    CreatePhones(phones, currentSeriaIndex, phoneDatas);
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
                value.AddPhoneContactAndContactData(_phoneContactCombiner.CreateNewPhoneContactData(phoneData.PhoneContactDatas, phoneData.SeriaIndex < currentSeriaIndex));
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

    private void CreatePhones(List<Phone> phones, int currentSeriaIndex, Dictionary<string, PhoneDataLocalizable> newPhoneDatas)
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