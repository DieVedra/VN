
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadScreenAssetProvider : PrefabLoader
{
    private const string _name = "LoadScreen";
    public async UniTask<LoadScreenUIView> LoadAsset(Transform parent = null)
    {
        GameObject instantiated = await InstantiatePrefab(_name, parent);
        if (instantiated.TryGetComponent(out LoadScreenUIView loadScreenView))
        {
            return loadScreenView;
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