
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteRendererAssetProvider : PrefabLoader
{
    private const string _name = "SpriteRendererPrefab";
    public async UniTask LoadSpriteRendererPrefab()
    {
        await Load(_name);
    }

    public SpriteRenderer CreateSpriteRenderer(Transform parent)
    {
        GameObject gameObject = Object.Instantiate(CashedPrefab, parent);
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            return spriteRenderer;
        }
        else
        {
            return null;
        }
    }
    
    public async UniTask<SpriteRenderer> CreateSpriteRendererAsync(Transform parent)
    {
        GameObject gameObject = await InstantiatePrefab(_name, parent);
        if (gameObject.TryGetComponent(out SpriteRenderer spriteRenderer))
        {
            return spriteRenderer;
        }
        else
        {
            return default;
        }
    }
    
    public void UnloadAsset()
    {
        base.Unload();
    } 
}