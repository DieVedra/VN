using Cysharp.Threading.Tasks;
public class ContactPrefabAssetProvider : PrefabLoader
{
    private const string _name = "PhoneContact";

    public async UniTask<ContactView> LoadContactPrefab()
    {
        await Load(_name);
        return CashedPrefab.GetComponent<ContactView>();
    }

    public void UnloadAsset()
    {
        base.Unload();
    } 
}