
using Cysharp.Threading.Tasks;
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

    public void Init(SettingsButtonView settingsButtonView)
    {
        if (AssetIsLoaded == false)
        {
            _settingsButtonView = settingsButtonView;
            _settingsButton = settingsButtonView.Button;
            AssetIsLoaded = true;
        }
    }

    public void SubscribeButtonAndActivate()
    {
        _settingsButton.transform.SetSiblingIndex(_sublingIndex);
        _settingsButton.gameObject.SetActive(true);
        _settingsButton.onClick.AddListener(() =>
        {
            OpenPanel().Forget();
        });
    }

    private async UniTask OpenPanel()
    {
        if (_settingsPanelUIHandler.AssetIsLoaded == false)
        {
            await _loadIndicatorUIHandler.Init(_darkeningBackgroundFrameUIHandler.Transform);
            _settingsPanelUIHandler.Init(_parent).Forget();
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            
            _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
            await UniTask.WaitUntil(() => _settingsPanelUIHandler.AssetIsLoaded == true);
            
            _settingsPanelUIHandler.Show(_darkeningBackgroundFrameUIHandler);
            _loadIndicatorUIHandler.StopIndicate();
        }
        else
        {
            _darkeningBackgroundFrameUIHandler.CloseTranslucent().Forget();
            _settingsPanelUIHandler.Show(_darkeningBackgroundFrameUIHandler);
        }
    }
}