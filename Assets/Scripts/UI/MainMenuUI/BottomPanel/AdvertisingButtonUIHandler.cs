
using Cysharp.Threading.Tasks;
using UnityEngine;

public class AdvertisingButtonUIHandler
{
    public readonly string LabelTextToConfirmedPanel = "Бесплатные монеты!";
    public readonly string TranscriptionTextToConfirmedPanel = "Посмотрите рекламу и получите бесплатные монеты!";
    public readonly string ButtonText = "Смотреть рекламу";
    public readonly int FontSizeValue = 80;
    public readonly float HeightPanel = 850f;

    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly BlackFrameUIHandler _blackFrameUIHandler;
    private AdvertisingHandler _advertisingHandler;
    private readonly Transform _parent;
    private AdvertisingPanelAssetProvider _advertisingPanelAssetProvider;
    private AdvertisingPanelView _advertisingPanelView;
    public bool AssetIsLoad { get; private set; }
    public AdvertisingButtonUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler, AdvertisingHandler advertisingHandler, Transform parent)
    {
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _blackFrameUIHandler = blackFrameUIHandler;
        _advertisingHandler = advertisingHandler;
        _parent = parent;
        _advertisingPanelAssetProvider = new AdvertisingPanelAssetProvider();
        AssetIsLoad = false;
    }

    public async UniTask Press()
    {
        if (AssetIsLoad == false)
        {
            await _loadIndicatorUIHandler.Init(_parent);
            _loadIndicatorUIHandler.SetClearIndicateMode();
            _loadIndicatorUIHandler.StartIndicate(_blackFrameUIHandler.Transform);
            _advertisingPanelView = await _advertisingPanelAssetProvider.LoadAdvertisingPanel(_parent);
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
            _advertisingHandler.Wallet.AddCash(_advertisingPanelView.MonetReward, false);
            _advertisingHandler.Wallet.AddHearts(_advertisingPanelView.HeartsReward, false);
            _advertisingPanelView.ButtonExit.onClick.RemoveAllListeners();
        });
        _advertisingPanelView.ButtonExit.gameObject.SetActive(true);
    }
}