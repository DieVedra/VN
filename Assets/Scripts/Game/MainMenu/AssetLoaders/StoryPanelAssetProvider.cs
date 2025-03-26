

using Cysharp.Threading.Tasks;
using UnityEngine;

public class StoryPanelAssetProvider : LocalAssetLoader
{
    public UniTask<StoryPanel> LoadAsset(Transform parent = null)
    {
        return Load<StoryPanel>("StoryPanelHorizontal", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}