
using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryPanelAssetProvider : LocalAssetLoader
{
    private const string _name = "StoryPanelHorizontal";
    public UniTask<StoryPanel> LoadAsset(Transform parent = null)
    {
        return Load<StoryPanel>(_name, parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}