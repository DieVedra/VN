using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#003C05")]
public class PhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private List<PhoneNodeCase> _phoneNodeCases;
    [SerializeField] private List<PhoneNotification> _notificationsInBlockScreen;
    [SerializeField] private int _phoneIndex;
    // [SerializeField] private PhoneBackgroundScreen _phoneStartScreen;
    [SerializeField] private int _butteryPercent;
    [SerializeField] private int _startHour;
    [SerializeField] private int _startMinute;
    [SerializeField] private LocalizationString _date;
    [SerializeField] private int _startScreenContactIndex;
    [SerializeField] private List<ContactInfoToGame> _contactsInfoToGame;
    [SerializeField] private List<Phone> _phones;
    
    private List<PhoneContact> _allContacts;
    private IReadOnlyList<PhoneContact> _contactsToAddInPlot;
    private const string _port = "Port ";
    private Dictionary<string, ContactInfoToGame> _contactsDictionary;
    private PhoneUIHandler _phoneUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    private int _seriaIndex;
    public IReadOnlyList<Phone> Phones { get; private set; }
    public Phone CurrentPhone => _phones[_phoneIndex];
    public IReadOnlyList<PhoneContact> AllContacts => _allContacts;
    // public IReadOnlyList<PhoneContact> PhoneContactDatasLocalizable =>
    //     Phones[_phoneIndex].PhoneDataLocalizable.PhoneContactDatasLocalizable;
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContact> contactsToAddInPlot,
        PhoneUIHandler phoneUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler, int seriaIndex)
    {
        _phones = phones.ToList();
        Phones = phones;
        _phoneUIHandler = phoneUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        // Contacts = contacts;
        _seriaIndex = seriaIndex;
        // CreateContactsToOnlineAndNotifications(contacts);
        // AllContactsCurrentPhone = contactsToAddInPlot;
        // _allContacts = contactsToAddInPlot;
        _contactsToAddInPlot = contactsToAddInPlot;
        InitAllContacts();
        if (IsPlayMode() == false)
        {
            bool key;
            for (int i = 0; i < _phoneNodeCases.Count; i++)
            {
                key = false;
                for (int j = 0; j < _allContacts.Count; j++)
                {
                    if (_phoneNodeCases[i].ContactKey == _allContacts[j].NameLocalizationString.Key)
                    {
                        key = true;
                        break;
                    }
                }

                if (key == false)
                {
                    Remove(i);
                }
            }
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
        _phoneUIHandler.ConstructFromNode(_contactsDictionary, Phones[_phoneIndex], SetLocalizationChangeEvent, SwitchToNextNodeEvent, IsPlayMode(),
            _seriaIndex, _butteryPercent,_startHour, _startMinute);
        
        
        _phoneUIHandler.SetBlockScreenBackgroundFromNode(_notificationsInBlockScreen, _date, IsPlayMode());
        
        
        // switch (_phoneStartScreen)
        // {
        //     case PhoneBackgroundScreen.BlockScreen:
        //         _phoneUIHandler.SetBlockScreenBackgroundFromNode(_date, _startScreenContactIndex, IsPlayMode());
        //         break;
        //     case PhoneBackgroundScreen.ContactsScreen:
        //         _phoneUIHandler.SetContactsScreenBackgroundFromNode();
        //         break;
        //     case PhoneBackgroundScreen.DialogScreen:
        //         _phoneUIHandler.SetDialogScreenBackgroundFromNode(_startScreenContactIndex);
        //         break;
        // }
    }

    public override void Dispose()
    {
        _contactsDictionary = null;
        base.Dispose();
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_date};
    }
    private void CreateContactsToOnlineAndNotifications(IReadOnlyList<PhoneContact> contacts)
    {
        _contactsDictionary = new Dictionary<string, ContactInfoToGame>();
        for (int i = 0; i < contacts.Count; i++)
        {
            // TryAdd(contacts[i]);
        }
        int count;
        // PhoneDataLocalizable dataLocalizable;
        for (int i = 0; i < Phones.Count; i++)
        {
            // count = Phones[i].PhoneDataLocalizable.PhoneContactDatasLocalizable.Count;
            // dataLocalizable = Phones[i].PhoneDataLocalizable;
            // for (int j = 0; j < count; j++)
            // {
            //     // TryAdd(dataLocalizable.PhoneContactDatasLocalizable[j]);
            // }
        }
        if (_contactsInfoToGame.Count > 0)
        {
            TransferringKeys();
        }
        else
        {
            FillContactsInfoToGame();
        }
    }
    // private void TryAdd(PhoneContactDataLocalizable phoneContactDataLocalizable, bool statusKey = false, bool notificationKey = false)
    // {
    //     if (_contactsDictionary.ContainsKey(phoneContactDataLocalizable.NameContact.Key) == false)
    //     {
    //         _contactsDictionary.Add(
    //             phoneContactDataLocalizable.NameContact.Key,
    //             new ContactInfoToGame(
    //                 phoneContactDataLocalizable.NameContact.Key,
    //                 phoneContactDataLocalizable.NameContact.DefaultText,
    //                 statusKey, notificationKey));
    //     }
    // }
    private void InitAllContacts()
    {
        if (_allContacts == null)
        {
            _allContacts = new List<PhoneContact>();
        } 
        _allContacts.Clear();
        _allContacts.AddRange(CurrentPhone.PhoneContactDatas); 
        _allContacts.AddRange(_contactsToAddInPlot);
    }
    private void TransferringKeys()
    {
        ContactInfoToGame contact;
        for (int i = _contactsInfoToGame.Count -1 ; i >= 0; i--)
        {
            contact = _contactsInfoToGame[i];
            if (_contactsDictionary.TryGetValue(contact.KeyName, out ContactInfoToGame contactFromDictionary))
            {
                contactFromDictionary.KeyNotification = contact.KeyNotification;
                contactFromDictionary.KeyOnline = contact.KeyOnline;
            }
            else
            {
                _contactsInfoToGame.RemoveAt(i);
            }
        }
    }

    private void FillContactsInfoToGame()
    {
        _contactsInfoToGame.Clear();
        foreach (var pair in _contactsDictionary)
        {
            _contactsInfoToGame.Add(pair.Value);
        }
        _contactsInfoToGame.TrimExcess();
    }
    private void Awake()
    {
        if (_contactsInfoToGame == null)
        {
            _contactsInfoToGame = new List<ContactInfoToGame>();
        }
    }

    private void AddCase(int index)
    {
        PhoneContact contact = _allContacts[index];
        if (contact.ToPhoneKey != CurrentPhone.NamePhone.Key)
        {
            return;
        }
        for (int i = 0; i < _phoneNodeCases.Count; i++)
        {
            if (_phoneNodeCases[i].ContactKey == contact.NameLocalizationString.Key)
            {
                return;
            }
        }
        PhoneNodeCase phoneNodeCase = new PhoneNodeCase(contact.NameLocalizationString.Key,
            contact.NameLocalizationString.DefaultText, $"{_port}{DynamicOutputs.Count()}", index);
        _phoneNodeCases.Add(phoneNodeCase);
        
        AddDynamicOutput(typeof(Empty), ConnectionType.Override, fieldName: phoneNodeCase.PortName);
    }
    private void RemoveCase(string key)
    {
        for (int i = 0; i < _phoneNodeCases.Count; i++)
        {
            if (_phoneNodeCases[i].ContactKey == key)
            {
                Remove(i);
            }
        }
    }

    private void Remove(int i)
    {
        RemoveDynamicPort(_phoneNodeCases[i].PortName);
        _phoneNodeCases.RemoveAt(i);
    }

    private void RemoveAllCases()
    {
        for (int i = _phoneNodeCases.Count - 1; i >= 0; i--)
        {
            RemoveDynamicPort(_phoneNodeCases[i].PortName);
        }
        _phoneNodeCases.Clear();
        InitAllContacts();
        _notificationsInBlockScreen.Clear();
    }
}