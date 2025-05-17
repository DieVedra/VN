
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CircleIndicatorAssetProvider : LocalAssetLoader
{
    public UniTask<RectTransform> LoadCircleIndicatorAsset(Transform parent = null)
    {
        return Load<RectTransform>("CircleIndicator", parent);
    }
    public UniTask<RectTransform> LoadCircleIndicatorFillAsset(Transform parent = null)
    {
        return Load<RectTransform>("CircleIndicatorFill", parent);
    }
}