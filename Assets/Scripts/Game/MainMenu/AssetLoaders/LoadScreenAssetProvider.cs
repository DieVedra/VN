
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadScreenAssetProvider : LocalAssetLoader
{
    public UniTask<LoadScreenUIView> LoadAsset(Transform parent = null)
    {
        return Load<LoadScreenUIView>("LoadScreen", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    }
}