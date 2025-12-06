using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeWidth(350),NodeTint("#003C05")]
public class PhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private List<ContactNodeCase> _phoneNodeCases;
    [SerializeField] private List<ContactInfo> _notificationsInBlockScreen;
    [SerializeField] private List<ContactInfo> _onlineContacts;
    [SerializeField] private int _phoneIndex;
    [SerializeField] private int _butteryPercent;
    [SerializeField] private int _startHour;
    [SerializeField] private int _startMinute;
    [SerializeField] private LocalizationString _date;
    // [SerializeField] private List<Phone> _phones;
    
    private List<PhoneContact> _allContacts;
    private IReadOnlyList<PhoneContact> _contactsToAddInPlot;
    private const string _port = "Port ";
    private PhoneUIHandler _phoneUIHandler;
    private NarrativePanelUIHandler _narrativePanelUI;
    private ChoicePanelUIHandler _choicePanelUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    private int _seriaIndex;
    public IReadOnlyList<Phone> Phones;
    public Phone CurrentPhone => Phones[_phoneIndex];
    public IReadOnlyList<PhoneContact> AllContacts => _allContacts;
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContact> contactsToAddInPlot,
        PhoneUIHandler phoneUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler,
        NarrativePanelUIHandler narrativePanelUI, ChoicePanelUIHandler choicePanelUIHandler,

        int seriaIndex)
    {
        Phones = phones;
        for (int i = 0; i < phones.Count; i++)
        {
            Debug.Log($"ConstructMyPhoneNode {seriaIndex}   {CurrentPhone.PhoneContactDictionary.Count}");
        }

        _phoneUIHandler = phoneUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
        _narrativePanelUI = narrativePanelUI;
        _choicePanelUIHandler = choicePanelUIHandler;
        _seriaIndex = seriaIndex;
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
        
        int siblig = _phoneUIHandler.ConstructFromNode(_phoneNodeCases, _onlineContacts, _notificationsInBlockScreen, Phones[_phoneIndex],
            SetLocalizationChangeEvent, SwitchToNextNodeEvent, _date, IsPlayMode(),
            _seriaIndex, _butteryPercent,_startHour, _startMinute);
        _customizationCurtainUIHandler.SetCurtainUnderTargetPanel(++siblig);
        _choicePanelUIHandler.SetSibling(++siblig);
        _narrativePanelUI.SetSibling(++siblig);
        ButtonSwitchSlideUIHandler.DeactivatePushOption();
        await _customizationCurtainUIHandler.CurtainOpens(CancellationTokenSource.Token);
    }

    public override async UniTask Exit()
    {
        await _customizationCurtainUIHandler.CurtainCloses(CancellationTokenSource.Token);
        CancellationTokenSource = null;
        _phoneUIHandler.DisposeScreensBackgrounds();
        
        _customizationCurtainUIHandler.ResetSibling();
        _narrativePanelUI.ResetSibling();
        _choicePanelUIHandler.ResetSibling();
        
        ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
    }

    // protected override void SetInfoToView()
    // {
    //     _phoneUIHandler.ConstructFromNode(_phoneNodeCases, _onlineContacts, _notificationsInBlockScreen, Phones[_phoneIndex],
    //         SetLocalizationChangeEvent, SwitchToNextNodeEvent, _date, IsPlayMode(),
    //         _seriaIndex, _butteryPercent,_startHour, _startMinute);
    // }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_date};
    }
    private void InitAllContacts()
    {
        if (_allContacts == null)
        {
            _allContacts = new List<PhoneContact>();
        } 
        _allContacts.Clear();
        foreach (var contact in CurrentPhone.PhoneContactDictionary)
        {
            _allContacts.Add(contact.Value);
        }
        _allContacts.AddRange(_contactsToAddInPlot);
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

        string portName = $"{_port}{DynamicOutputs.Count()}";
        AddDynamicOutput(typeof(Empty), ConnectionType.Override, fieldName: portName);
        ContactNodeCase contactNodeCase = new ContactNodeCase(GetPort(portName), index, contact.NameLocalizationString.Key,
            contact.NameLocalizationString.DefaultText, portName);
        _phoneNodeCases.Add(contactNodeCase);
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