using System.Collections.Generic;
using UnityEngine;

public class PhoneCreatorBuildMode : PhoneCreator
{
    public PhoneCreatorBuildMode(IReadOnlyList<PhoneDataProvider> dataProviders,
        IReadOnlyList<PhoneContactsProvider> contactsToSeriaProviders,
        IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians, PhoneContactCombiner phoneContactCombiner)
        : base(dataProviders, contactsToSeriaProviders, customizableCharacterIndexesCustodians, phoneContactCombiner)
    {
        
    }
    
    public void CreatePhonesOnStart(List<Phone> phones, int currentSeriaIndex)
    {

        //собирает все даты по сериям в список который идет для создания телефона
        var a = GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex);
        if (a.Count == 0)
        {
            Debug.Log($"a.Count == 0");

        }

        for (int i = 0; i < a.Count; i++)
        {
            Debug.Log($"a{i} {a[i]}");
        }

        // Dictionary<string, PhoneDataLocalizable> phoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
        //     GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
        Dictionary<string, PhoneDataLocalizable> phoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
            a, currentSeriaIndex);
        foreach (var VARIABLE in phoneDatas)
        {
            Debug.Log($"VARIABLE {VARIABLE.Key}");

        }

        TryCreatePhones(phones, phoneDatas, currentSeriaIndex);
    }
}