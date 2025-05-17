
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AdvertisingPanelPrefabProvider : PrefabLoader
{
    private const string _name = "AdvertisingPanel";
    
    public async UniTask<AdvertisingPanelView> CreateAdvertisingPanel(Transform parent = null)
    {
        GameObject instantiatedPrefab = await InstantiatePrefab(_name, parent);
        if (instantiatedPrefab.TryGetComponent(out AdvertisingPanelView advertisingPanelView))
        {
            return advertisingPanelView;
        }
        else
        {
            return default;
        }
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