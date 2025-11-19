using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;



//надо сделать что бы в сообщениях было возможность ветвления
//перенести все сообщения в графы для удобного редактирования

//переписки с мамой и мирандой достпны при любом исходе
//переписка с эвином или марком доступна только одна взаимоисключающая
//если есть их оба контакта то при нажатии на эдвина переписки с марком не должно быть
//получается что надо добавить механику взаимоисключения сообщений контактов
[NodeWidth(350),NodeTint("#07B715")]
public class PhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private int _phoneIndex;
    [SerializeField] private PhoneBackgroundScreen _phoneStartScreen;
    [SerializeField] private int _butteryPercent;
    [SerializeField] private int _startHour;
    [SerializeField] private int _startMinute;
    [SerializeField] private LocalizationString _date;
    [SerializeField] private int _startScreenCharacterIndex;
    [SerializeField] private List<ContactInfoToGame> _contactsInfoToGame;
    private Dictionary<string, ContactInfoToGame> _contactsDictionary;
    private PhoneUIHandler _phoneUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;
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
        CreateContactsToOnlineAndNotifications(contacts);
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
            _butteryPercent,_startHour, _startMinute);
        switch (_phoneStartScreen)
        {
            case PhoneBackgroundScreen.BlockScreen:
                _phoneUIHandler.SetBlockScreenBackgroundFromNode(_date, _startScreenCharacterIndex, IsPlayMode());
                break;
            case PhoneBackgroundScreen.ContactsScreen:
                _phoneUIHandler.SetContactsScreenBackgroundFromNode();
                break;
            case PhoneBackgroundScreen.DialogScreen:
                _phoneUIHandler.SetDialogScreenBackgroundFromNode(_startScreenCharacterIndex);
                break;
        }
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
    private void CreateContactsToOnlineAndNotifications(IReadOnlyList<PhoneContactDataLocalizable> contacts)
    {
        _contactsDictionary = new Dictionary<string, ContactInfoToGame>();
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
        if (_contactsInfoToGame.Count > 0)
        {
            TransferringKeys();
        }
        else
        {
            FillContactsInfoToGame();
        }
    }
    private void TryAdd(PhoneContactDataLocalizable phoneContactDataLocalizable, bool statusKey = false, bool notificationKey = false)
    {
        if (_contactsDictionary.ContainsKey(phoneContactDataLocalizable.NameContact.Key) == false)
        {
            _contactsDictionary.Add(
                phoneContactDataLocalizable.NameContact.Key,
                new ContactInfoToGame(
                    phoneContactDataLocalizable.NameContact.Key,
                    phoneContactDataLocalizable.NameContact.DefaultText,
                    statusKey, notificationKey));
        }
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
}