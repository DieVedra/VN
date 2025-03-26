
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CustomizationCharacterPanelAssetProvider : PrefabLoader
{
    public async UniTask LoadCustomizationCharacterPanel()
    {
        await Load("CustomizationCharacterPanel");
    }
    public CustomizationCharacterPanelUI CreateCustomizationCharacterPanelUI(Transform parent)
    {
        GameObject gameObject = Object.Instantiate(CashedPrefab, parent: parent);
        return gameObject.GetComponent<CustomizationCharacterPanelUI>();
    }
    public void UnloadAsset()
    {
        base.Unload();
    } 
}