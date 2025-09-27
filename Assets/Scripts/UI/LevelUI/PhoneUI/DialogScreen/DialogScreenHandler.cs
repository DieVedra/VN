using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class DialogScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private LocalizationString _contactNameLS;
    private LocalizationString _contactStatusLS = "Онлайн";
    private readonly Image _contactImage;
    private readonly TextMeshProUGUI _contactName;
    private readonly TextMeshProUGUI _contactStatusText;
    private readonly Button _backArrow;
    private readonly GameObject _contactStatus;
    private readonly RectTransform _dialogTransform;
    private CompositeDisposable _compositeDisposableLocalization;

    public DialogScreenHandler(DialogScreenView dialogScreenView, TopPanelHandler topPanelHandler, ReactiveCommand<PhoneBackgroundScreen> switchPanelCommand)
        :base(dialogScreenView.gameObject, topPanelHandler, dialogScreenView.GradientImage, switchPanelCommand, dialogScreenView.ColorTopPanel)
    {
        _contactImage = dialogScreenView.ContactImage;
        _contactStatus = dialogScreenView.ContactStatusGameObject;
        _contactName = dialogScreenView.ContactName;
        _contactStatusText = dialogScreenView.ContactStatus;
        _backArrow = dialogScreenView.BackArrow;
        _dialogTransform = dialogScreenView.DialogTransform;
    }
    public void Enable(PhoneTime phoneTime, PhoneContactDataLocalizable contact,
        SetLocalizationChangeEvent setLocalizationChangeEvent, int butteryPercent, bool playModeKey, bool characterOnlineKey)
    {
        BaseEnable(phoneTime, butteryPercent, playModeKey);
        _contactNameLS = contact.NameContactLocalizationString;
        if (characterOnlineKey)
        {
            _contactStatus.SetActive(true);
        }
        else
        {
            _contactStatus.SetActive(false);
        }
        SetTexts();
        _compositeDisposableLocalization = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
    }
    public override void Disable()
    {
        base.Disable();
        _compositeDisposableLocalization?.Clear();
        // _compositeDisposable?.Clear();
    }
    private void SetTexts()
    {
        _contactName.text = _contactNameLS;
        _contactStatusText.text = _contactStatusLS;
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_contactStatusLS};
    }
}