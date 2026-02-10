using System;
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

    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private BlackFrameUIHandler _darkeningBackgroundFrameUIHandler;
    private Transform _parent;
    private GlobalCanvasCloser _globalCanvasCloser;

    private readonly Wallet _wallet;
    private AdvertisingPanelPrefabProvider _advertisingPanelPrefabProvider;
    private AdvertisingPanelView _advertisingPanelView;
    public bool AssetIsLoad { get; private set; }
    public AdvertisingButtonUIHandler(Wallet wallet)
    {
        _wallet = wallet;
        _advertisingPanelPrefabProvider = new AdvertisingPanelPrefabProvider();
        AssetIsLoad = false;
    }

    public void Init(Transform parent, GlobalCanvasCloser globalCanvasCloser,
        LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler darkeningBackgroundFrameUIHandler)
    {
        _parent = parent;
        _globalCanvasCloser = globalCanvasCloser;
        _loadIndicatorUIHandler = loadIndicatorUIHandler;
        _darkeningBackgroundFrameUIHandler = darkeningBackgroundFrameUIHandler;
    }
    public void Shutdown()
    {
        if (_advertisingPanelView != null)
        {
            Addressables.ReleaseInstance(_advertisingPanelView.gameObject);
        }
    }
    public async UniTask Show(Action operationPutExitButton)
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
        
        _globalCanvasCloser.TryEnable();

        _advertisingPanelView.ButtonExit.gameObject.SetActive(false);
        _advertisingPanelView.gameObject.SetActive(true);
        await UniTask.Delay(2000);
        _advertisingPanelView.ButtonExit.onClick.AddListener(() =>
        {
            _advertisingPanelView.ButtonExit.onClick.RemoveAllListeners();
            _darkeningBackgroundFrameUIHandler.OpenTranslucent().Forget();
            _advertisingPanelView.gameObject.SetActive(false);
            _globalCanvasCloser.TryDisable();
            _wallet.AddCash(_advertisingPanelView.MonetReward, false);
            _wallet.AddHearts(_advertisingPanelView.HeartsReward, false);
            operationPutExitButton?.Invoke();
        });
        _advertisingPanelView.ButtonExit.gameObject.SetActive(true);
    }
}