using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PhoneCreator
{
    private readonly IReadOnlyList<PhoneContactsProvider> _contacts;
    private readonly IReadOnlyList<PhoneProvider> _phoneProviders;
    private readonly Dictionary<string, Phone> _phonesDictionary;
    
    private readonly IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    private readonly CheckMathSeriaIndex _checkMathSeriaIndex;

    public PhoneCreator(IReadOnlyList<PhoneProvider> phoneProviders, IReadOnlyList<PhoneContactsProvider> contacts,
        IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians,
        CheckMathSeriaIndex checkMathSeriaIndex)
    {
        _phoneProviders = phoneProviders;
        _contacts = contacts;
        _customizableCharacterIndexesCustodians = customizableCharacterIndexesCustodians;
        _checkMathSeriaIndex = checkMathSeriaIndex;
        _phonesDictionary = new Dictionary<string, Phone>();
    }
    
    
    public List<Phone> CreatePhonesOnStart(int currentSeriaIndex, bool isRuntime = true)
    {
        if (isRuntime == false)
        {
            
        }
        PhoneProvider phoneProvider;
        Phone phone;
        List<Phone> phones = new List<Phone>();
        for (int i = 0; i < _phoneProviders.Count; i++)
        {
            phoneProvider = _phoneProviders[i];
            if (phoneProvider.SeriaIndex <= currentSeriaIndex)
            {
                for (int j = 0; j < phoneProvider.Phone.Count; j++)
                {
                    phone = phoneProvider.Phone[j];
                    if (_phonesDictionary.ContainsKey(phone.NamePhone.Key) == false)
                    {
                        var newPhone = new Phone(phone.NamePhone, phone.Hands, phone.PhoneFrame, phone.Background);
                        _phonesDictionary.Add(phone.NamePhone.Key, newPhone);
                    }
                }
            }
        }
        phones.AddRange(_phonesDictionary.Select(x=>x.Value).ToList());
        if (isRuntime == false)
        {
            _phonesDictionary.Clear();
        }
        return phones;
        
    }

    public void TryAddPhone(List<Phone> phones, int currentSeriaIndex)
    {
        if (_checkMathSeriaIndex.Check(currentSeriaIndex))
        {
            PhoneProvider phoneProvider;
            Phone phone;
            for (int i = 0; i < _phoneProviders.Count; i++)
            {
                phoneProvider = _phoneProviders[i];
                if (phoneProvider.SeriaIndex == currentSeriaIndex)
                {
                    for (int j = 0; j < phoneProvider.Phone.Count; j++)
                    {
                        phone = phoneProvider.Phone[j];
                        if (_phonesDictionary.ContainsKey(phone.NamePhone.Key) == false)
                        {
                            phones.Add(phone);
                            _phonesDictionary.Add(phone.NamePhone.Key, phone);
                        }
                    }
                }
            }
        }
    }


    
    // public void TryAddDataToIntegratedContactsAndTryCreateNewPhones(List<Phone> phones, int currentSeriaIndex)
    // {
    //     Dictionary<string, PhoneDataLocalizable> phoneDatas = TryGetDataProvidersByCurrentSeria(currentSeriaIndex);
    //     TryAddIntegratedContent(phones, currentSeriaIndex, phoneDatas);
    //     TryCreatePhones(phones, phoneDatas, currentSeriaIndex); // создаст новые если что то останется в списке
    // }
    // private void TryAddIntegratedContent(List<Phone> phones, int currentSeriaIndex, Dictionary<string, PhoneDataLocalizable> phoneDatas)
    // {
    //     if (phoneDatas.Count > 0)
    //     {
    //         for (int i = 0; i < phones.Count; i++)
    //         {
    //             if (phones[i].LastSeriaIndex < currentSeriaIndex)
    //             {
    //                 if (phoneDatas.TryGetValue(phones[i].NamePhone, out PhoneDataLocalizable phoneDataLocalizable))
    //                 {
    //                     phones[i].AddPhoneData(phoneDataLocalizable.PhoneContactDatasLocalizable, currentSeriaIndex, true);
    //                     phoneDatas.Remove(phones[i].NamePhone);
    //                 }
    //             }
    //         }
    //     }
    // }

    // protected List<PhoneData> GetAllDataProvidersWithContentFromPreviousSeries(int seriaIndex)
    // {
    //     List<PhoneData> phoneDatas = new List<PhoneData>();
    //     for (int i = 0; i < _dataProviders.Count; i++)
    //     {
    //         if (_dataProviders[i].SeriaIndex <= seriaIndex)
    //         {
    //             phoneDatas.AddRange(_dataProviders[i].PhoneDatas);
    //         }
    //     }
    //     return phoneDatas;
    // }

    // protected Dictionary<string, PhoneDataLocalizable> CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(List<PhoneData> dataProviders, int currentSeriaIndex)
    // {
    //     Dictionary<string, PhoneDataLocalizable> phoneDatas = new Dictionary<string, PhoneDataLocalizable>();
    //     PhoneData phoneData = null;
    //     for (int i = 0; i < dataProviders.Count; i++)
    //     {
    //         phoneData = dataProviders[i];
    //         if (phoneDatas.TryGetValue(phoneData.NamePhone, out PhoneDataLocalizable value))
    //         {
    //             value.AddContactData(_phoneContactCombiner.CreateNewPhoneContactData(phoneData.PhoneContactDatas, phoneData.SeriaIndex < currentSeriaIndex));
    //             TryAddNewContent(phoneData, value);
    //         }
    //         else
    //         {
    //             var newPhoneData = GetNewPhoneData(phoneData, currentSeriaIndex);
    //             TryAddNewContent(phoneData, newPhoneData);
    //             phoneDatas.Add(phoneData.NamePhone, newPhoneData);
    //         }
    //     }
    //
    //     return phoneDatas;
    // }

    // private Dictionary<string, PhoneDataLocalizable> TryGetDataProvidersByCurrentSeria(int currentSeriaIndex)
    // {
    //     Dictionary<string, PhoneDataLocalizable> phoneDatas = new Dictionary<string, PhoneDataLocalizable>();
    //     for (int i = 0; i < _dataProviders.Count; i++)
    //     {
    //         if (_dataProviders[i].SeriaIndex == currentSeriaIndex)
    //         {
    //             PhoneData phoneData;
    //             for (int j = 0; j < _dataProviders[i].PhoneDatas.Count; j++)
    //             {
    //                 phoneData = _dataProviders[i].PhoneDatas[j];
    //                 var newPhoneData = GetNewPhoneData(phoneData, currentSeriaIndex);
    //                 TryAddNewContent(phoneData, newPhoneData);
    //                 phoneDatas.Add(phoneData.NamePhone, newPhoneData);
    //             }
    //             break;
    //         }
    //     }
    //     return phoneDatas;
    // }

    // private PhoneDataLocalizable GetNewPhoneData(PhoneData phoneData, int currentSeriaIndex)
    // {
    //     //создать новые и пометить прочитанными если индекс серии меньше текущей
    //     var contactDataLocalizable = _phoneContactCombiner.CreateNewPhoneContactData(phoneData.PhoneContactDatas, phoneData.SeriaIndex < currentSeriaIndex);
    //     return new PhoneDataLocalizable(contactDataLocalizable);
    // }

    // protected void TryCreatePhones(List<Phone> phones, Dictionary<string, PhoneDataLocalizable> newPhoneDatas, int currentSeriaIndex)
    // {
    //     bool key = true;
    //     foreach (var pair in newPhoneDatas)
    //     {
    //         for (int i = 0; i < phones.Count; i++)
    //         {
    //             if (phones[i].NamePhone == pair.Key)
    //             {
    //                 key = false;
    //                 break;
    //             }
    //             else
    //             {
    //                 key = true;
    //             }
    //         }
    //         if (key == true)
    //         {
    //             Phone phone = new Phone(pair.Value, pair.Key, currentSeriaIndex);
    //             phones.Add(phone);
    //         }
    //     }
    // }
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

    // private void TryAddNewContent(PhoneData phoneData , PhoneDataLocalizable phoneDataLocalizable)
    // {
    //     if (phoneData.Hands?.Count > 0)
    //     {
    //         int index = GetIndexBodyCustomizableCharacter(phoneData.CharacterNameKey);
    //         if (index < phoneData.Hands.Count)
    //         {
    //             var sprite = phoneData.Hands[index];
    //             if (sprite != null)
    //             {
    //                 phoneDataLocalizable.SetHandSprite(sprite);
    //             }
    //         }
    //     }
    //
    //     if (phoneData.PhoneFrame != null)
    //     {
    //         phoneDataLocalizable.SetPhoneFrameSprite(phoneData.PhoneFrame);
    //     }
    //
    //     if (phoneData.Background != null)
    //     {
    //         phoneDataLocalizable.SetPhoneBackgroundSprite(phoneData.Background);
    //     }
    // }
}