

using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadIndicatorAssetProvider : LocalAssetLoader
{
    public UniTask<LoadIndicatorView> LoadAsset(Transform parent = null)
    {
        return Load<LoadIndicatorView>("LoadIndicator", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    }
}