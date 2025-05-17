
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ProjectContextCanvasAssetProvider : PrefabLoader
{
    private const string _name = "ProjectContextCanvas";
    public async UniTask<Canvas> LoadAsset(Transform parent = null)
    {
        GameObject instantiated = await InstantiatePrefab(_name, parent);
        if (instantiated.TryGetComponent(out Canvas loadScreenView))
        {
            return loadScreenView;
        }
        else
        {
            return default;
        }
    }
}