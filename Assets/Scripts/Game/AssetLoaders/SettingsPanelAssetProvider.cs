
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SettingsPanelAssetProvider : PrefabLoader
{
    private const string _name = "SettingsPanel";
    public async UniTask<SettingsPanelView> CreateSettingsPanel(Transform parent = null)
    {
        return await InstantiatePrefab<SettingsPanelView>(_name, parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}