
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectContextCanvasAssetProvider : PrefabLoader
{
    private const string _name = "ProjectContextCanvas";
    public async UniTask<Canvas> LoadAsset(Transform parent = null)
    {
        return await InstantiatePrefab<Canvas>(_name, parent);
    }
}