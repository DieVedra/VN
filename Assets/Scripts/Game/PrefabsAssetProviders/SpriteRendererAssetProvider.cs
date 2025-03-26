
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteRendererAssetProvider : PrefabLoader
{
    public async UniTask LoadSpriteRendererPrefab()
    {
        await Load("SpriteRenderer");
    }

    public SpriteRenderer CreateSpriteRenderer(Transform parent)
    {
        GameObject gameObject = Object.Instantiate(CashedPrefab, parent: parent);
        return gameObject.GetComponent<SpriteRenderer>();
    }
    
    public void UnloadAsset()
    {
        base.Unload();
    } 
}