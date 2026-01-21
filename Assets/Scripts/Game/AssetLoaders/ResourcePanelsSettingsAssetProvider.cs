using Cysharp.Threading.Tasks;

public class ResourcePanelsSettingsAssetProvider : ScriptableObjectAssetLoader
{
    private const string _name = "ResourcePanelsSettingsProvider";

    public async UniTask<ResourcePanelsSettingsProvider> LoadLocalizationHandlerAsset()
    {
        return await Load<ResourcePanelsSettingsProvider>(_name);
    }
}