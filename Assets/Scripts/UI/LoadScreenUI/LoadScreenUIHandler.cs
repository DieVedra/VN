
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
        _cancellationTokenSource?.Cancel();
        Addressables.ReleaseInstance(_loadScreenUIView.gameObject);
        _indicatorUIHandler.Dispose();
        _blackFrameUIHandler.Dispose();
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
            _loadScreenUIView.LoadScreenImage.raycastTarget = true;
            _loadScreenUIView.LoadScreenMaskImage.raycastTarget = true;
        }
    }
    private async UniTask Show()
    {
        _loadScreenUIView.gameObject.SetActive(false);
        _indicatorUIHandler.StopIndicate();
        _canvas.gameObject.SetActive(true);
        await _blackFrameUIHandler.Close();
        _loadScreenUIView.gameObject.SetActive(true);
        _indicatorUIHandler.SetTextIndicateMode();
        _indicatorUIHandler.StartIndicate();
        await _blackFrameUIHandler.Open();
    }

    public async UniTaskVoid ShowOnMainMenuMove()
    {
        _loadScreenUIView.LoadScreenImage.sprite = _backgrountSpriteDefault;
        _loadScreenUIView.LogoImage.sprite = _logoSpriteDefault;
        await Show();
    }
    public async UniTask ShowOnLevelMove(Sprite loadScreenSprite, Sprite logoImage)
    {
        _loadScreenUIView.LoadScreenImage.sprite = loadScreenSprite;
        _loadScreenUIView.LogoImage.sprite = logoImage;
        await Show();
    }
    public void ShowOnStart()
    {
        _canvas.gameObject.SetActive(true);
        _blackFrameUIHandler.On();
        _loadScreenUIView.gameObject.SetActive(true);
        _indicatorUIHandler.StartIndicate();
        _blackFrameUIHandler.Open().Forget();
    }

    public async UniTask HideOnLevelMove()
    {
        await _blackFrameUIHandler.Close();
        _loadScreenUIView.gameObject.SetActive(false);
        _indicatorUIHandler.StopIndicate();
        _blackFrameUIHandler.Open().Forget();
    }
    
    public async UniTaskVoid HideOnMainMenuMove()
    {
        _cancellationTokenSource = new CancellationTokenSource();

        Vector4 padding = Vector4.zero;
        _loadScreenUIView.LoadScreenImage.raycastTarget = false;
        _loadScreenUIView.LoadScreenMaskImage.raycastTarget = false;

        _indicatorUIHandler.StopIndicate();
        float paddingValue = Screen.width;
        await DOTween.To(() => padding.z, x => padding.z = x, paddingValue, _loadScreenUIView.MaskHideDuration)
            .OnUpdate(() => _loadScreenUIView.RectMask2D.padding = padding).WithCancellation(_cancellationTokenSource.Token);
        // _cancellationTokenSource.Cancel();
        _loadScreenUIView.gameObject.SetActive(false);
        _loadScreenUIView.RectMask2D.padding = Vector4.zero;
        _canvas.gameObject.SetActive(false);
    }
    // public async UniTaskVoid Hide()
    // {
    //     _cancellationTokenSource = new CancellationTokenSource();
    //
    //     Vector4 padding = Vector4.zero;
    //     _loadScreenUIView.LoadScreenImage.raycastTarget = false;
    //     _loadScreenUIView.LoadScreenMaskImage.raycastTarget = false;
    //
    //     float paddingValue = Screen.width;
    //     await DOTween.To(() => padding.z, x => padding.z = x, paddingValue, _loadScreenUIView.MaskHideDuration)
    //         .OnUpdate(() => _loadScreenUIView.RectMask2D.padding = padding).WithCancellation(_cancellationTokenSource.Token);
    //     _indicatorUIHandler.StopIndicate();
    //     // _cancellationTokenSource.Cancel();
    //     _loadScreenUIView.gameObject.SetActive(false);
    //     _loadScreenUIView.RectMask2D.padding = Vector4.zero;
    //     _canvas.gameObject.SetActive(false);
    // }
}