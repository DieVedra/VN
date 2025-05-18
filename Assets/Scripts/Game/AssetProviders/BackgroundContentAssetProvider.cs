
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BackgroundContentAssetProvider : PrefabLoader
{
    private const string _name = "BackgroundContentPrefab";
    private List<BackgroundContent> _createdObjects;

    public async UniTask<BackgroundContent> GetBackgroundContent(Transform parent)
    {
        var content = await InstantiatePrefab<BackgroundContent>(_name, parent);
        if (_createdObjects == null)
        {
            _createdObjects = new List<BackgroundContent>();
        }
        _createdObjects.Add(content);
        return content;
    }

    public void ReleaseAllCreatedObjects()
    {
        for (int i = 0; i < _createdObjects.Count; ++i)
        {
            Addressables.ReleaseInstance(_createdObjects[i].gameObject);
        }
    }
}