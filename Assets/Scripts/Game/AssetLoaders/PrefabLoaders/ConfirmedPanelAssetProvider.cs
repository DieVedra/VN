
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ConfirmedPanelAssetProvider : PrefabLoader
{
    private const string _name = "ConfirmedPanel";

    public async UniTask<ConfirmedPanelView> CreateConfirmedPanel(Transform parent = null)
    {
        GameObject instantiated = await InstantiatePrefab(_name, parent);
        if (instantiated.TryGetComponent(out ConfirmedPanelView confirmedPanelView))
        {
            return confirmedPanelView;
        }
        else
        {
            return default;
        }
    }

    public void UnloadAsset()
    {
        base.Unload();
    }
}