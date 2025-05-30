
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelButtonUIHandler
{
    private const int _sublingIndex = 1;
    private Button _settingsButton;
    private SettingsPanelUIHandler _settingsPanelUIHandler;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private SettingsButtonView _settingsButtonView;
    private Transform _parent;
    private IReactiveProperty<bool> _soundStatus;
    private ILocalizationChanger _localizationChanger;
    public bool AssetIsLoaded { get; private set; }
    public SettingsPanelButtonUIHandler(Transform parent, SettingsPanelUIHandler settingsPanelUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        LoadIndicatorUIHandler loadIndicatorUIHandler)
    {
        _parent = parent;
        _settingsPanelUIHandler = settingsPanelUIHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        AssetIsLoaded = false;
    }

    public void Init(SettingsButtonView settingsButtonView, IReactiveProperty<bool> soundStatus, ILocalizationChanger localizationChanger)
    {
        if (AssetIsLoaded == false)
        {
            _settingsButtonView = settingsButtonView;
            _settingsButton = settingsButtonView.Button;
            _soundStatus = soundStatus;
            _localizationChanger = localizationChanger;
            AssetIsLoaded = true;
        }

        SubscribeButtonAndActivate();
    }

    private void SubscribeButtonAndActivate()
    {
        _settingsButton.transform.SetSiblingIndex(_sublingIndex);
        _settingsButton.gameObject.SetActive(true);
        _settingsButton.onClick.AddListener(() =>
        {
            OpenPanel().Forget();
        });
    }

    public async UniTask OpenPanel()
    {
        if (_settingsPanelUIHandler.AssetIsLoaded == false)
        {
            _settingsPanelUIHandler.Init(_parent, _soundStatus, _localizationChanger).Forget();
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            
            _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
            await UniTask.WaitUntil(() => _settingsPanelUIHandler.AssetIsLoaded == true);

            _loadIndicatorUIHandler.StopIndicate();
            _settingsPanelUIHandler.Show(_darkeningBackgroundFrameUIHandler);
        }
        else
        {
            _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
            _settingsPanelUIHandler.Show(_darkeningBackgroundFrameUIHandler);
        }
        
    }
}