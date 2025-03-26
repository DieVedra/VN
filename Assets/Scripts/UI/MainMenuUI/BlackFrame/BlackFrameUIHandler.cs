
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class BlackFrameUIHandler
{
    private readonly RectTransform _parentRectTransform;
    private readonly Transform _transform;
    private readonly Image _image;
    private CancellationTokenSource _cancellationTokenSource;
    public Transform Transform => _transform;
    public bool IsOpen { get; private set; }
    public BlackFrameUIHandler(BlackFrameView blackFrameView)
    {
        blackFrameView.Image.color = Color.black;
        IsOpen = false;
        blackFrameView.gameObject.SetActive(true);
        _image = blackFrameView.Image;
        _parentRectTransform = blackFrameView.transform.parent.GetComponent<RectTransform>();
        _transform = blackFrameView.transform;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Dispose()
    {
        _cancellationTokenSource.Cancel();
    }

    public void SetAsLastSibling()
    {
        _transform.SetAsLastSibling();
    }
    public void SetSiblingOnIndex(int index)
    {
        _transform.SetSiblingIndex(index);
    }

    public async UniTask Open()
    {
        SetAsLastSibling();
        _image.color = Color.black;
        await DoAnimation(0f, 1f);
        _transform.gameObject.SetActive(false);
        IsOpen = true;
    }

    public async UniTask Close()
    {
        SetAsLastSibling();
        _image.color = Color.clear;
        _transform.gameObject.SetActive(true);
        await DoAnimation(1f, 1f);
        IsOpen = false;
    }
    public async UniTask OpenTranslucent()
    {
        _transform.gameObject.SetActive(true);
        await DoAnimation(0f, 0.5f);
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
        _image.color = new Color(0f,0f,0f,0.5f);
        _transform.gameObject.SetActive(true);
        await _image.DOFade(0.5f, 0.5f).WithCancellation(_cancellationTokenSource.Token);
    }
    private async UniTask DoAnimation(float end, float duration)
    {
        await _image.DOFade(end, duration).WithCancellation(_cancellationTokenSource.Token);
    }
}