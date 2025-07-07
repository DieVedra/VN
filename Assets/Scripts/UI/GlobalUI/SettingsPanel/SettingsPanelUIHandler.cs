using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class SettingsPanelUIHandler
{
    private LocalizationString _textLabel = "Настройки";
    private LocalizationString _localizationLabel = "Язык:";
    private LocalizationString _soundLabel = "Звук:";
    private SettingsPanelView _settingsPanelView;
    private SettingPanelChoiceHandler _settingPanelChoiceHandler;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandle;
    private ILevelLocalizationHandler _levelLocalizationHandler;
    public Transform Transform => _settingsPanelView.transform;
    public ReactiveCommand<bool> SwipeDetectorOff { get; }
    public bool AssetIsLoaded { get; private set; }
    public bool PanelOpen { get; private set; }
    public bool IsInLevel { get; private set; }
    public event Action OnPanelClosed;

    public SettingsPanelUIHandler(ReactiveCommand languageChanged, ReactiveCommand<bool> swipeDetectorOff)
    {
        SwipeDetectorOff = swipeDetectorOff;
        languageChanged.Subscribe(_ =>
        {
            LanguageChanged();
        });
    }
    public async UniTask Init(Transform parent, IReactiveProperty<bool> soundStatus,
        ILocalizationChanger localizationChanger)
    {
        IsInLevel = false;
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

            await localizationChanger.LoadAllLanguagesForMenu();
            
            AssetIsLoaded = true;
        }
    }

    public void InitInLevel(ILevelLocalizationHandler levelLocalizationHandler)
    {
        IsInLevel = true;
        _levelLocalizationHandler = levelLocalizationHandler;
    }
    public void Dispose()
    {
        _settingsPanelView?.SoundField.Toggle.onValueChanged.RemoveAllListeners();
    }
    public void Show(BlackFrameUIHandler blackFrameUIHandler, LoadIndicatorUIHandler loadIndicatorUIHandler)
    {
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadIndicatorUIHandle = loadIndicatorUIHandler;
        PanelOpen = true;
        SwipeDetectorOff?.Execute(true);
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
        SwipeDetectorOff?.Execute(false);
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
        _levelLocalizationHandler.Test().Forget();
        _levelLocalizationHandler.OnEndLoadLocalization += HideOnSwitchLevelLocalizationPart2;
    }

    private void HideOnSwitchLevelLocalizationPart2()
    {
        _loadIndicatorUIHandle.StopIndicate();
        HideDefault();
        _levelLocalizationHandler.OnEndLoadLocalization -= HideOnSwitchLevelLocalizationPart2;
    }
}