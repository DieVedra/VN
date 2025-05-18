
using Cysharp.Threading.Tasks;
using UnityEngine;

public class CustomizationCharacterPanelAssetProvider : PrefabLoader
{
    private const string _name = "CustomizationCharacterPanel";
    public async UniTask LoadCustomizationCharacterPanel()
    {
        await Load(_name);
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