using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class SettingsPanelUIHandler : ILocalizable
{
    private LocalizationString _textLabel = "Настройки";
    private LocalizationString _localizationLabel = "Язык:";
    private LocalizationString _soundLabel = "Звук:";
    private StoryLabelsProvider _storyLabelsProvider = new StoryLabelsProvider();
    private SettingsPanelView _settingsPanelView;
    private SettingPanelChoiceHandler _settingPanelChoiceHandler;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandle;
    private readonly bool _ccdStatus;
    private CashCleaner _cashCleaner;
    private GlobalCanvasCloser _globalCanvasCloser;
    private ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private SaveServiceProvider _saveServiceProvider;
    private StoriesProvider _storiesProvider;
    private ILevelLocalizationHandler _levelLocalizationHandler;
    private IReactiveProperty<bool> _soundStatus;
    private CompositeDisposable _hideOnSwitchLevelLocalizationCompositeDisposable;
    public Transform Transform => _settingsPanelView.transform;
    public ReactiveCommand<bool> SwipeDetectorOffReactiveCommand { get; }
    public ReactiveCommand LanguageChangedReactiveCommand { get; }
    public bool AssetIsLoaded { get; private set; }
    public bool PanelOpen { get; private set; }
    public bool IsInLevel { get; private set; }

    public SettingsPanelUIHandler(BlackFrameUIHandler blackFrameUIHandler, LoadIndicatorUIHandler loadIndicatorUIHandler,
        ReactiveCommand languageChanged, ReactiveCommand<bool> swipeDetectorOffReactiveCommand, bool CCDStatus)
    {
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadIndicatorUIHandle = loadIndicatorUIHandler;
        _ccdStatus = CCDStatus;
        SwipeDetectorOffReactiveCommand = swipeDetectorOffReactiveCommand;
        LanguageChangedReactiveCommand = languageChanged;
        languageChanged.Subscribe(_ =>
        {
            LanguageChanged();
        });
    }
    public async UniTask Init(Transform parent, GlobalCanvasCloser globalCanvasCloser, CashCleaner cashCleaner, ConfirmedPanelUIHandler confirmedPanelUIHandler,
        SaveServiceProvider saveServiceProvider, StoriesProvider storiesProvider, 
        IReactiveProperty<bool> soundStatus, ILocalizationChanger localizationChanger)
    {
        if (AssetIsLoaded == false)
        {
            _settingsPanelView = await new SettingsPanelAssetProvider().CreateSettingsPanel(parent);
            _globalCanvasCloser = globalCanvasCloser;
            _soundStatus = soundStatus;
            _settingPanelChoiceHandler = new SettingPanelChoiceHandler(localizationChanger,
                _settingsPanelView.LocalizationField.LeftButton,
                _settingsPanelView.LocalizationField.RightButton,
                _settingsPanelView.LocalizationField.TextChoice);
            _cashCleaner = cashCleaner;
            _confirmedPanelUIHandler = confirmedPanelUIHandler;
            _saveServiceProvider = saveServiceProvider;
            _storiesProvider = storiesProvider;
            AssetIsLoaded = true;
        } 
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new []
        {
            _textLabel, _localizationLabel, _soundLabel, _storyLabelsProvider.ProgressLabelTextToConfirmedPanel,
            _storyLabelsProvider.ProgressQuestionTextToConfirmedPanel
        };
    }
    public void InitInLevel(ILevelLocalizationHandler levelLocalizationHandler)
    {
        IsInLevel = true;
        _levelLocalizationHandler = levelLocalizationHandler;
    }
    public void SetInMenuKey()
    {
        IsInLevel = false;
    }
    public void Shutdown()
    {
        _hideOnSwitchLevelLocalizationCompositeDisposable?.Clear();
        _settingsPanelView?.SoundField.Toggle.onValueChanged.RemoveAllListeners();
    }
    public void Show()
    {
        PanelOpen = true;
        SwipeDetectorOffReactiveCommand?.Execute(true);
        _settingsPanelView.transform.SetAsLastSibling();
        LanguageChanged();
        _settingPanelChoiceHandler.Init();
        TrySubscribeClearCashButton();
        TryClearProgressLevels();
        
        
        
        _settingsPanelView.SoundField.Toggle.isOn = _soundStatus.Value;
        _settingsPanelView.SoundField.Toggle.onValueChanged.AddListener(_ =>
        {
            _soundStatus.Value = _settingsPanelView.SoundField.Toggle.isOn;
        });
        
        _settingsPanelView.ExitButton.onClick.AddListener(Hide);
        _settingsPanelView.transform.parent.gameObject.SetActive(true);
        _settingsPanelView.gameObject.SetActive(true);
        _globalCanvasCloser.TryEnable();
    }

    private void TryClearProgressLevels()
    {
        _settingsPanelView.ClearProgressButton.onClick.AddListener(() =>
        {
            _confirmedPanelUIHandler.Show(_storyLabelsProvider.ProgressLabelTextToConfirmedPanel,
                _storyLabelsProvider.ProgressQuestionTextToConfirmedPanel, _confirmedPanelUIHandler.ConfirmedButtonText,
                StoryLabelsProvider.HeightPanel, StoryLabelsProvider.FontSizeValue, () =>
                {
                    foreach (var story in _storiesProvider.Stories)
                    {
                        story.ResetProgress();
                    }
                }, TryClearProgressLevels).Forget();
        });
    }

    private void TrySubscribeClearCashButton()
    {
        if (_ccdStatus)
        {
            _settingsPanelView.ClearCashButton.gameObject.SetActive(true);
            _settingsPanelView.ClearCashButton.onClick.AddListener(() =>
            {
                _settingsPanelView.ClearCashButton.onClick.RemoveAllListeners();
                _confirmedPanelUIHandler.Show(_cashCleaner.CashLabelTextToConfirmedPanel,
                    _cashCleaner.CashQuestionTextToConfirmedPanel, _confirmedPanelUIHandler.ConfirmedButtonText,
                    CashCleaner.HeightPanel, CashCleaner.FontSizeValue, () => { _cashCleaner.CleanAllCash(); },
                    TrySubscribeClearCashButton).Forget();
            });
        }
        else
        {
            _settingsPanelView.ClearCashButton.gameObject.SetActive(false);
        }
    }

    private void LanguageChanged()
    {
        _settingsPanelView.TextPanelLabel.text = _textLabel;
        _settingsPanelView.LocalizationField.Text.text = _localizationLabel;
        _settingsPanelView.SoundField.Text.text = _soundLabel;
    }
    private void Hide()
    {
        PanelOpen = false;
        _settingPanelChoiceHandler.Shutdown();
        _settingsPanelView.ExitButton.onClick.RemoveAllListeners();
        _settingsPanelView.SoundField.Toggle.onValueChanged.RemoveAllListeners();
        _settingsPanelView.gameObject.SetActive(false);
        if (IsInLevel == false)
        {
            HideDefault();
        }
        else
        {
            HideOnSwitchLevelLocalizationPart1();
        }
        _globalCanvasCloser.TryDisable();
    }

    private void HideDefault()
    {
        _blackFrameUIHandler.OpenTranslucent().Forget();
        _settingsPanelView.transform.parent.gameObject.SetActive(false);
        SwipeDetectorOffReactiveCommand?.Execute(false);
    }

    private void HideOnSwitchLevelLocalizationPart1()
    {
        if (_levelLocalizationHandler.IsLocalizationHasBeenChanged() == false)
        {
            HideDefault();
            return;
        }
        _loadIndicatorUIHandle.SetLocalizationIndicate();
        _loadIndicatorUIHandle.StartIndicate();
        _hideOnSwitchLevelLocalizationCompositeDisposable = new CompositeDisposable();
        _levelLocalizationHandler.OnEndSwitchLocalization.Subscribe(_ =>
        {
            _hideOnSwitchLevelLocalizationCompositeDisposable.Clear();
            HideOnSwitchLevelLocalizationPart2();
        }).AddTo(_hideOnSwitchLevelLocalizationCompositeDisposable);
        _levelLocalizationHandler.TrySwitchLanguageFromSettingsChange().Forget();
    }

    private void HideOnSwitchLevelLocalizationPart2()
    {
        _loadIndicatorUIHandle.StopIndicate();
        HideDefault();
    }
}