
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

public class LoadScreenUIHandler
{
    private readonly LoadScreenAssetProvider _loadScreenAssetProvider;
    private LoadScreenUIView _loadScreenUIView;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _assetLoaded;
    public Transform ParentMask => _loadScreenUIView.LoadScreenMaskImage.transform;
    public LoadScreenUIHandler()
    {
        _loadScreenAssetProvider = new LoadScreenAssetProvider();
        _assetLoaded = false;
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
        _loadScreenAssetProvider.UnloadAsset();
        _assetLoaded = false;
    }

    public void SetAsLastSibling()
    {
        _loadScreenUIView.transform.SetAsLastSibling();
    }
    public async UniTask Init(Transform parent, Sprite loadScreenSprite = null, Sprite logoImage = null)
    {
        if (_assetLoaded == false)
        {
            _loadScreenUIView = await _loadScreenAssetProvider.LoadAsset(parent);
            if (loadScreenSprite != null)
            {
                _loadScreenUIView.LoadScreenImage.sprite = loadScreenSprite;
            }

            if (logoImage != null)
            {
                _loadScreenUIView.LogoImage.sprite = logoImage;
            }
            _assetLoaded = true;
        }
        _cancellationTokenSource = new CancellationTokenSource();
    }
    public void Show()
    {
        _loadScreenUIView.gameObject.SetActive(true);
    }

    public async UniTask Hide()
    {
        Vector4 padding = Vector4.zero;
        _loadScreenUIView.LoadScreenImage.raycastTarget = false;
        _loadScreenUIView.LoadScreenMaskImage.raycastTarget = false;
        await DOTween.To(() => padding.z, x => padding.z = x, _loadScreenUIView.PaddingValue, _loadScreenUIView.MaskHideDuration)
            .OnUpdate(() => _loadScreenUIView.RectMask2D.padding = padding).WithCancellation(_cancellationTokenSource.Token);

        
        _cancellationTokenSource.Cancel();
        _loadScreenUIView.gameObject.SetActive(false);
    }
}