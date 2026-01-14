using Cysharp.Threading.Tasks;
using UnityEngine;

public class SpriteRendererAssetProvider : PrefabLoader
{
    private const string _name = "SpriteRendererPrefab";
    public async UniTask<GameObject> LoadSpriteRendererPrefab()
    {
        await Load(_name);
        return CashedPrefab;
    }
    public void UnloadAsset()
    {
        base.Unload();
    } 
}