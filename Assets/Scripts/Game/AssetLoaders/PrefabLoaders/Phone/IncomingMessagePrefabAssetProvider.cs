using Cysharp.Threading.Tasks;
public class IncomingMessagePrefabAssetProvider : PrefabLoader
{
    private const string _name = "PhoneMessageIncoming";

    public async UniTask<MessageView> LoadIncomingMessagePrefab()
    {
        await Load(_name);
        return CashedPrefab.GetComponent<MessageView>();
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}