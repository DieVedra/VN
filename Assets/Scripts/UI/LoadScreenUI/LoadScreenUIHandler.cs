
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadScreenUIHandler
{
    private readonly Transform _projectContextParent;
    private readonly LoadScreenAssetProvider _loadScreenAssetProvider;
    private BlackFrameUIHandler _blackFrameUIHandler;
    private LoadIndicatorUIHandler _indicatorUIHandler;
    private LoadScreenUIView _loadScreenUIView;
    private CancellationTokenSource _cancellationTokenSource;
    private Sprite _backgrountSpriteDefault;
    private Sprite _logoSpriteDefault;
    private Canvas _canvas;
    private bool _isCreatedOneInstance;
    public Transform ParentMask => _loadScreenUIView.LoadScreenMaskImage.transform;
    public LoadScreenUIHandler(Transform projectContextParent)
    {
        _projectContextParent = projectContextParent;
        _loadScreenAssetProvider = new LoadScreenAssetProvider();
        _isCreatedOneInstance = false;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        Addressables.ReleaseInstance(_loadScreenUIView.gameObject);
    }

    public async UniTask Init(LoadIndicatorUIHandler loadIndicatorUIHandler, BlackFrameUIHandler blackFrameUIHandler)
    {
        if (_isCreatedOneInstance == false)
        {
            var projectContextCanvasAssetProvider = new ProjectContextCanvasAssetProvider();
            _canvas = await projectContextCanvasAssetProvider.LoadAsset(_projectContextParent);
            _canvas.gameObject.SetActive(true);
            var canvasTransform = _canvas.transform;
            _loadScreenUIView = await _loadScreenAssetProvider.LoadAsset(canvasTransform);
            _loadScreenUIView.RectMask2D.padding = Vector4.zero;

            _backgrountSpriteDefault = _loadScreenUIView.LoadScreenImage.sprite;
            _logoSpriteDefault = _loadScreenUIView.LogoImage.sprite;
            _indicatorUIHandler = loadIndicatorUIHandler;
            _blackFrameUIHandler = blackFrameUIHandler;
            await _indicatorUIHandler.Init(canvasTransform);
            await _blackFrameUIHandler.Init(canvasTransform);
            _blackFrameUIHandler.On();
        }
    }
    public void Show(Sprite loadScreenSprite = null, Sprite logoImage = null)
    {
        if (loadScreenSprite != null)
        {
            _loadScreenUIView.LoadScreenImage.sprite = loadScreenSprite;
        }
        else
        {
            _loadScreenUIView.LoadScreenImage.sprite = _backgrountSpriteDefault;
        }

        if (logoImage != null)
        {
            _loadScreenUIView.LogoImage.sprite = logoImage;
        }
        else
        {
            _loadScreenUIView.LogoImage.sprite = _logoSpriteDefault;

        }
        _loadScreenUIView.LoadScreenImage.raycastTarget = true;
        _loadScreenUIView.LoadScreenMaskImage.raycastTarget = true;
        _canvas.gameObject.SetActive(true);

        _loadScreenUIView.gameObject.SetActive(true);
        _indicatorUIHandler.StartIndicate();
        _blackFrameUIHandler.Open().Forget();
    }

    public async UniTaskVoid Hide()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _indicatorUIHandler.StopIndicate();

        Vector4 padding = Vector4.zero;
        _loadScreenUIView.LoadScreenImage.raycastTarget = false;
        _loadScreenUIView.LoadScreenMaskImage.raycastTarget = false;

        float paddingValue = Screen.width;
        await DOTween.To(() => padding.z, x => padding.z = x, paddingValue, _loadScreenUIView.MaskHideDuration)
            .OnUpdate(() => _loadScreenUIView.RectMask2D.padding = padding).WithCancellation(_cancellationTokenSource.Token);
        _cancellationTokenSource.Cancel();
        _loadScreenUIView.gameObject.SetActive(false);
        _canvas.gameObject.SetActive(false);
    }
}