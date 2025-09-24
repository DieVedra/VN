
using System.Collections.Generic;
using UnityEngine;

public class PhoneCreatorEditMode : PhoneCreator
{
    public PhoneCreatorEditMode(IReadOnlyList<PhoneDataProvider> dataProviders, PhoneContactCombiner phoneContactCombiner)
        : base(dataProviders, phoneContactCombiner) { }
    public List<Phone> TryCreatePhonesForNonPlayMode(int currentSeriaIndex)
    {
        List<Phone> phones = new List<Phone>();
        var newPhoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
            GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
        Debug.Log($"newPhoneDatas {newPhoneDatas.Count}");
        foreach (var pair in newPhoneDatas)
        {
            Phone phone = new Phone(pair.Value, pair.Key, currentSeriaIndex); 
            phones.Add(phone);
        }
        return phones;
    }
}