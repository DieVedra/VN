
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayStoryPanelAssetProvider : PrefabLoader
{
    private const string _name = "PlayStoryPanel";
    public async UniTask<PlayStoryPanel> CreatePlayStoryPanel(Transform parent = null)
    {
        GameObject instantiatedPrefab = await InstantiatePrefab(_name, parent);
        if (instantiatedPrefab.TryGetComponent(out PlayStoryPanel playStoryPanel))
        {
            return playStoryPanel;
        }
        else
        {
            return default;
        }
    }
}