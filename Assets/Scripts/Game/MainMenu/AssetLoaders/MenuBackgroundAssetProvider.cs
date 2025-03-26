

using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuBackgroundAssetProvider : LocalAssetLoader
{
    public UniTask<RectTransform> LoadAsset(Transform parent = null)
    {
        return Load<RectTransform>("MenuBackground", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}