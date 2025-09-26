
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#07B715")]
public class PhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private int _phoneIndex;
    [SerializeField] private PhoneBackgroundScreen _phoneStartScreen;
    [SerializeField] private int _butteryPercent;
    [SerializeField] private int _startHour;
    [SerializeField] private int _startMinute;
    [SerializeField] private LocalizationString _date;
    [SerializeField] private bool _showStartInfo;
    [SerializeField] private bool _blockScreenNotificationKey;
    [SerializeField] private int _blockScreenCharacterIndex;

    
    
    [SerializeField] private LocalizationString _dayLocalizationString;
    // [SerializeField] private LocalizationString _notificationNameLocalizationString;
    // [SerializeField] private LocalizationString _notificationLocalizationString;

    private const string _blockScreenName = "BlockScreen";
    private const string _contactsScreenName = "ContactsScreen";
    private const string _dialogScreenName = "DialogScreen";
    private PhoneUIHandler _phoneUIHandler;
    
    public IReadOnlyList<Phone> Phones { get; private set; }

    public IReadOnlyList<PhoneContactDataLocalizable> PhoneContactDatasLocalizable =>
        Phones[_phoneIndex].PhoneDataLocalizable.PhoneContactDatasLocalizable;
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, PhoneUIHandler phoneUIHandler)
    {
        Phones = phones;
        _phoneUIHandler = phoneUIHandler;
    }

    public override UniTask Enter(bool isMerged = false)
    {
        // _phoneUIHandler.SetBackground(_phoneBackgroundScreens[_phoneStartScreenIndex], );

        SetInfoToView();
        
        return base.Enter(isMerged);
    }

    protected override void SetInfoToView()
    {
        switch (_phoneStartScreen)
        {
            case PhoneBackgroundScreen.BlockScreen:
                _phoneUIHandler.SetBlockScreenBackground(PhoneContactDatasLocalizable[_blockScreenCharacterIndex],
                    _dayLocalizationString, SetLocalizationChangeEvent, _butteryPercent, _startHour, _startMinute, IsPlayMode(), false);
                break;
            // case PhoneBackgroundScreen.ContactsScreen:
            //     _phoneUIHandler.SetContactsScreenBackground(_butteryPercent);
            //     break;
            // case PhoneBackgroundScreen.DialogScreen:
            //     _phoneUIHandler.SetDialogScreenBackground(_butteryPercent);
            //     break;
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        switch (_phoneStartScreen)
        {
            case PhoneBackgroundScreen.BlockScreen:
                strings.Add(_dayLocalizationString);
                // strings.Add(_notificationNameLocalizationString);
                break;
            // case (int)PhoneBackgroundScreen.ContactsScreen:
            //     strings.Add();
            //     break;
            // case (int)PhoneBackgroundScreen.DialogScreen:
            //     strings.Add();
            //     break;
        }

        return strings;
    }
}