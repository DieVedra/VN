
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CircleIndicatorAssetProvider : PrefabLoader
{
    private const string _nameCircleIndicator = "CircleIndicator";
    private const string _nameCircleIndicatorFill = "CircleIndicatorFill";
    public UniTask<RectTransform> CreateLoadCircleIndicator(Transform parent = null)
    {
        return InstantiatePrefab<RectTransform>(_nameCircleIndicator, parent);
    }
    public UniTask<RectTransform> CreateLoadCircleIndicatorFill(Transform parent = null)
    {
        return InstantiatePrefab<RectTransform>(_nameCircleIndicatorFill, parent);
    }
}