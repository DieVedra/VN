
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteViewerAssetProvider : PrefabLoader
{
    public async UniTask LoadSpriteViewerPrefab()
    {
        await Load("SpriteViewerPrefab");
    }

    public SpriteViewer CreateSpriteViewer(Transform parent)
    {
        GameObject gameObject = Object.Instantiate(CashedPrefab, parent: parent);
        return gameObject.GetComponent<SpriteViewer>();
    }
    public void UnloadAsset()
    {
        base.Unload();
    } 
}