
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ConfirmedPanelAssetProvider : LocalAssetLoader
{
    public UniTask<ConfirmedPanelView> LoadAsset(Transform parent = null)
    {
        return Load<ConfirmedPanelView>("ConfirmedPanel", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    }
}