using Cysharp.Threading.Tasks;
using UnityEngine;

public class BackgroundContentAssetProvider : PrefabLoader
{
    private const string _name = "BackgroundContentPrefab";

    public async UniTask<BackgroundContent> GetBackgroundContent(Transform parent)
    {
        return await InstantiatePrefab<BackgroundContent>(_name, parent);
    }
}