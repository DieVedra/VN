using Cysharp.Threading.Tasks;
public class NotificationViewPrefabAssetProvider : PrefabLoader
{
    private const string _name = "PhoneNotification";

    public async UniTask<NotificationView> LoadNotificationPrefab()
    {
        await Load(_name);
        return CashedPrefab.GetComponent<NotificationView>();
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}