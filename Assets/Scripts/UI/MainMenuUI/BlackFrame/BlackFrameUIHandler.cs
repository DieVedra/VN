
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

public class BlackFrameUIHandler
{
    private Transform _transform;
    private Image _image;
    private BlackFrameView _blackFrameView;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _assetLoaded;
    public Transform Transform => _transform;
    public bool IsOpen { get; private set; }
    public BlackFrameUIHandler()
    {
        IsOpen = false;
    }
    public async UniTask Init(Transform parent)
    {
        if (_assetLoaded == false)
        {
            _blackFrameView = await new BlackFramePanelAssetProvider().CreateBlackFramePanel(parent);
            _image = _blackFrameView.Image;
            _image.color = Color.black;
            _transform = _blackFrameView.transform;
            _assetLoaded = true;
        }
    }
    public void Dispose()
    {
        _cancellationTokenSource?.Cancel();
        Addressables.ReleaseInstance(_blackFrameView.gameObject);
    }

    public void SetAsLastSibling()
    {
        _transform.SetAsLastSibling();
    }
    public void SetSiblingOnIndex(int index)
    {
        _transform.SetSiblingIndex(index);
    }

    public void On()
    {
        _transform.gameObject.SetActive(true);
    }

    public void Off()
    {
        _transform.gameObject.SetActive(false);
    }
    public async UniTask Open()
    {
        _image.color = Color.black;
        await DoAnimation(AnimationValuesProvider.MinValue, AnimationValuesProvider.MaxValue);
        Off();
        IsOpen = true;
    }

    public async UniTask Close()
    {
        _image.color = Color.clear;
        On();
        await DoAnimation(AnimationValuesProvider.MaxValue, AnimationValuesProvider.MaxValue);
        IsOpen = false;
    }
    public async UniTask OpenTranslucent()
    {
        _transform.gameObject.SetActive(true);
        await DoAnimation(AnimationValuesProvider.MinValue, AnimationValuesProvider.HalfValue);
        _transform.gameObject.SetActive(false);
    }

    public async UniTask CloseTranslucent(int sublingIndex)
    {
        SetSiblingOnIndex(sublingIndex);

        await BaseCloseTranslucent();
    }
    public async UniTask CloseTranslucent()
    {
        SetAsLastSibling();
        await BaseCloseTranslucent();
    }

    private async UniTask BaseCloseTranslucent()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _image.color = new Color(AnimationValuesProvider.MinValue,AnimationValuesProvider.MinValue,AnimationValuesProvider.MinValue,AnimationValuesProvider.HalfValue);
        _transform.gameObject.SetActive(true);
        await _image.DOFade(AnimationValuesProvider.HalfValue, AnimationValuesProvider.HalfValue).WithCancellation(_cancellationTokenSource.Token);
    }
    private async UniTask DoAnimation(float end, float duration)
    {
        _cancellationTokenSource = new CancellationTokenSource();
        await _image.DOFade(end, duration).WithCancellation(_cancellationTokenSource.Token);
    }
}