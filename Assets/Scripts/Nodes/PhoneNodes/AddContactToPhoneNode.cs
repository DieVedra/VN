
using System;
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

    private const string _notificationTextPart = " в контактах";
    private const float _delayDisplayTime = 0.5f;
    private const float _showTime = 2f;
    private LocalizationString _localizationString;
    private NotificationPanelUIHandler _notificationPanelUIHandler;
    private CompositeDisposable _compositeDisposable;

    public IReadOnlyList<Phone> Phones { get; private set; }

    public IReadOnlyList<PhoneContactDataLocalizable> Contacts { get; private set; }

    
    public void ConstructMyAddContactToPhoneNode(IReadOnlyList<Phone> phones, IReadOnlyList<PhoneContactDataLocalizable> contacts,
        NotificationPanelUIHandler notificationPanelUIHandler)
    {
        _notificationPanelUIHandler = notificationPanelUIHandler;
        Phones = phones;
        Contacts = contacts;
        _localizationString = new LocalizationString(_notificationTextPart);
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_localizationString};
    }

    public override UniTask Enter(bool isMerged = false)
    {
        if (_addContact == true)
        {
            Phones[_phoneIndex].AddPhoneData(0, Contacts[_contactIndex]);
            Show().Forget();
        }
        return base.Enter(isMerged);
    }
    protected override void SetInfoToView()
    {
        SetText();
        _notificationPanelUIHandler.SetColorText(Color.white);
    }
    private void SetText()
    {
        _notificationPanelUIHandler.ShowNotificationInEditMode($"{Contacts[_contactIndex].NameContactLocalizationString}{_localizationString}");
    }
    private async UniTaskVoid Show()
    {
        CancellationTokenSource = new CancellationTokenSource();
        _compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetText);
        await UniTask.Delay(TimeSpan.FromSeconds(_delayDisplayTime), cancellationToken: CancellationTokenSource.Token);
        _notificationPanelUIHandler.EmergenceNotificationPanelInPlayMode();
        SetInfoToView();
        await _notificationPanelUIHandler.AnimationPanel.UnfadePanel(CancellationTokenSource.Token);
        await UniTask.Delay(TimeSpan.FromSeconds(_showTime), cancellationToken: CancellationTokenSource.Token);
        await _notificationPanelUIHandler.AnimationPanel.FadePanel(CancellationTokenSource.Token);
        _notificationPanelUIHandler.DisappearanceNotificationPanelInPlayMode();
        _compositeDisposable.Clear();
    }
}