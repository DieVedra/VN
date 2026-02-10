using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class BottomPanelUIHandler : ILocalizable
{
    private const int _sublingIndex = 4;
    private readonly ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private readonly ReactiveCommand _languageChanged;
    private readonly ExitButtonUIHandler _exitButtonUIHandler;
    private readonly AdvertisingButtonUIHandler _advertisingButtonUIHandler;
    private BottomPanelView _bottomPanelView;
    private CompositeDisposable _compositeDisposable;
    
    public BottomPanelUIHandler(ConfirmedPanelUIHandler confirmedPanelUIHandler, AdvertisingButtonUIHandler advertisingButtonUIHandler,
        ReactiveCommand languageChanged)
    {
        _confirmedPanelUIHandler = confirmedPanelUIHandler;
        _exitButtonUIHandler = new ExitButtonUIHandler();
        _advertisingButtonUIHandler = advertisingButtonUIHandler;
        _languageChanged = languageChanged;
    }

    public void Shutdown()
    {
        _compositeDisposable?.Clear();
        _advertisingButtonUIHandler.Shutdown();
    }

    public void SetSprite(Sprite icon)
    {
        _bottomPanelView.AdvertisingIcon.sprite = icon;
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new[]
        {
            _exitButtonUIHandler.ButtonText, _exitButtonUIHandler.ExitText, _exitButtonUIHandler.LabelTextToConfirmedPanel,
            _exitButtonUIHandler.TranscriptionTextToConfirmedPanel, _advertisingButtonUIHandler.ButtonText,
            _advertisingButtonUIHandler.AdvertisingButtonText, _advertisingButtonUIHandler.LabelTextToConfirmedPanel,
            _advertisingButtonUIHandler.TranscriptionTextToConfirmedPanel
        };
    }

    public void Init(BottomPanelView bottomPanelView)
    {
        _compositeDisposable = new CompositeDisposable();
        _languageChanged.Subscribe(_=>
        {
            ChangedLanguage();
        }).AddTo(_compositeDisposable);
        _bottomPanelView = bottomPanelView;
        ChangedLanguage();


        ChangedLanguage();
        SubscribeButtons();
    }

    private void ChangedLanguage()
    {
        _bottomPanelView.ShowAdvertisingButtonText.text = _advertisingButtonUIHandler.AdvertisingButtonText;
        _bottomPanelView.GameExitButtonText.text = _exitButtonUIHandler.ExitText;
    }

    private void SubscribeButtons()
    {
        SubscribeGameExitButton();
        SubscribeAdvertisingButton();
        _bottomPanelView.transform.SetSiblingIndex(_sublingIndex);
        _bottomPanelView.gameObject.SetActive(true);
    }

    private void SubscribeAdvertisingButton()
    {
        _bottomPanelView.ShowAdvertisingButton.onClick.AddListener(() =>
        {
            _bottomPanelView.ShowAdvertisingButton.onClick.RemoveAllListeners();
            _confirmedPanelUIHandler.Show(
                _advertisingButtonUIHandler.LabelTextToConfirmedPanel,
                _advertisingButtonUIHandler.TranscriptionTextToConfirmedPanel,
                _advertisingButtonUIHandler.ButtonText, AdvertisingButtonUIHandler.HeightPanel,
                AdvertisingButtonUIHandler.FontSizeValue,
                () => { _advertisingButtonUIHandler.Show(SubscribeAdvertisingButton).Forget(); },
                SubscribeAdvertisingButton,
                true).Forget();
        });
    }

    private void SubscribeGameExitButton()
    {
        _bottomPanelView.GameExitButton.onClick.AddListener(() =>
        {
            _bottomPanelView.GameExitButton.onClick.RemoveAllListeners();
            _confirmedPanelUIHandler.Show(
                _exitButtonUIHandler.LabelTextToConfirmedPanel, _exitButtonUIHandler.TranscriptionTextToConfirmedPanel,
                _exitButtonUIHandler.ButtonText, ExitButtonUIHandler.HeightPanel,
                ExitButtonUIHandler.FontSizeValue,
                _exitButtonUIHandler.Press,
                SubscribeGameExitButton
            ).Forget();
        });
    }
}