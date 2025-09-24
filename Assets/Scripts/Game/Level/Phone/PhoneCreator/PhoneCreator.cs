using System.Collections.Generic;
using UnityEngine;

public class PhoneCreator
{
    private readonly PhoneContactCombiner _phoneContactCombiner;
    private readonly IReadOnlyList<PhoneDataProvider> _dataProviders;

    public PhoneCreator(IReadOnlyList<PhoneDataProvider> dataProviders, PhoneContactCombiner phoneContactCombiner)
    {
        _dataProviders = dataProviders;
        _phoneContactCombiner = phoneContactCombiner;
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
        Debug.Log($"seriaIndex {seriaIndex} {_dataProviders.Count}");
        for (int i = 0; i < _dataProviders.Count; i++)
        {
            Debug.Log($"111");

            if (_dataProviders[i].SeriaIndex <= seriaIndex)
            {
                Debug.Log($"111");

                phoneDatas.AddRange(_dataProviders[i].PhoneDatas);
            }
        }
        return phoneDatas;
    }

    protected Dictionary<string, PhoneDataLocalizable> CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(List<PhoneData> dataProviders, int currentSeriaIndex)
    {
        //если телефон создан то проверить новую дату и добавить ее если она есть
        //
        Dictionary<string, PhoneDataLocalizable> phoneDatas = new Dictionary<string, PhoneDataLocalizable>();
        PhoneData phoneData = null;
        for (int i = 0; i < dataProviders.Count; i++)
        {
            phoneData = dataProviders[i];
            if (phoneDatas.TryGetValue(phoneData.NamePhone, out PhoneDataLocalizable value))
            {
                value.AddPhoneContactAndContactData(_phoneContactCombiner.CreateNewPhoneContactData(phoneData.PhoneContactDatas, phoneData.SeriaIndex < currentSeriaIndex));
            }
            else
            {
                phoneDatas.Add(phoneData.NamePhone, GetNewPhoneData(phoneData, currentSeriaIndex));
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
                    phoneDatas.Add(phoneData.NamePhone, GetNewPhoneData(phoneData, currentSeriaIndex));
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
}