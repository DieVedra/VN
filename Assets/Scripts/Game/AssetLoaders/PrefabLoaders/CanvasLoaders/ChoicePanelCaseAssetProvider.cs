using Cysharp.Threading.Tasks;

public class ChoicePanelCaseAssetProvider : AssetLoader<ChoiceCaseView>
{
    private const string _name = "ChoiceCase";
    
    public async UniTask<ChoiceCaseView> LoadAsset()
    {
        return await Load(_name);
    }

    public void Unload()
    {
        base.Unload();
    }
}