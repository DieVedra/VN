
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SettingsPanelAssetProvider : LocalAssetLoader
{
    public UniTask<SettingsPanelView> LoadAsset(Transform parent = null)
    {
        return Load<SettingsPanelView>("SettingsPanel", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}