

using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShopMoneyAssetLoader : LocalAssetLoader
{
    public UniTask<ShopMoneyPanelView> LoadAsset(Transform parent = null)
    {
        return Load<ShopMoneyPanelView>("ShopPanel", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}