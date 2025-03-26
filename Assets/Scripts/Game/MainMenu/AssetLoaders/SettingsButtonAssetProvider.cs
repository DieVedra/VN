
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonAssetProvider : LocalAssetLoader
{
    public UniTask<Button> LoadAsset(Transform parent = null)
    {
        return Load<Button>("SettingsButton", parent);
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}