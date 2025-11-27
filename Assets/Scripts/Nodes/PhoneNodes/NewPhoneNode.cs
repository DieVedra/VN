using System.Collections.Generic;
using UnityEngine;

public class NewPhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private int _phoneIndex;
    [SerializeField] private int _butteryPercent;
    [SerializeField] private int _startHour;
    [SerializeField] private int _startMinute;
    [SerializeField] private LocalizationString _date;
    
    public IReadOnlyList<Phone> Phones { get; private set; }
    private PhoneUIHandler _phoneUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    // public IReadOnlyList<PhoneContactDataLocalizable> Contacts { get; private set; }
    private int _seriaIndex;

    
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContact> contacts,
        PhoneUIHandler phoneUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler, int seriaIndex)
    {
        Phones = phones;
        _phoneUIHandler = phoneUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        // Contacts = contacts;
        _seriaIndex = seriaIndex;
        // CreateContactsToOnlineAndNotifications(contacts);
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        throw new System.NotImplementedException();
    }
}