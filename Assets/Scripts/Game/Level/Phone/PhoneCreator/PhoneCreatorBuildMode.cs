

using System.Collections.Generic;

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
        if (phones.Count > 0)
        {
            //собирает все даты по сериям в список который идет для создания телефона
            Dictionary<string, PhoneDataLocalizable> phoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
                GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
            TryCreatePhones(phones, phoneDatas, currentSeriaIndex);
        }
    }
}