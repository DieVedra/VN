
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AdvertisingPanelPrefabProvider : PrefabLoader
{
    private const string _name = "AdvertisingPanel";
    
    public async UniTask<AdvertisingPanelView> CreateAdvertisingPanel(Transform parent = null)
    {
        return await InstantiatePrefab<AdvertisingPanelView>(_name, parent);
    }

    public async UniTask Load()
    {
        await Load(_name);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}