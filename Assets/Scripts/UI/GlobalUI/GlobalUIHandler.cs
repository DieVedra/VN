using Cysharp.Threading.Tasks;
using UnityEngine;

public class GlobalUIHandler
{
    private readonly Transform _projectContextParent;
    private LoadScreenUIHandler _loadScreenUIHandler;
    private LoadIndicatorUIHandler _loadIndicatorUIHandler;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private SettingsPanelUIHandler _settingsPanelUIHandler;
    private ShopMoneyPanelUIHandler _shopMoneyPanelUIHandler;
    private AdvertisingButtonUIHandler _advertisingButtonUIHandler;
    private ConfirmedPanelUIHandler _confirmedPanelUIHandler;
    private Transform _canvasTransform;
    private GlobalCanvasCloser _globalCanvasCloser;
    private bool _isCreatedOneInstance;

    public Transform GlobalUITransforn => _canvasTransform;
    public LoadScreenUIHandler LoadScreenUIHandler => _loadScreenUIHandler;
    public BlackFrameUIHandler BlackFrameUIHandler => _blackFrameUIHandler;
    public LoadIndicatorUIHandler LoadIndicatorUIHandler => _loadIndicatorUIHandler;
    public SettingsPanelUIHandler SettingsPanelUIHandler => _settingsPanelUIHandler;
    public ShopMoneyPanelUIHandler ShopMoneyPanelUIHandler => _shopMoneyPanelUIHandler;
    public AdvertisingButtonUIHandler AdvertisingButtonUIHandler => _advertisingButtonUIHandler;
    public ConfirmedPanelUIHandler ConfirmedPanelUIHandler => _confirmedPanelUIHandler;

    public GlobalCanvasCloser GlobalCanvasCloser => _globalCanvasCloser;

    public GlobalUIHandler(Transform projectContextParent)
    {
        _projectContextParent = projectContextParent;
        _isCreatedOneInstance = false;
        _globalCanvasCloser = new GlobalCanvasCloser();
    }

    public async UniTask Init(LoadScreenUIHandler loadScreenUIHandler, SettingsPanelUIHandler settingsPanelUIHandler,
        ShopMoneyPanelUIHandler shopMoneyPanelUIHandler, AdvertisingButtonUIHandler advertisingButtonUIHandler,
        LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler)
    {
        if (_isCreatedOneInstance == false)
        {
            _isCreatedOneInstance = true;
            _loadScreenUIHandler = loadScreenUIHandler;
            _loadIndicatorUIHandler = loadIndicatorUIHandler;
            _blackFrameUIHandler = blackFrameUIHandler;
            _settingsPanelUIHandler = settingsPanelUIHandler;
            _shopMoneyPanelUIHandler = shopMoneyPanelUIHandler;
            _advertisingButtonUIHandler = advertisingButtonUIHandler;
            _confirmedPanelUIHandler = new ConfirmedPanelUIHandler();
            if (_canvasTransform == null)
            {
                var projectContextCanvasAssetProvider = new ProjectContextCanvasAssetProvider();
                Canvas canvas = await projectContextCanvasAssetProvider.LoadAsset(_projectContextParent);
                _globalCanvasCloser.Init(canvas.gameObject);

                _canvasTransform = canvas.transform;
                _canvasTransform.gameObject.SetActive(true);
            }

            await loadIndicatorUIHandler.Init(_canvasTransform);
            await blackFrameUIHandler.Init(_canvasTransform);
            _confirmedPanelUIHandler.Init(_canvasTransform, _globalCanvasCloser, loadIndicatorUIHandler, blackFrameUIHandler);
            advertisingButtonUIHandler.Init(_canvasTransform, _globalCanvasCloser, loadIndicatorUIHandler, blackFrameUIHandler);
            shopMoneyPanelUIHandler.Init(_canvasTransform, _globalCanvasCloser, advertisingButtonUIHandler, _confirmedPanelUIHandler);
            await loadScreenUIHandler.Init(_canvasTransform, loadIndicatorUIHandler, blackFrameUIHandler, _globalCanvasCloser);
        }
    }
    public void Shutdown()
    {
         _loadScreenUIHandler.Shutdown();
         _blackFrameUIHandler.Shutdown();
         _loadIndicatorUIHandler.Shutdown();
         _settingsPanelUIHandler.Shutdown();
         _shopMoneyPanelUIHandler.Shutdown();
    }
}