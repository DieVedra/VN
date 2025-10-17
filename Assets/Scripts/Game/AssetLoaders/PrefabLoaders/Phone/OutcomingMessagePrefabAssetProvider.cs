using Cysharp.Threading.Tasks;
public class OutcomingMessagePrefabAssetProvider : PrefabLoader
{
    private const string _name = "PhoneMessageOutcoming";

    public async UniTask<MessageView> LoadOutcomingMessagePrefab()
    {
        await Load(_name);
        return CashedPrefab.GetComponent<MessageView>();
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}