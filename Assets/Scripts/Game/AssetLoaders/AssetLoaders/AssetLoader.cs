
using Cysharp.Threading.Tasks;
using UnityEngine.AddressableAssets;

public class AssetLoader<T>
{
    private T _cashedObject;
    public T GetAsset => _cashedObject;
    
    protected async UniTask<T> Load(string assetId)
    {
        var handle = Addressables.LoadAssetAsync<T>(assetId);
        _cashedObject = await handle.Task;
        if (_cashedObject is { } f)
        {
            return f;
        }
        else
        {
            Unload();
            return default;
        }
    }

    protected void Unload()
    {
        if (_cashedObject == null)
        {
            return;
        }
        Addressables.Release(_cashedObject);
        _cashedObject = default(T);
    }
}