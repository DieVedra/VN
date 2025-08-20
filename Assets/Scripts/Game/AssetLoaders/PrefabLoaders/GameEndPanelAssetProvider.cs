using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameEndPanelAssetProvider : PrefabLoader
{
    private const string _name = "GameEndPanel";
    
    public async UniTask<GameEndPanelView> LoadGameEndPanelPrefab(Transform parent)
    {
        GameObject panel = await InstantiatePrefab(_name, parent);
        return panel.GetComponent<GameEndPanelView>();
    }
}