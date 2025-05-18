
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadScreenAssetProvider : PrefabLoader
{
    private const string _name = "LoadScreen";
    public async UniTask<LoadScreenUIView> LoadAsset(Transform parent = null)
    {
        return await InstantiatePrefab<LoadScreenUIView>(_name, parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    }
}