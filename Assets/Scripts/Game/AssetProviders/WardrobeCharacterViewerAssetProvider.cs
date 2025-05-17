
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WardrobeCharacterViewerAssetProvider : PrefabLoader
{
    public async UniTask LoadWardrobeCharacterViewerPrefab()
    {
        await Load("WardrobeCharacterViewer");
    }
    public WardrobeCharacterViewer CreateWardrobeCharacterViewer(Transform parent)
    {
        GameObject gameObject = Object.Instantiate(CashedPrefab, parent: parent);
        return gameObject.GetComponent<WardrobeCharacterViewer>();
    }
    public void UnloadAsset()
    {
        base.Unload();
    }
}