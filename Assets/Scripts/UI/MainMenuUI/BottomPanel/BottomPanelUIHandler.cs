using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class BottomPanelUIHandler : ILocalizable
{
    private const int _sublingIndex = 4;
    private readonly ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private readonly ReactiveCommand _languageChanged;
    private readonly bool _advertisementStatus;
    private readonly ExitButtonUIHandler _exitButtonUIHandler;
    private readonly AdvertisingButtonUIHandler _advertisingButtonUIHandler;
    private BottomPanelView _bottomPanelView;
    private CompositeDisposable _compositeDisposable;
    
    public BottomPanelUIHandler(ConfirmedPanelUIHandler confirmedPanelUIHandler, AdvertisingButtonUIHandler advertisingButtonUIHandler,
        ReactiveCommand languageChanged, bool advertisementStatus)
    {
        _confirmedPanelUIHandler = confirmedPanelUIHandler;
        _exitButtonUIHandler = new ExitButtonUIHandler();
        _advertisingButtonUIHandler = advertisingButtonUIHandler;
        _languageChanged = languageChanged;
        _advertisementStatus = advertisementStatus;
    }

    public void Shutdown()
    {
        _compositeDisposable?.Clear();
        _bottomPanelView.ShowAdvertisingButton.onClick.RemoveAllListeners();
    }

    public void SetSprite(Sprite icon)
    {
        _bottomPanelView.AdvertisingIcon.sprite = icon;
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        if (_advertisementStatus)
        {
            return new[]
            {
                _exitButtonUIHandler.ButtonText, _exitButtonUIHandler.ExitText, _exitButtonUIHandler.LabelTextToConfirmedPanel,
                _exitButtonUIHandler.TranscriptionTextToConfirmedPanel, _advertisingButtonUIHandler.ButtonText,
                _advertisingButtonUIHandler.AdvertisingButtonText, _advertisingButtonUIHandler.LabelTextToConfirmedPanel,
                _advertisingButtonUIHandler.TranscriptionTextToConfirmedPanel
            };
        }
        else
        {
            return new[] {_exitButtonUIHandler.ButtonText, _exitButtonUIHandler.ExitText, _exitButtonUIHandler.LabelTextToConfirmedPanel,
                _exitButtonUIHandler.TranscriptionTextToConfirmedPanel};
        }
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
        if (_advertisementStatus)
        {
            _bottomPanelView.ShowAdvertisingButtonText.text = _advertisingButtonUIHandler.AdvertisingButtonText;
        }

        _bottomPanelView.GameExitButtonText.text = _exitButtonUIHandler.ExitText;
    }

    private void SubscribeButtons()
    {
        SubscribeGameExitButton();
        TrySubscribeAdvertisingButton();
        _bottomPanelView.transform.SetSiblingIndex(_sublingIndex);
        _bottomPanelView.gameObject.SetActive(true);
    }

    private void TrySubscribeAdvertisingButton()
    {
        if (_advertisementStatus)
        { 
            _bottomPanelView.ShowAdvertisingButton.gameObject.SetActive(true);
            _bottomPanelView.ShowAdvertisingButton.onClick.AddListener(() =>
            {
                _bottomPanelView.ShowAdvertisingButton.onClick.RemoveAllListeners();
                _confirmedPanelUIHandler.Show(
                    _advertisingButtonUIHandler.LabelTextToConfirmedPanel,
                    _advertisingButtonUIHandler.TranscriptionTextToConfirmedPanel,
                    _advertisingButtonUIHandler.ButtonText, AdvertisingButtonUIHandler.HeightPanel,
                    AdvertisingButtonUIHandler.FontSizeValue,
                    () => { _advertisingButtonUIHandler.Show(TrySubscribeAdvertisingButton).Forget(); },
                    TrySubscribeAdvertisingButton,
                    true).Forget();
            });
        }
        else
        {
            _bottomPanelView.ShowAdvertisingButton.gameObject.SetActive(false);
        }
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