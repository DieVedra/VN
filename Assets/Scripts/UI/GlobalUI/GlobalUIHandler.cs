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
    private Transform _canvasTransform;
    private bool _isCreatedOneInstance;

    public Transform GlobalUITransforn => _canvasTransform;
    public LoadScreenUIHandler LoadScreenUIHandler => _loadScreenUIHandler;
    public BlackFrameUIHandler BlackFrameUIHandler => _blackFrameUIHandler;
    public LoadIndicatorUIHandler LoadIndicatorUIHandler => _loadIndicatorUIHandler;
    public SettingsPanelUIHandler SettingsPanelUIHandler => _settingsPanelUIHandler;
    public ShopMoneyPanelUIHandler ShopMoneyPanelUIHandler => _shopMoneyPanelUIHandler;

    public GlobalUIHandler(Transform projectContextParent)
    {
        _projectContextParent = projectContextParent;
        _isCreatedOneInstance = false;
    }

    public async UniTask Init(LoadScreenUIHandler loadScreenUIHandler, SettingsPanelUIHandler settingsPanelUIHandler,
        ShopMoneyPanelUIHandler shopMoneyPanelUIHandler, LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler)
    {
        if (_isCreatedOneInstance == false)
        {
            _isCreatedOneInstance = true;
            _loadScreenUIHandler = loadScreenUIHandler;
            _loadIndicatorUIHandler = loadIndicatorUIHandler;
            _blackFrameUIHandler = blackFrameUIHandler;
            _settingsPanelUIHandler = settingsPanelUIHandler;
            _shopMoneyPanelUIHandler = shopMoneyPanelUIHandler;
            if (_canvasTransform == null)
            {
                var projectContextCanvasAssetProvider = new ProjectContextCanvasAssetProvider();
                Canvas canvas = await projectContextCanvasAssetProvider.LoadAsset(_projectContextParent);
                _canvasTransform = canvas.transform;
                _canvasTransform.gameObject.SetActive(true);
            }
            shopMoneyPanelUIHandler.Init(_canvasTransform);
            await loadScreenUIHandler.Init(_canvasTransform, loadIndicatorUIHandler, blackFrameUIHandler);
            await loadIndicatorUIHandler.Init(_canvasTransform);
            await blackFrameUIHandler.Init(_canvasTransform);
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