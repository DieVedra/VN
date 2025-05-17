
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AdvertisingButtonUIHandler
{
    public const int FontSizeValue = 80;
    public const float HeightPanel = 850f;
    public readonly string LabelTextToConfirmedPanel = "Бесплатные монеты!";
    public readonly string TranscriptionTextToConfirmedPanel = "Посмотрите рекламу и получите бесплатные монеты!";
    public readonly string ButtonText = "Смотреть рекламу";

    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private readonly Transform _parent;
    private readonly Wallet _wallet;
    private AdvertisingPanelPrefabProvider _advertisingPanelPrefabProvider;
    private AdvertisingPanelView _advertisingPanelView;
    public bool AssetIsLoad { get; private set; }
    public AdvertisingButtonUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler, Wallet wallet, Transform parent)
    {
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _blackFrameUIHandler = blackFrameUIHandler;
        _wallet = wallet;
        _parent = parent;
        _advertisingPanelPrefabProvider = new AdvertisingPanelPrefabProvider();
        AssetIsLoad = false;
    }

    public void Dispose()
    {
        Addressables.ReleaseInstance(_advertisingPanelView.gameObject);
    }
    public async UniTask Press()
    {
        if (AssetIsLoad == false)
        {
            await _loadIndicatorUIHandler.Init(_parent);
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate();
            _advertisingPanelView = await _advertisingPanelPrefabProvider.CreateAdvertisingPanel(_parent);
            _loadIndicatorUIHandler.StopIndicate();
            AssetIsLoad = true;
        }
        _advertisingPanelView.transform.SetAsLastSibling();
        
        //какая то логика показа рекламы
        
        
        _advertisingPanelView.ButtonExit.gameObject.SetActive(false);
        _advertisingPanelView.gameObject.SetActive(true);
        await UniTask.Delay(2000);
        _advertisingPanelView.ButtonExit.onClick.AddListener(() =>
        {
            _blackFrameUIHandler.OpenTranslucent().Forget();
            _advertisingPanelView.gameObject.SetActive(false);
            _wallet.AddCash(_advertisingPanelView.MonetReward, false);
            _wallet.AddHearts(_advertisingPanelView.HeartsReward, false);
            _advertisingPanelView.ButtonExit.onClick.RemoveAllListeners();
        });
        _advertisingPanelView.ButtonExit.gameObject.SetActive(true);
    }
}