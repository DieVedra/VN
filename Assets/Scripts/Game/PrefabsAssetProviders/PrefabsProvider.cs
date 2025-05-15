
using Cysharp.Threading.Tasks;

public static class PrefabsProvider
{
    public static SpriteViewerAssetProvider SpriteViewerAssetProvider { get; private set; }
    public static CustomizationCharacterPanelAssetProvider CustomizationCharacterPanelAssetProvider  { get; private set; }
    public static WardrobeCharacterViewerAssetProvider WardrobeCharacterViewerAssetProvider  { get; private set; }
    public static SpriteRendererAssetProvider SpriteRendererAssetProvider  { get; private set; }
    public static bool IsInitialized { get; private set; }

    public static async UniTask Init()
    {
        SpriteViewerAssetProvider = new SpriteViewerAssetProvider();
        CustomizationCharacterPanelAssetProvider = new CustomizationCharacterPanelAssetProvider();
        WardrobeCharacterViewerAssetProvider = new WardrobeCharacterViewerAssetProvider();
        SpriteRendererAssetProvider = new SpriteRendererAssetProvider();
        
        await UniTask.WhenAll(
            SpriteRendererAssetProvider.LoadSpriteRendererPrefab(),
            SpriteViewerAssetProvider.LoadSpriteViewerPrefab(),
            CustomizationCharacterPanelAssetProvider.LoadCustomizationCharacterPanel(),
            WardrobeCharacterViewerAssetProvider.LoadWardrobeCharacterViewerPrefab()
        );
        
        IsInitialized = true;
    }
}