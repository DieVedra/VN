using Cysharp.Threading.Tasks;

public class LocalizationHandlerAssetProvider : ScriptableObjectAssetLoader
{
    private const string _nameMainMenuLocalizationAsset = "LocalizationInfoHolder";

    public async UniTask<LocalizationInfoHolder> LoadLocalizationHandlerAsset()
    {
        return await Load<LocalizationInfoHolder>(_nameMainMenuLocalizationAsset);
    }
}