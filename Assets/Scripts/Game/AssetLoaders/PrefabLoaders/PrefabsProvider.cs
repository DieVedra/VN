
using Cysharp.Threading.Tasks;

public class PrefabsProvider
{
    public SpriteViewerAssetProvider SpriteViewerAssetProvider { get; private set; }

    public CustomizationCharacterPanelAssetProvider CustomizationCharacterPanelAssetProvider  { get; private set; }

    public WardrobeCharacterViewerAssetProvider WardrobeCharacterViewerAssetProvider  { get; private set; }

    public SpriteRendererAssetProvider SpriteRendererAssetProvider  { get; private set; }
    public WardrobePSProvider WardrobePSProvider { get; private set; }

    public bool IsInitialized { get; private set; }

    public PrefabsProvider()
    {
        SpriteViewerAssetProvider = new SpriteViewerAssetProvider();
        CustomizationCharacterPanelAssetProvider = new CustomizationCharacterPanelAssetProvider();
        WardrobeCharacterViewerAssetProvider = new WardrobeCharacterViewerAssetProvider();
        SpriteRendererAssetProvider = new SpriteRendererAssetProvider();
        WardrobePSProvider = new WardrobePSProvider();
    }


    public async UniTask Init()
    {
        await SpriteRendererAssetProvider.LoadSpriteRendererPrefab();
        await SpriteViewerAssetProvider.LoadSpriteViewerPrefab();
        await CustomizationCharacterPanelAssetProvider.LoadCustomizationCharacterPanel();
        await WardrobeCharacterViewerAssetProvider.LoadWardrobeCharacterViewerPrefab();
        await WardrobePSProvider.LoadWardrobePSPrefab();
        IsInitialized = true;
    }
}