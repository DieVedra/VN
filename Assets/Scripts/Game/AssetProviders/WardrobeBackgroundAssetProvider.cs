
using Cysharp.Threading.Tasks;
using UnityEngine;

public class WardrobeBackgroundAssetProvider : PrefabLoader
{
    private const string _name = "WardrobeBackground";
    public async UniTask LoadWardrobeBackgroundPrefab()
    {
        await Load(_name);
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