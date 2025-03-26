

using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanelButtonUIHandler
{
    private readonly int _sublingIndex = 1;
    private readonly SettingsButtonAssetProvider _settingsButtonAssetProvider;
    private Button _settingsButton;
    private SettingsPanelUIHandler _settingsPanelUIHandler;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private Transform _parent;
    public bool AssetIsLoaded { get; private set; }
    public SettingsPanelButtonUIHandler(Transform parent, SettingsPanelUIHandler settingsPanelUIHandler, BlackFrameUIHandler blackFrameUIHandler,
        LoadIndicatorUIHandler loadIndicatorUIHandler)
    {
        _settingsButtonAssetProvider = new SettingsButtonAssetProvider();
        _parent = parent;
        _settingsPanelUIHandler = settingsPanelUIHandler;
        _blackFrameUIHandler = blackFrameUIHandler;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        AssetIsLoaded = false;
    }
    public async UniTask Init()
    {
        if (AssetIsLoaded == false)
        {
            _settingsButton = await _settingsButtonAssetProvider.LoadAsset(_parent);
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
            await _loadIndicatorUIHandler.Init(_blackFrameUIHandler.Transform);
            _settingsPanelUIHandler.Init(_parent).Forget();
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            
            _blackFrameUIHandler.CloseTranslucent().Forget();
            await UniTask.WaitUntil(() => _settingsPanelUIHandler.AssetIsLoaded == true);
            
            _settingsPanelUIHandler.Show(_blackFrameUIHandler);
            _loadIndicatorUIHandler.StopIndicate();
        }
        else
        {
            _blackFrameUIHandler.CloseTranslucent().Forget();
            _settingsPanelUIHandler.Show(_blackFrameUIHandler);
        }
    }
}