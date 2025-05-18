
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BlackFramePanelAssetProvider : PrefabLoader
{
    private const string _name = "BlackFrame";

    public async UniTask<BlackFrameView> CreateBlackFramePanel(Transform parent = null)
    {
        return await InstantiatePrefab<BlackFrameView>(_name, parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}