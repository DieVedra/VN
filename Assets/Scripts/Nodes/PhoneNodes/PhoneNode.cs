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
    [SerializeField] private bool _blockScreenNotificationKey;
    [SerializeField] private int _startScreenCharacterIndex;
    [SerializeField] private List<string> _onlineContacts;
    [SerializeField] private List<string> _notificationContacts;
    private PhoneUIHandler _phoneUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    // private ReactiveCommand _exitCommand;
    public IReadOnlyList<Phone> Phones { get; private set; }
    public IReadOnlyList<PhoneContactDataLocalizable> Contacts { get; private set; }
    public IReadOnlyList<PhoneContactDataLocalizable> PhoneContactDatasLocalizable =>
        Phones[_phoneIndex].PhoneDataLocalizable.PhoneContactDatasLocalizable;
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContactDataLocalizable> contacts,
        PhoneUIHandler phoneUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler)
    {
        Phones = phones;
        _phoneUIHandler = phoneUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        Contacts = contacts;
        // _exitCommand = new ReactiveCommand();
        // if (IsPlayMode() == false)
        // {
        //     CreateContactsToOnlineAndNotifications(contacts);
        // }
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
    private void Awake()
    {
        Debug.Log($"Awake()");

    }

    private void OnDestroy()
    {
        Debug.Log($"OnDestroy()");
    }
    // private void CreateContactsToOnlineAndNotifications(IReadOnlyList<PhoneContactDataLocalizable> contacts)
    // {
    //     Dictionary<string, ContactInfoToGame> contactsDictionary = new Dictionary<string, ContactInfoToGame>();
    //     for (int i = 0; i < contacts.Count; i++)
    //     {
    //         TryAdd(contacts[i]);
    //     }
    //     int count;
    //     PhoneDataLocalizable dataLocalizable;
    //     for (int i = 0; i < Phones.Count; i++)
    //     {
    //         count = Phones[i].PhoneDataLocalizable.PhoneContactDatasLocalizable.Count;
    //         dataLocalizable = Phones[i].PhoneDataLocalizable;
    //         for (int j = 0; j < count; j++)
    //         {
    //             TryAdd(dataLocalizable.PhoneContactDatasLocalizable[j]);
    //         }
    //     }
    //
    //     if (_onlineContacts != null)
    //     {
    //         TransferringKeys(_onlineContacts);
    //     }
    //
    //     if (_notificationContacts != null)
    //     {
    //         TransferringKeys(_notificationContacts);
    //
    //     }
    //     void TryAdd(PhoneContactDataLocalizable phoneContactDataLocalizable, bool statusKey = false)
    //     {
    //         if (contactsDictionary.ContainsKey(phoneContactDataLocalizable.NameContactLocalizationString.Key) == false)
    //         {
    //             contactsDictionary.Add(
    //                 phoneContactDataLocalizable.NameContactLocalizationString.Key,
    //                 new ContactInfoToGame(
    //                     phoneContactDataLocalizable.NameContactLocalizationString.DefaultText,
    //                     phoneContactDataLocalizable.NameContactLocalizationString.Key,
    //                     statusKey));
    //         }
    //     }
    //
    //     void TransferringKeys(List<ContactInfoToGame> from)
    //     {
    //         for (int i = 0; i < from.Count; i++)
    //         {
    //             if (contactsDictionary.ContainsKey(from[i].KeyName))
    //             {
    //                 var newInfoValue = new ContactInfoToGame(from[i].Name, from[i].KeyName, from[i].OnlineKey);
    //                 contactsDictionary[from[i].KeyName] = newInfoValue;
    //             }
    //         }
    //         from.Clear();
    //         foreach (var pair in contactsDictionary)
    //         {
    //             from.Add(new ContactInfoToGame(pair.Value.Name, pair.Value.KeyName, pair.Value.OnlineKey));
    //         }
    //         from.TrimExcess();
    //     }
    // }
}