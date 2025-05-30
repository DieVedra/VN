
using Cysharp.Threading.Tasks;

public class LevelCanvasAssetProvider : PrefabLoader
{
    private const string _name = "LevelCanvas";

    public async UniTask<LevelUIView> CreateAsset()
    {
        return await InstantiatePrefab<LevelUIView>(_name);
    }
}