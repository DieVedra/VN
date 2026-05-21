using Cysharp.Threading.Tasks;

public class IconsDataAssetProvider : ScriptableObjectAssetLoader
{
    private const string _name = "IconsData";

    public async UniTask<BackgroundData> LoadIconsDataAsset()
    {
        return await Load<BackgroundData>(_name);
    }
}