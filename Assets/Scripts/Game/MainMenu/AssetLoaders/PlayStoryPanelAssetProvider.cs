
using Cysharp.Threading.Tasks;
using UnityEngine;

public class PlayStoryPanelAssetProvider : LocalAssetLoader
{
    public UniTask<PlayStoryPanel> LoadAsset(Transform parent = null)
    {
        return Load<PlayStoryPanel>("PlayStoryPanel", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}