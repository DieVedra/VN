using Cysharp.Threading.Tasks;

public class StartConfigAssetProvider : AssetLoader<StartConfig>
{
    private const string _name = "StartConfig";
    
    public async UniTask<StartConfig> Load()
    {
        return await Load(_name);
    }
}