using System.Collections.Generic;

public class PhoneCreatorEditMode : PhoneCreator
{
    public PhoneCreatorEditMode(IReadOnlyList<PhoneDataProvider> dataProviders,
        IReadOnlyList<PhoneContactsProvider> contactsToSeriaProviders,
        IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians, 
        PhoneContactCombiner phoneContactCombiner)
        : base(dataProviders, contactsToSeriaProviders, customizableCharacterIndexesCustodians, phoneContactCombiner) { }
    
    
    //в лайтайме в эдитор режиме телефоны поставляются  в каждую серию отдельные со своим ограниченным контентом
    public List<Phone> TryCreatePhonesForNonPlayMode(int currentSeriaIndex)
    {
        var newPhoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
            GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
        List<Phone> list = new List<Phone>(newPhoneDatas.Count);
        TryCreatePhones(list, newPhoneDatas, currentSeriaIndex);
        return list;
    }
    public void CreatePhonesOnStart(List<Phone> phones, int currentSeriaIndex)
    {
        //собирает все даты по сериям в список который идет для создания телефона
        Dictionary<string, PhoneDataLocalizable> phoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
            GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
        TryCreatePhones(phones, phoneDatas, currentSeriaIndex);
    }

    
}