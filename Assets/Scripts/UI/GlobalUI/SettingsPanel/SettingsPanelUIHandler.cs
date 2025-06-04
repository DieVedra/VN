
using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SettingsPanelUIHandler
{
    private LocalizationString _textLabel = "Настройки";
    private LocalizationString _localizationLabel = "Язык:";
    private LocalizationString _soundLabel = "Звук:";
    private SettingsPanelView _settingsPanelView;
    private SettingPanelChoiceHandler _settingPanelChoiceHandler;
    public Transform Transform => _settingsPanelView.transform;
    public ReactiveCommand<bool> SwipeDetectorOff { get; private set; }
    public bool AssetIsLoaded { get; private set; }
    public bool PanelOpen { get; private set; }

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

            await localizationChanger.LoadAllLanguages();
            
            AssetIsLoaded = true;
        }
    }

    public void Dispose()
    {
        _settingsPanelView?.SoundField.Toggle.onValueChanged.RemoveAllListeners();
    }
    public void Show(BlackFrameUIHandler blackFrameUIHandler)
    {
        PanelOpen = true;
        SwipeDetectorOff?.Execute(true);
        _settingsPanelView.transform.SetAsLastSibling();
        LanguageChanged();
        _settingsPanelView.ExitButton.onClick.AddListener(()=>
        {
            Hide(blackFrameUIHandler);
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
    private void Hide(BlackFrameUIHandler blackFrameUIHandler)
    {
        PanelOpen = false;
        SwipeDetectorOff?.Execute(false);
        _settingsPanelView.gameObject.SetActive(false);
        blackFrameUIHandler.OpenTranslucent().Forget();
        _settingsPanelView.transform.parent.gameObject.SetActive(false);
    }
}