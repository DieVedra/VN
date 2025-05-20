
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SettingsPanelUIHandler
{
    private readonly SettingsPanelAssetProvider _settingsPanelAssetProvider;
    private LocalizationString _localizationTextLabel;
    private LocalizationString _localizationTextLocalizationLabel;
    private LocalizationString _localizationTextSoundLabel;
    private SettingsPanelView _settingsPanelView;
    private SettingPanelChoiceHandler _settingPanelChoiceHandler;
    public Transform Transform => _settingsPanelView.transform;
    public bool AssetIsLoaded { get; private set; }
    public SettingsPanelUIHandler()
    {
        _settingsPanelAssetProvider = new SettingsPanelAssetProvider();
    }
    public async UniTask Init(Transform parent, IReactiveProperty<bool> soundStatus, ILocalizationChanger localizationChanger)
    {
        if (AssetIsLoaded == false)
        {
            _settingsPanelView = await _settingsPanelAssetProvider.CreateSettingsPanel(parent);
            _settingsPanelView.SoundField.Toggle.isOn = soundStatus.Value;
            _settingsPanelView.SoundField.Toggle.onValueChanged.AddListener(_ =>
            {
                soundStatus.Value = _settingsPanelView.SoundField.Toggle.isOn;
            });
            
            _localizationTextLabel = _settingsPanelView.TextPanelLabel.text;
            _localizationTextLocalizationLabel = _settingsPanelView.LocalizationField.Text.text;
            _localizationTextSoundLabel = _settingsPanelView.SoundField.Text.text;
            
            
            _settingPanelChoiceHandler = new SettingPanelChoiceHandler(localizationChanger,
                _settingsPanelView.LocalizationField.LeftButton,
                _settingsPanelView.LocalizationField.RightButton,
                _settingsPanelView.LocalizationField.TextChoice);
            
            
            
            AssetIsLoaded = true;
        }
    }

    public void Dispose()
    {
        if (_settingsPanelView != null)
        {
            Addressables.ReleaseInstance(_settingsPanelView.gameObject);
        }
        _settingsPanelView.SoundField.Toggle.onValueChanged.RemoveAllListeners();
    }
    public void Show(BlackFrameUIHandler blackFrameUIHandler)
    {
        _settingsPanelView.transform.SetAsLastSibling();

        _settingsPanelView.ExitButton.onClick.AddListener(()=>
        {
            Hide(blackFrameUIHandler);
            _settingsPanelView.ExitButton.onClick.RemoveAllListeners();
        });
        _settingsPanelView.TextPanelLabel.text = _localizationTextLabel.DefaultText;
        _settingsPanelView.LocalizationField.Text.text = _localizationTextLocalizationLabel.DefaultText;
        _settingsPanelView.SoundField.Text.text = _localizationTextSoundLabel.DefaultText;
        _settingsPanelView.gameObject.SetActive(true);
    }

    private void Hide(BlackFrameUIHandler blackFrameUIHandler)
    {
        _settingsPanelView.gameObject.SetActive(false);
        blackFrameUIHandler.OpenTranslucent().Forget();
    }
}