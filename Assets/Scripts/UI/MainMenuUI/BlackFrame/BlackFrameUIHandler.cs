using System;
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
    private CurtainUI _curtainUI;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _assetLoaded;
    public Transform Transform => _transform;
    public bool IsOpen { get; private set; }
    public BlackFrameView BlackFrameView => _blackFrameView;
    public BlackFrameUIHandler(BlackFrameView blackFrameView = null)
    {
        IsOpen = false;
        if (blackFrameView != null)
        {
            _blackFrameView = blackFrameView;
            InitFrameView();
        }
    }
    public async UniTask Init(Transform parent)
    {
        if (_assetLoaded == false)
        {
            _blackFrameView = await new BlackFramePanelAssetProvider().CreateBlackFramePanel(parent);
            InitFrameView();
        }
    }

    private void InitFrameView()
    {
        _image = _blackFrameView.Image;
        _image.color = Color.black;
        _transform = _blackFrameView.transform;
        _assetLoaded = true;
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
        _cancellationTokenSource = new CancellationTokenSource();
        _image.color = Color.black;
        await DoAnimation(_cancellationTokenSource,AnimationValuesProvider.MinValue, AnimationValuesProvider.MaxValue);
        Off();
        IsOpen = true;
    }

    public async UniTask Close()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        _image.color = Color.clear;
        On();
        await DoAnimation(_cancellationTokenSource, AnimationValuesProvider.MaxValue, AnimationValuesProvider.MaxValue);
        IsOpen = false;
    }
    public async UniTask OpenTranslucent(float delay = 0f)
    {
        _transform.gameObject.SetActive(true);
        _cancellationTokenSource = new CancellationTokenSource();
        if (delay > 0)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(delay), cancellationToken: _cancellationTokenSource.Token);
        }
        await DoAnimation(_cancellationTokenSource, AnimationValuesProvider.MinValue, AnimationValuesProvider.HalfValue);
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
        await DoAnimation(_cancellationTokenSource, AnimationValuesProvider.HalfValue, AnimationValuesProvider.HalfValue);
    }
    private async UniTask DoAnimation(CancellationTokenSource cancellationTokenSource, float end, float duration)
    {
        await _image.DOFade(end, duration).WithCancellation(cancellationTokenSource.Token);
    }
}