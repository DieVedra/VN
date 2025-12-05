using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ContactsScreenHandler : PhoneScreenBaseHandler, ILocalizable
{
    private readonly ContactsShower _contactsShower;
    private readonly ReactiveCommand<PhoneContact> _switchToDialogScreenCommand;
    private readonly PoolBase<ContactView> _contactsPool;
    private LocalizationString _textFindLS = "Люди, группы, сообщения";
    private LocalizationString _textCallsLS = "Звонки";
    private LocalizationString _textExitLS = "Выход";
    private LocalizationString _textContactsLS = "Контакты";
    private LocalizationString _textAddContactLS = "Новый номер";
    private readonly TextMeshProUGUI _textFind;
    private readonly TextMeshProUGUI _textCalls;
    private readonly TextMeshProUGUI _textExit;
    private readonly TextMeshProUGUI _textContacts;
    private readonly Button _buttonExit;
    private readonly CanvasGroup _buttonExitCanvasGroup;
    private SwitchToNextNodeEvent _switchToNextNodeEvent;
    private CompositeDisposable _compositeDisposable;
    private CancellationTokenSource _cancellationTokenSource;
    private IReadOnlyList<ContactNodeCase> _sortedPhoneNodeCases;
    private HashSet<string> _nonReadedContacts;
    public ContactsScreenHandler(ContactsScreenView contactsScreenViewBackground, ContactsShower contactsShower, TopPanelHandler topPanelHandler,
        ReactiveCommand<PhoneContact> switchToDialogScreenCommand, PoolBase<ContactView> contactsPool)
        :base(contactsScreenViewBackground.gameObject, topPanelHandler, contactsScreenViewBackground.ImageBackground,
            contactsScreenViewBackground.ColorTopPanel)
    {
        _buttonExitCanvasGroup = contactsScreenViewBackground.ButtonExitCanvasGroup;
        _contactsShower = contactsShower;
        _switchToDialogScreenCommand = switchToDialogScreenCommand;
        _contactsPool = contactsPool;
        _textFind = contactsScreenViewBackground.TextFind;
        _textCalls = contactsScreenViewBackground.TextCalls;
        _textExit = contactsScreenViewBackground.TextExit;
        _textContacts = contactsScreenViewBackground.TextContacts;
        _buttonExit = contactsScreenViewBackground.ButtonExit;
    }

    public void Init(IReadOnlyList<ContactNodeCase> sortedPhoneNodeCases)
    {
        _sortedPhoneNodeCases = sortedPhoneNodeCases;
        if (_nonReadedContacts == null)
        {
            _nonReadedContacts = new HashSet<string>();
        }
        else
        {
            _nonReadedContacts.Clear();
        }

        for (int i = 0; i < sortedPhoneNodeCases.Count; i++)
        {
            _nonReadedContacts.Add(sortedPhoneNodeCases[i].ContactKey);
        }
    }
    public void Enable(IReadOnlyDictionary<string, PhoneContact> phoneContacts,
        SetLocalizationChangeEvent setLocalizationChangeEvent, SwitchToNextNodeEvent switchToNextNodeEvent)
    {
        _buttonExitCanvasGroup.alpha = AlphaMin;
        _buttonExit.interactable = false;
        Screen.SetActive(true);
        _switchToNextNodeEvent = switchToNextNodeEvent;
        SetTexts();
        TopPanelHandler.SetColorAndMode(TopPanelColor);
        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(SetTexts);
        _contactsShower.Init(phoneContacts, _nonReadedContacts,
            _contactsPool, setLocalizationChangeEvent, _switchToDialogScreenCommand, GetFistLetter, SubscribeButtons);
    }
    public override void Disable()
    {
        base.Disable();
        _contactsShower.Dispose();
        _compositeDisposable?.Clear();
        _cancellationTokenSource?.Cancel();
    }
    private void SubscribeButtons()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _buttonExit.interactable = true;
        _buttonExit.onClick.AddListener(() =>
        {
            _buttonExit.onClick.RemoveAllListeners();
            _switchToNextNodeEvent.Execute();
        });
        _buttonExitCanvasGroup.DOFade(AlphaMax, Duration).SetLoops(LoopsCount, LoopType.Yoyo)
            .WithCancellation(_cancellationTokenSource.Token);
    }
    private void SetTexts()
    {
        _textFind.text = _textFindLS;
        _textCalls.text = _textCallsLS;
        _textExit.text = _textExitLS;
        _textContacts.text = _textContactsLS;
    }

    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[] {_textFindLS, _textCallsLS, _textExitLS, _textContactsLS, _textAddContactLS};
    }
}