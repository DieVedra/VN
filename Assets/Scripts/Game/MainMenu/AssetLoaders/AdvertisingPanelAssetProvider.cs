
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AdvertisingPanelAssetProvider : LocalAssetLoader
{
    public UniTask<AdvertisingPanelView> LoadAdvertisingPanel(Transform parent = null)
    {
        return Load<AdvertisingPanelView>("AdvertisingPanel", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}