

using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuBackgroundAssetProvider : LocalAssetLoader
{
    private const string _name = "MenuBackground";
    public UniTask<RectTransform> LoadAsset(Transform parent = null)
    {
        return Load<RectTransform>(_name, parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}