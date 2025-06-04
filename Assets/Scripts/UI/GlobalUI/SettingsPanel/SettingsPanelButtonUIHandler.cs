
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelButtonUIHandler
{
    private Button _settingsButton;
    private SettingsPanelUIHandler _settingsPanelUIHandler;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private SettingsButtonView _settingsButtonView;
    private Transform _parent;
    private IReactiveProperty<bool> _soundStatus;
    private ILocalizationChanger _localizationChanger;
    public bool IsInited { get; private set; }
    public SettingsPanelButtonUIHandler(Transform parent, SettingsPanelUIHandler settingsPanelUIHandler, 
        LoadIndicatorUIHandler loadIndicatorUIHandler)
    {
        _parent = parent;
        _settingsPanelUIHandler = settingsPanelUIHandler;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        IsInited = false;
    }

    public void Init(SettingsButtonView settingsButtonView, BlackFrameUIHandler darkeningBackgroundFrameUIHandler, IReactiveProperty<bool> soundStatus, ILocalizationChanger localizationChanger, bool activeKey = true)
    {
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        if (IsInited == false)
        {
            _settingsButtonView = settingsButtonView;
            _settingsButton = settingsButtonView.Button;
            _soundStatus = soundStatus;
            _localizationChanger = localizationChanger;
            IsInited = true;
            _settingsButton.gameObject.SetActive(activeKey);
            _settingsButton.onClick.AddListener(() =>
            {
                OpenPanel().Forget();
            });
        }
    }

    private async UniTask OpenPanel()
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