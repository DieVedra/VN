
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WardrobeBackgroundAssetProvider : PrefabLoader
{
    public async UniTask LoadWardrobeBackgroundPrefab()
    {
        await Load("WardrobeBackground");
    }
    public BackgroundContent CreateWardrobeBackground(Transform parent)
    {
        GameObject gameObject = Object.Instantiate(CashedPrefab, parent: parent);
        return gameObject.GetComponent<BackgroundContent>();
    }
    public void UnloadAsset()
    {
        base.Unload();
    }  
}