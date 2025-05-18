
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class SettingsPanelUIHandler
{
    private readonly SettingsPanelAssetProvider _settingsPanelAssetProvider;
    private SettingsPanelView _settingsPanelView;
    public Transform Transform => _settingsPanelView.transform;
    public bool AssetIsLoaded { get; private set; }
    public SettingsPanelUIHandler()
    {
        _settingsPanelAssetProvider = new SettingsPanelAssetProvider();
    }
    public async UniTask Init(Transform parent)
    {
        if (AssetIsLoaded == false)
        {
            _settingsPanelView = await _settingsPanelAssetProvider.CreateSettingsPanel(parent);
            AssetIsLoaded = true;
        }
    }

    public void Dispose()
    {
        if (_settingsPanelView != null)
        {
            Addressables.ReleaseInstance(_settingsPanelView.gameObject);
        }
    }
    public void Show(BlackFrameUIHandler blackFrameUIHandler)
    {
        _settingsPanelView.gameObject.SetActive(true);
        _settingsPanelView.transform.SetAsLastSibling();

        _settingsPanelView.ExitButton.onClick.AddListener(()=>
        {
            Hide(blackFrameUIHandler);
            _settingsPanelView.ExitButton.onClick.RemoveAllListeners();
        });
    }

    private void Hide(BlackFrameUIHandler blackFrameUIHandler)
    {
        _settingsPanelView.gameObject.SetActive(false);
        blackFrameUIHandler.OpenTranslucent().Forget();
    }
}