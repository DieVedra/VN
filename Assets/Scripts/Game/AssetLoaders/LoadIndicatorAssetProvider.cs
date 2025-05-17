
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadIndicatorAssetProvider : PrefabLoader
{
    private const string _name = "LoadIndicator";
    public async UniTask<LoadIndicatorView> CreateIndicatorView(Transform parent = null)
    {
        GameObject instantiate = await InstantiatePrefab(_name, parent);
        if (instantiate.TryGetComponent(out LoadIndicatorView loadIndicatorView))
        {
            return loadIndicatorView;
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