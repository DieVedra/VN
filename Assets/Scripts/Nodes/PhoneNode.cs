
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#07D418")]
public class PhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private int _phoneIndex;
    [SerializeField] private int _phoneScreenIndex;
    [SerializeField] private int _butteryPercent;

    [SerializeField] private LocalizationString _dayLocalizationString;
    [SerializeField] private LocalizationString _notificationNameLocalizationString;
    // [SerializeField] private LocalizationString _notificationLocalizationString;

    private const string _blockScreenName = "BlockScreen";
    private const string _contactsScreenName = "ContactsScreen";
    private const string _dialogScreenName = "DialogScreen";
    private PhoneUIHandler _phoneUIHandler;
    
    public IReadOnlyList<PhoneData> DataProviders { get; private set; }
    public void ConstructMyPhoneNode(IReadOnlyList<PhoneData> dataProviders, PhoneUIHandler phoneUIHandler)
    {
        DataProviders = dataProviders;
        _phoneUIHandler = phoneUIHandler;
    }

    public override UniTask Enter(bool isMerged = false)
    {
        // _phoneUIHandler.SetBackground(_phoneBackgroundScreens[_phoneScreenIndex], );
        
        switch (_phoneScreenIndex)
        {
            case (int)PhoneBackgroundScreen.BlockScreen:
                _phoneUIHandler.SetBlockScreenBackground(_butteryPercent);
                break;
            case (int)PhoneBackgroundScreen.ContactsScreen:
                _phoneUIHandler.SetContactsScreenBackground(_butteryPercent);
                break;
            case (int)PhoneBackgroundScreen.DialogScreen:
                _phoneUIHandler.SetDialogScreenBackground(_butteryPercent);
                break;
        }
        
        return base.Enter(isMerged);
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        List<LocalizationString> strings = new List<LocalizationString>();
        switch (_phoneScreenIndex)
        {
            case (int)PhoneBackgroundScreen.BlockScreen:
                strings.Add(_dayLocalizationString);
                strings.Add(_notificationNameLocalizationString);
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