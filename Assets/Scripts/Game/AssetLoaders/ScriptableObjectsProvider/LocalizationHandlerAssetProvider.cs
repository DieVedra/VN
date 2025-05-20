

using Cysharp.Threading.Tasks;

public class LocalizationHandlerAssetProvider : ScriptableObjectAssetLoader
{
    private const string _nameMainMenuLocalizationAsset = "MainMenuLocalizationInfoHolder";

    public async UniTask<MainMenuLocalizationInfoHolder> LoadLocalizationHandlerAsset()
    {
        return await Load<MainMenuLocalizationInfoHolder>(_nameMainMenuLocalizationAsset);
    }
}