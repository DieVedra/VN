
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AdvertisingButtonUIHandler
{
    public const int FontSizeValue = 80;
    public const float HeightPanel = 850f;
    public readonly LocalizationString LabelTextToConfirmedPanel = "Бесплатные монеты!";
    public readonly LocalizationString TranscriptionTextToConfirmedPanel = "Посмотрите рекламу и получите бесплатные монеты!";
    public readonly LocalizationString ButtonText = "Смотреть рекламу";
    public readonly LocalizationString AdvertisingButtonText  = "Реклама";

    private readonly LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private readonly BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private readonly Transform _parent;
    private readonly Wallet _wallet;
    private AdvertisingPanelPrefabProvider _advertisingPanelPrefabProvider;
    private AdvertisingPanelView _advertisingPanelView;
    public bool AssetIsLoad { get; private set; }
    public AdvertisingButtonUIHandler(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler,
        Wallet wallet, Transform parent)
    {
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
        _wallet = wallet;
        _parent = parent;
        _advertisingPanelPrefabProvider = new AdvertisingPanelPrefabProvider();
        AssetIsLoad = false;
    }

    public void Dispose()
    {
        if (_advertisingPanelView != null)
        {
            Addressables.ReleaseInstance(_advertisingPanelView.gameObject);
        }
    }
    public async UniTask Press()
    {
        if (AssetIsLoad == false)
        {
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
            _darkeningBackgroundFrameUIHandler.OpenTranslucent().Forget();
            _advertisingPanelView.gameObject.SetActive(false);
            _wallet.AddCash(_advertisingPanelView.MonetReward, false);
            _wallet.AddHearts(_advertisingPanelView.HeartsReward, false);
            _advertisingPanelView.ButtonExit.onClick.RemoveAllListeners();
        });
        _advertisingPanelView.ButtonExit.gameObject.SetActive(true);
    }
}