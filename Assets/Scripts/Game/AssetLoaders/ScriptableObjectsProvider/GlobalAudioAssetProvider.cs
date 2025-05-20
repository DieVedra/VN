

using Cysharp.Threading.Tasks;

public class GlobalAudioAssetProvider : ScriptableObjectAssetLoader
{
    private const string _name = "GlobalAudioData";

    public async UniTask<GlobalAudioData> LoadGlobalAudioAsset()
    {
        return await Load<GlobalAudioData>(_name);
    }
}