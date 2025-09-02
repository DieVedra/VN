using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class BottomPanelUIHandler : ILocalizable
{
    private const int _sublingIndex = 4;
    private readonly ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private readonly Transform _parent;
    private readonly ReactiveCommand _languageChanged;
    private readonly ExitButtonUIHandler _exitButtonUIHandler;
    private readonly AdvertisingButtonUIHandler _advertisingButtonUIHandler;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private BottomPanelView _bottomPanelView;
    private CompositeDisposable _compositeDisposable;
    
    public BottomPanelUIHandler(ConfirmedPanelUIHandler confirmedPanelUIHandler, AdvertisingButtonUIHandler advertisingButtonUIHandler,
        Transform parent, ReactiveCommand languageChanged)
    {
        _confirmedPanelUIHandler = confirmedPanelUIHandler;
        _exitButtonUIHandler = new ExitButtonUIHandler();
        _advertisingButtonUIHandler = advertisingButtonUIHandler;
        _parent = parent;
        _languageChanged = languageChanged;
    }

    public void Dispose()
    {
        _compositeDisposable?.Clear();
        _advertisingButtonUIHandler.Dispose();
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

    public void Init(BottomPanelView bottomPanelView, BlackFrameUIHandler darkeningBackgroundFrameUIHandler)
    {
        _compositeDisposable = new CompositeDisposable();
        _languageChanged.Subscribe(_=>
        {
            ChangedLanguage();
        }).AddTo(_compositeDisposable);
        _bottomPanelView = bottomPanelView;
        ChangedLanguage();


        ChangedLanguage();
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        SubscribeButtons();
    }

    private void ChangedLanguage()
    {
        _bottomPanelView.ShowAdvertisingButtonText.text = _advertisingButtonUIHandler.AdvertisingButtonText;
        _bottomPanelView.GameExitButtonText.text = _exitButtonUIHandler.ExitText;
    }

    private void SubscribeButtons()
    {
        _bottomPanelView.GameExitButton.onClick.AddListener(() =>
        {
            _confirmedPanelUIHandler.Show(
                _exitButtonUIHandler.LabelTextToConfirmedPanel, _exitButtonUIHandler.TranscriptionTextToConfirmedPanel,
                _exitButtonUIHandler.ButtonText, ExitButtonUIHandler.HeightPanel,
                ExitButtonUIHandler.FontSizeValue,
                _exitButtonUIHandler.Press).Forget();
        });
        _bottomPanelView.ShowAdvertisingButton.onClick.AddListener(() =>
        {
            
            _confirmedPanelUIHandler.Show(
                _advertisingButtonUIHandler.LabelTextToConfirmedPanel, _advertisingButtonUIHandler.TranscriptionTextToConfirmedPanel,
                _advertisingButtonUIHandler.ButtonText, AdvertisingButtonUIHandler.HeightPanel,
                AdvertisingButtonUIHandler.FontSizeValue,
                () => { _advertisingButtonUIHandler.Press().Forget();},
                true).Forget();
        });
        _bottomPanelView.transform.SetSiblingIndex(_sublingIndex);
        _bottomPanelView.gameObject.SetActive(true);
    }
}