using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeTint("#207CCB")]
public class AddContactToPhoneNode : BaseNode, ILocalizable
{
    [SerializeField] private int _phoneIndex;
    [SerializeField] private int _contactIndex;
    [SerializeField] private bool _addContact;
    [SerializeField] private bool _showNotificationKey;
    private const string _notificationTextPart = " в контактах";
    private LocalizationString _localizationString;
    private NotificationPanelUIHandler _notificationPanelUIHandler;
    private CompositeDisposable _compositeDisposable;
    private int _currentSeria;
    public IReadOnlyList<Phone> Phones { get; private set; }

    public IReadOnlyList<PhoneContactDataLocalizable> Contacts { get; private set; }

    
    public void ConstructMyAddContactToPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContactDataLocalizable> contacts,
        NotificationPanelUIHandler notificationPanelUIHandler, int currentSeria)
    {
        _notificationPanelUIHandler = notificationPanelUIHandler;
        Phones = phones;
        Contacts = contacts;
        _currentSeria = currentSeria;
        _localizationString = new LocalizationString(_notificationTextPart);
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] { _localizationString};
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        if (_addContact == true)
        {
            Phones[_phoneIndex].AddPhoneData(_currentSeria, Contacts[_contactIndex]);
            if (_showNotificationKey)
            {
                _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(
                    () => { _notificationPanelUIHandler.SetText(GetText());});
                CancellationTokenSource = new CancellationTokenSource();
                _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode(GetText(), CancellationTokenSource.Token, false, _compositeDisposable).Forget();
            }
        }
    }
    protected override void SetInfoToView()
    {
        _notificationPanelUIHandler.ShowNotificationInEditMode(GetText());
    }

    private string GetText()
    {
        return $"{Contacts[_contactIndex].NikNameContact}{_localizationString}";
    }
}