
using Cysharp.Threading.Tasks;

public class StoriesProviderAssetProvider : AssetLoader<StoriesProvider>
{
    private const string _name = "StoriesProvider";
    
    public async UniTask<StoriesProvider> Load()
    {
        return await Load(_name);
    }

    public void Release()
    {
        base.Unload();
    }
}