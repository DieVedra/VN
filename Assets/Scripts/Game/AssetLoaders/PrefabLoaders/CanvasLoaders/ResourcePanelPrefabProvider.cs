using Cysharp.Threading.Tasks;
using UnityEngine;

public class ResourcePanelPrefabProvider : PrefabLoader
{
    private const string _name = "ResourcePanel";
    public async UniTask<ResourcePanelView> CreateAsset(Transform parent)
    {
        return await InstantiatePrefab<ResourcePanelView>(_name, parent);
    }
}