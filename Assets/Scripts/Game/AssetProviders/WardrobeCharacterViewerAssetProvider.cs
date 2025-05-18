
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WardrobeCharacterViewerAssetProvider : PrefabLoader
{
    private const string _name = "WardrobeCharacterViewer";
    public async UniTask LoadWardrobeCharacterViewerPrefab()
    {
        await Load(_name);
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