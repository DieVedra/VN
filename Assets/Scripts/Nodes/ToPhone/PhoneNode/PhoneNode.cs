using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using XNode;

[NodeWidth(350),NodeTint("#003C05")]
public class PhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private List<ContactNodeCase> _phoneNodeCases;
    [SerializeField] private List<NotificationContactInfo> _notificationsInBlockScreen;
    [SerializeField] private List<OnlineContactInfo> _onlineContacts;
    [SerializeField] private int _phoneIndex;
    [SerializeField] private int _butteryPercent;
    [SerializeField] private int _startHour;
    [SerializeField] private int _startMinute;
    [SerializeField] private LocalizationString _date;
    // [SerializeField] public List<Phone> Phones;

    public const string Port = "Port ";
    private List<PhoneContact> _allContacts;
    private IReadOnlyList<PhoneContact> _contactsToAddInPlot;
    private PhoneUIHandler _phoneUIHandler;
    private NarrativePanelUIHandler _narrativePanelUI;
    private ChoicePanelUIHandler _choicePanelUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
    private int _seriaIndex;
    private bool _curtainUIHandlerRaycastTargetKey;
    private IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> _customizableCharacterIndexesCustodians;
    public IReadOnlyList<Phone> Phones;
    public Phone CurrentPhone => Phones[_phoneIndex];
    public IReadOnlyList<PhoneContact> AllContacts => _allContacts;
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContact> contactsToAddInPlot,
        IReadOnlyDictionary<string, CustomizableCharacterIndexesCustodian> customizableCharacterIndexesCustodians,
        PhoneUIHandler phoneUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler,
        NarrativePanelUIHandler narrativePanelUI, ChoicePanelUIHandler choicePanelUIHandler, int seriaIndex)
    {
        _customizableCharacterIndexesCustodians = customizableCharacterIndexesCustodians;
        Phones = phones.ToList();
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
            for (int i = _phoneNodeCases.Count - 1; i >= 0; i--)
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
                else
                {
                    _phoneNodeCases[i].Port = GetPort(_phoneNodeCases[i].PortName);
                }
            }
        }
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        _curtainUIHandlerRaycastTargetKey = _customizationCurtainUIHandler.CurtainImage.raycastTarget;
        int siblig = _phoneUIHandler.ConstructFromNode(_phoneNodeCases, _onlineContacts, _notificationsInBlockScreen, 
            Phones[_phoneIndex], SetLocalizationChangeEvent, SwitchToNextNodeEvent, _date, IsPlayMode(),
            _seriaIndex, _butteryPercent,_startHour, _startMinute, GetIndexBodyCustomizableCharacter(Phones[_phoneIndex].ToCharacterNameKey));
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
        _customizationCurtainUIHandler.CurtainImage.raycastTarget = _curtainUIHandlerRaycastTargetKey;

        _phoneUIHandler.ShutdownScreensBackgrounds();
        
        _customizationCurtainUIHandler.ResetSibling();
        _narrativePanelUI.ResetSibling();
        _choicePanelUIHandler.ResetSibling();
        ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
        
    }

    public override void Shutdown()
    {
        base.Shutdown();
        for (int i = 0; i < _phoneNodeCases.Count; i++)
        {
            _phoneNodeCases[i].IsReaded = false;
        }
    }

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
    private int GetIndexBodyCustomizableCharacter(string nameKey)
    {
        if (_customizableCharacterIndexesCustodians.TryGetValue(nameKey, out CustomizableCharacterIndexesCustodian value))
        {
            return value.BodyIndexRP.Value;
        }
        else
        {
            return 0;
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

        string portName = $"{Port}{DynamicOutputs.Count()}";
        AddDynamicOutput(typeof(Empty), ConnectionType.Override, fieldName: portName);
        ContactNodeCase contactNodeCase = new ContactNodeCase(GetPort(portName), index, contact.NameLocalizationString.Key,
            contact.NameLocalizationString.DefaultText, portName);
        _phoneNodeCases.Add(contactNodeCase);
    }

    private void AddPortToNotificationContactInfo(string portName, int index)
    {
        AddDynamicOutput(typeof(Empty), Node.ConnectionType.Override, fieldName: portName);
        _notificationsInBlockScreen[index].Port = GetOutputPort(portName);
        var a = _notificationsInBlockScreen[index].ContactKey;
        for (int i = 0; i < _allContacts.Count; i++)
        {
            if (_allContacts[i].NameLocalizationString.Key == _notificationsInBlockScreen[index].ContactKey)
            {
                _notificationsInBlockScreen[index].Contact = _allContacts[i];
            }
        }
    }
    private void RemovePortFromNotificationContactInfo(string portName)
    {
        RemoveDynamicPort(portName);
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

    private void Remove(int i)
    {
        RemoveDynamicPort(_phoneNodeCases[i].PortName);
        _phoneNodeCases.RemoveAt(i);
    }
}