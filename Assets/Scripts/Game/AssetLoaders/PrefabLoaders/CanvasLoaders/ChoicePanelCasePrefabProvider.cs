using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChoicePanelCasePrefabProvider : PrefabLoader
{
    private const string _name = "ChoiceCase";
    public GameObject GetPrefab => CashedPrefab;

    public async UniTask<GameObject> InstantiatePrefab(Transform parent)
    {
        return await InstantiatePrefab(_name, parent);
    }
}