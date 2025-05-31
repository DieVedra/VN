
using System;
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
    private bool OnePress;
    public bool IsInited { get; private set; }
    public SettingsPanelButtonUIHandler(Transform parent, SettingsPanelUIHandler settingsPanelUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        LoadIndicatorUIHandler loadIndicatorUIHandler)
    {
        _parent = parent;
        _settingsPanelUIHandler = settingsPanelUIHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        IsInited = false;
    }

    public void Init(SettingsButtonView settingsButtonView, IReactiveProperty<bool> soundStatus, ILocalizationChanger localizationChanger, bool activeKey = true)
    {
        if (IsInited == false)
        {
            _settingsButtonView = settingsButtonView;
            _settingsButton = settingsButtonView.Button;
            _soundStatus = soundStatus;
            _localizationChanger = localizationChanger;
            IsInited = true;
            _settingsButton.gameObject.SetActive(activeKey);
            SubscribeButton().Invoke();
        }
    }

    public async UniTask OpenPanel()
    {
        if (OnePress == false)
        {
            OnePress = true;
            if (_settingsPanelUIHandler.AssetIsLoaded == false)
            {
                _settingsPanelUIHandler.Init(_parent, _soundStatus, _localizationChanger, SubscribeButton()).Forget();
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

    private Action SubscribeButton()
    {
        return () => _settingsButton.onClick.AddListener(() =>
        {
            _settingsButton.onClick.RemoveAllListeners();
            OnePress = false;
            OpenPanel().Forget();
        });
    }
}