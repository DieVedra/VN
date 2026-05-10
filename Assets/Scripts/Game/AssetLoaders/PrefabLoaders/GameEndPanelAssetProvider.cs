using Cysharp.Threading.Tasks;
using UnityEngine;

public class GameEndPanelAssetProvider : PrefabLoader
{
    private const string _namePanelPrefab = "GameEndPanel";
    private const string _nameStatCasePrefab = "GameEndPanel";
    
    public async UniTask<GameEndPanelView> LoadGameEndPanelPrefab(Transform parent)
    {
        GameObject panel = await InstantiatePrefab(_namePanelPrefab, parent);
        return panel.GetComponent<GameEndPanelView>();
    }
    public async UniTask<StatCaseView> LoadStatCasePrefab(Transform parent)
    {
        GameObject statCase = await InstantiatePrefab(_nameStatCasePrefab, parent);
        return statCase.GetComponent<StatCaseView>();
    }
}