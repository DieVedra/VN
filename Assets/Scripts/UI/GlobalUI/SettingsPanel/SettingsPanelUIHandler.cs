using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class SettingsPanelUIHandler : ILocalizable
{
    private LocalizationString _textLabel = "Настройки";
    private LocalizationString _localizationLabel = "Язык:";
    private LocalizationString _soundLabel = "Звук:";
    private SettingsPanelView _settingsPanelView;
    private SettingPanelChoiceHandler _settingPanelChoiceHandler;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandle;
    private ILevelLocalizationHandler _levelLocalizationHandler;
    private CompositeDisposable _hideOnSwitchLevelLocalizationCompositeDisposable;
    public Transform Transform => _settingsPanelView.transform;
    public ReactiveCommand<bool> SwipeDetectorOffReactiveCommand { get; }
    public ReactiveCommand LanguageChangedReactiveCommand { get; }
    public bool AssetIsLoaded { get; private set; }
    public bool PanelOpen { get; private set; }
    public bool IsInLevel { get; private set; }

    public SettingsPanelUIHandler(ReactiveCommand languageChanged, ReactiveCommand<bool> swipeDetectorOffReactiveCommand)
    {
        SwipeDetectorOffReactiveCommand = swipeDetectorOffReactiveCommand;
        LanguageChangedReactiveCommand = languageChanged;
        languageChanged.Subscribe(_ =>
        {
            LanguageChanged();
        });
    }
    public async UniTask Init(Transform parent, IReactiveProperty<bool> soundStatus,
        ILocalizationChanger localizationChanger)
    {
        if (AssetIsLoaded == false)
        {
            _settingsPanelView = await new SettingsPanelAssetProvider().CreateSettingsPanel(parent);
            _settingsPanelView.SoundField.Toggle.isOn = soundStatus.Value;
            _settingsPanelView.SoundField.Toggle.onValueChanged.AddListener(_ =>
            {
                soundStatus.Value = _settingsPanelView.SoundField.Toggle.isOn;
            });
            _settingPanelChoiceHandler = new SettingPanelChoiceHandler(localizationChanger,
                _settingsPanelView.LocalizationField.LeftButton,
                _settingsPanelView.LocalizationField.RightButton,
                _settingsPanelView.LocalizationField.TextChoice);
            
            AssetIsLoaded = true;
        } 
    }
    public IReadOnlyList<LocalizationString> GetLocalizableContent()
    {
        return new LocalizationString[] {_textLabel, _localizationLabel, _soundLabel};
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
    public void Show(BlackFrameUIHandler blackFrameUIHandler, LoadIndicatorUIHandler loadIndicatorUIHandler)
    {
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadIndicatorUIHandle = loadIndicatorUIHandler;
        PanelOpen = true;
        SwipeDetectorOffReactiveCommand?.Execute(true);
        _settingsPanelView.transform.SetAsLastSibling();
        LanguageChanged();
        _settingsPanelView.ExitButton.onClick.AddListener(()=>
        {
            Hide();
            _settingsPanelView.ExitButton.onClick.RemoveAllListeners();
        });
        _settingsPanelView.transform.parent.gameObject.SetActive(true);
        _settingsPanelView.gameObject.SetActive(true);
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
        _settingsPanelView.gameObject.SetActive(false);
        if (IsInLevel == false)
        {
            HideDefault();
        }
        else
        {
            HideOnSwitchLevelLocalizationPart1();
        }
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