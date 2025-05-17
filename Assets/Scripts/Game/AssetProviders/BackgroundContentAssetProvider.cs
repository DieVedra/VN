
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BackgroundContentAssetProvider : PrefabLoader
{
    private const string _backgroundContentAssetName = "BackgroundContentPrefab";
    private List<GameObject> _createdObjects;

    public async UniTask<BackgroundContent> GetBackgroundContent(Transform parent)
    {
        GameObject go = await InstantiatePrefab(_backgroundContentAssetName, parent);
        if (go.TryGetComponent(out BackgroundContent result) == true)
        {
            if (_createdObjects == null)
            {
                _createdObjects = new List<GameObject>();
            }
            _createdObjects.Add(go);
            return result;
        }
        else
        {
            return default;
        }
    }

    public void ReleaseAllCreatedObjects()
    {
        for (int i = 0; i < _createdObjects.Count; ++i)
        {
            Addressables.ReleaseInstance(_createdObjects[i]);
        }
    }
}