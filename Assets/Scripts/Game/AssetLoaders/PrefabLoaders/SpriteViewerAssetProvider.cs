
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteViewerAssetProvider : PrefabLoader
{
    private const string _name = "SpriteViewerPrefab";
    public async UniTask LoadSpriteViewerPrefab()
    {
        await Load(_name);
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