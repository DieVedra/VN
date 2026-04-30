using Cysharp.Threading.Tasks;

public class ContactBlockNotificationViewPrefabAssetProvider : PrefabLoader
{
    private const string _name = "ContactBlock";
    
    public async UniTask<MessageView> LoadContactBlockNotificationPrefab()
    {
        await Load(_name);
        return CashedPrefab.GetComponent<MessageView>();
    }

    public void UnloadAsset()
    {
        base.Unload();
    }
}