
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlackFramePanelAssetProvider : LocalAssetLoader
{
    public UniTask<BlackFrameView> LoadBlackFramePanel(Transform parent = null)
    {
        return Load<BlackFrameView>("BlackFrame", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}