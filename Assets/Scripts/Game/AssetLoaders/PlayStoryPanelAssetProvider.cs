
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayStoryPanelAssetProvider : PrefabLoader
{
    private const string _name = "PlayStoryPanel";
    public async UniTask<PlayStoryPanel> CreatePlayStoryPanel(Transform parent = null)
    {
        return await InstantiatePrefab<PlayStoryPanel>(_name, parent);
    }
}