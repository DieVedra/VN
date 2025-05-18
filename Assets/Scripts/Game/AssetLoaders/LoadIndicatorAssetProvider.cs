
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadIndicatorAssetProvider : PrefabLoader
{
    private const string _name = "LoadIndicator";
    public async UniTask<LoadIndicatorView> CreateIndicatorView(Transform parent = null)
    {
        return await InstantiatePrefab<LoadIndicatorView>(_name, parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    }
}