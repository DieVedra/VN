
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ConfirmedPanelAssetProvider : PrefabLoader
{
    private const string _name = "ConfirmedPanel";

    public async UniTask<ConfirmedPanelView> CreateConfirmedPanel(Transform parent = null)
    {
        return await InstantiatePrefab<ConfirmedPanelView>(_name, parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    }
}