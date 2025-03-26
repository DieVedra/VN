
using Cysharp.Threading.Tasks;

public class AdvertisingHandler
{
    private readonly Wallet _wallet;
    public Wallet Wallet => _wallet;

    public AdvertisingHandler(Wallet wallet)
    {
        _wallet = wallet;
    }

    // public async UniTask TryShowAD()
    // {
    //     await UniTask.Delay(2000);
    //     
    //     
    // }
}