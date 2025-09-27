using System.Collections.Generic;
using System.Threading;
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
    [SerializeField] private bool _characterOnlineKey;
    [SerializeField] private int _startScreenCharacterIndex;

    private PhoneUIHandler _phoneUIHandler;
    private CustomizationCurtainUIHandler _customizationCurtainUIHandler;

    public IReadOnlyList<Phone> Phones { get; private set; }

    public IReadOnlyList<PhoneContactDataLocalizable> PhoneContactDatasLocalizable =>
        Phones[_phoneIndex].PhoneDataLocalizable.PhoneContactDatasLocalizable;
    public void ConstructMyPhoneNode(IReadOnlyList<Phone> phones, PhoneUIHandler phoneUIHandler, CustomizationCurtainUIHandler customizationCurtainUIHandler)
    {
        Phones = phones;
        _phoneUIHandler = phoneUIHandler;
        _customizationCurtainUIHandler = customizationCurtainUIHandler;
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
        ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
        _phoneUIHandler.DisposeBackgrounds();
    }

    protected override void SetInfoToView()
    {
        switch (_phoneStartScreen)
        {
            case PhoneBackgroundScreen.BlockScreen:
                _phoneUIHandler.SetBlockScreenBackground(Phones[_phoneIndex], _date, SetLocalizationChangeEvent,
                    _startScreenCharacterIndex, _butteryPercent, _startHour, _startMinute, IsPlayMode(), _blockScreenNotificationKey);
                break;
            case PhoneBackgroundScreen.ContactsScreen:
                _phoneUIHandler.SetContactsScreenBackground(PhoneContactDatasLocalizable, SetLocalizationChangeEvent, _butteryPercent, _startHour, _startMinute, IsPlayMode());
                break;
            case PhoneBackgroundScreen.DialogScreen:
                _phoneUIHandler.SetDialogScreenBackground(PhoneContactDatasLocalizable[_startScreenCharacterIndex],
                    SetLocalizationChangeEvent, _butteryPercent, _startHour, _startMinute, _characterOnlineKey, IsPlayMode());
                break;
        }
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_date};
    }
}