using System.Collections.Generic;

public class PhoneCreatorEditMode : PhoneCreator
{
    public PhoneCreatorEditMode(IReadOnlyList<PhoneDataProvider> dataProviders, IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians, 
        PhoneContactCombiner phoneContactCombiner)
        : base(dataProviders, customizableCharacterIndexesCustodians, phoneContactCombiner) { }
    public List<Phone> TryCreatePhonesForNonPlayMode(int currentSeriaIndex)
    {
        List<Phone> phones = new List<Phone>();
        var newPhoneDatas = CombineIntoOneNewPhoneDataWithContentFromPreviousSeries(
            GetAllDataProvidersWithContentFromPreviousSeries(currentSeriaIndex), currentSeriaIndex);
        foreach (var pair in newPhoneDatas)
        {
            Phone phone = new Phone(pair.Value, pair.Key, currentSeriaIndex); 
            phones.Add(phone);
        }
        return phones;
    }
}