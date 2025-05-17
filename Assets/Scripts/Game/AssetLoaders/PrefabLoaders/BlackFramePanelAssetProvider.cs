
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlackFramePanelAssetProvider : PrefabLoader
{
    private const string _name = "BlackFrame";

    public async UniTask<BlackFrameView> CreateBlackFramePanel(Transform parent = null)
    {
        GameObject instantiated = await InstantiatePrefab(_name, parent);
        if (instantiated.TryGetComponent(out BlackFrameView blackFrameView))
        {
            return blackFrameView;
        }
        else
        {
            return default;
        }
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}