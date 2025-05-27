
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryPanelAssetProvider : PrefabLoader
{
    private const string _name = "StoryPanelHorizontal";
    public UniTask<StoryPanel> CreateStoryPanel(Transform parent = null)
    {
        
        return InstantiatePrefab<StoryPanel>(_name, parent);
    }
}