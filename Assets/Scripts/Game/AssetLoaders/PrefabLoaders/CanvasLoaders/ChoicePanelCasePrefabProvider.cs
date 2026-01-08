using Cysharp.Threading.Tasks;
using UnityEngine;

public class ChoicePanelCasePrefabProvider : PrefabLoader
{
    private const string _name = "ChoiceCase";
    public GameObject GetPrefab => CashedPrefab;

    
    public async UniTask LoadPrefab()
    {
        await Load(_name);
    }

    public void Unload()
    {
        base.Unload();
    }
}