
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BottomPanelAssetProvider : LocalAssetLoader
{
    public UniTask<BottomPanelView> LoadBottomPanelAsset(Transform parent = null)
    {
        return Load<BottomPanelView>("BottomPanel", parent);
    }
}