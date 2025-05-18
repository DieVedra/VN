
using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShopMoneyAssetLoader : PrefabLoader
{
    private const string _name = "ShopPanel";
    public async UniTask<ShopMoneyPanelView> CreateShopMoneyPanel(Transform parent = null)
    {
        return await InstantiatePrefab<ShopMoneyPanelView>(_name, parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}