
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
    private ILevelLocalizationHandler _levelLocalizationHandler;
    private bool _isInLevel;
    public bool IsInited { get; private set; }
    public RectTransform SettingsButtonRectTransform => _settingsButtonView.transform as RectTransform;
    public SettingsPanelButtonUIHandler(Transform parent, SettingsPanelUIHandler settingsPanelUIHandler, 
        LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler)
    {
        _parent = parent;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _settingsPanelUIHandler = settingsPanelUIHandler;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        IsInited = false;
    }

    public void BaseInit(SettingsButtonView settingsButtonView, 
        IReactiveProperty<bool> soundStatus, ILocalizationChanger localizationChanger, bool activeKey = true)
    {
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
    public void InitInLevel(ILevelLocalizationHandler levelLocalizationHandler)
    {
        _levelLocalizationHandler = levelLocalizationHandler;
        _isInLevel = true;
    }
    public void InitInMenu()
    {
        _isInLevel = false;
    }
    private async UniTask OpenPanel()
    {
        if (_isInLevel == true)
        {
            _settingsPanelUIHandler.InitInLevel(_levelLocalizationHandler);
        }
        else
        {
            _settingsPanelUIHandler.SetInMenuKey();
        }
        if (_settingsPanelUIHandler.AssetIsLoaded == false)
        {
            _settingsPanelUIHandler.Init(_parent, _soundStatus, _localizationChanger).Forget();
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            
            _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
            await UniTask.WaitUntil(() => _settingsPanelUIHandler.AssetIsLoaded == true);

            _loadIndicatorUIHandler.StopIndicate();
        }
        else
        {
            _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
        }
        await _localizationChanger.LoadAllLanguagesForPanels();
        _settingsPanelUIHandler.Show();
    }
}