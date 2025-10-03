using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
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
    [SerializeField] private bool _initStartInfo;
    [SerializeField] private bool _blockScreenNotificationKey;
    [SerializeField] private int _startScreenCharacterIndex;
    [SerializeField] private List<ContactInfoToOnlineStatus> _onlineContacts; 
    private PhoneUIHandler _phoneUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    private ReactiveCommand _exitCommand;
    public IReadOnlyList<Phone> Phones { get; private set; }

    public IReadOnlyList<PhoneContactDataLocalizable> PhoneContactDatasLocalizable =>
        Phones[_phoneIndex].PhoneDataLocalizable.PhoneContactDatasLocalizable;
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContactDataLocalizable> contacts, PhoneUIHandler phoneUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler)
    {
        Phones = phones;
        _phoneUIHandler = phoneUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        _exitCommand = new ReactiveCommand();
        if (IsPlayMode() == false)
        {
            CreateDictionaryToChooseOnlineContacts(contacts);
        }
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        SetInfoToView();
        ButtonSwitchSlideUIHandler.DeactivatePushOption();

        await _customizationCurtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
    }

    public override async UniTask Exit()
    {
        await _customizationCurtainUIHandler.CurtainCloses(CancellationTokenSource.Token);
        CancellationTokenSource = null;
        _phoneUIHandler.DisposeScreensBackgrounds();
        ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
    }

    protected override void SetInfoToView()
    {
        _phoneUIHandler.ConstructFromNode(_onlineContacts, Phones[_phoneIndex], SetLocalizationChangeEvent, SwitchToNextNodeEvent, IsPlayMode(),
            _butteryPercent,_startHour, _startMinute);
        switch (_phoneStartScreen)
        {
            case PhoneBackgroundScreen.BlockScreen:
                _phoneUIHandler.SetBlockScreenBackgroundFromNode(_date, _startScreenCharacterIndex, IsPlayMode(), _blockScreenNotificationKey);
                break;
            case PhoneBackgroundScreen.ContactsScreen:
                _phoneUIHandler.SetContactsScreenBackgroundFromNode();
                break;
            case PhoneBackgroundScreen.DialogScreen:
                _phoneUIHandler.SetDialogScreenBackgroundFromNode(_startScreenCharacterIndex);
                break;
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_date};
    }

    private void CreateDictionaryToChooseOnlineContacts(IReadOnlyList<PhoneContactDataLocalizable> contacts)
    {
        Dictionary<string, ContactInfoToOnlineStatus> contactsToChooseOnline = new Dictionary<string, ContactInfoToOnlineStatus>();
        for (int i = 0; i < contacts.Count; i++)
        {
            TryAdd(contacts[i]);
        }
        int count;
        PhoneDataLocalizable dataLocalizable;
        for (int i = 0; i < Phones.Count; i++)
        {
            count = Phones[i].PhoneDataLocalizable.PhoneContactDatasLocalizable.Count;
            dataLocalizable = Phones[i].PhoneDataLocalizable;
            for (int j = 0; j < count; j++)
            {
                TryAdd(dataLocalizable.PhoneContactDatasLocalizable[j]);
            }
        }

        if (_onlineContacts != null && _onlineContacts.Count > 0)
        {
            ContactInfoToOnlineStatus oldInfo;
            for (int i = 0; i < _onlineContacts.Count; i++)
            {
                oldInfo = _onlineContacts[i];
                if (contactsToChooseOnline.ContainsKey(oldInfo.Key))
                {
                    var newInfoValue = new ContactInfoToOnlineStatus(oldInfo.Name, oldInfo.Key, oldInfo.OnlineKey);
                    contactsToChooseOnline[oldInfo.Key] = newInfoValue;
                }
            }
        }
        List<ContactInfoToOnlineStatus> contactInfoToOnlineStatus = new List<ContactInfoToOnlineStatus>(contactsToChooseOnline.Count);
        foreach (var pair in contactsToChooseOnline)
        {
            contactInfoToOnlineStatus.Add(new ContactInfoToOnlineStatus(pair.Value.Name, pair.Value.Key, pair.Value.OnlineKey));
        }
        _onlineContacts = contactInfoToOnlineStatus;
        void TryAdd(PhoneContactDataLocalizable phoneContactDataLocalizable, bool statusKey = false)
        {
            if (contactsToChooseOnline.ContainsKey(phoneContactDataLocalizable.NameContactLocalizationString.Key) == false)
            {
                contactsToChooseOnline.Add(
                    phoneContactDataLocalizable.NameContactLocalizationString.Key,
                    new ContactInfoToOnlineStatus(
                        phoneContactDataLocalizable.NameContactLocalizationString.DefaultText,
                        phoneContactDataLocalizable.NameContactLocalizationString.Key,
                        statusKey));
            }
        }
    }
}