

using Cysharp.Threading.Tasks;
using UnityEngine;

public class ShopMoneyButtonsAssetProvider : LocalAssetLoader
{
    public UniTask<ResourcePanelButtonView> LoadAssetMonet(Transform parent = null)
    {
        return Load<ResourcePanelButtonView>("MonetPanelButton", parent);
    }
    public UniTask<ResourcePanelButtonView> LoadAssetHearth(Transform parent = null)
    {
        return Load<ResourcePanelButtonView>("HeartsPanelButton", parent);
    }
}