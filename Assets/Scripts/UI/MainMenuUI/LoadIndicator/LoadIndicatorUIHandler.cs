
using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class LoadIndicatorUIHandler
{
    private const float _minRotationValue = 0f;
    private const float _maxRotationValue = -360f;
    private readonly Vector3 _endRotationValue;
    private readonly Vector3 _startRotationValue;
    private readonly float _duration = 3f;
    private readonly float _textIndicateDuration = 0.5f;
    private readonly string _dot = ".";
    private readonly string _loadText = "Загрузка";
    private LoadIndicatorView _loadIndicatorView;
    private LoadIndicatorAssetProvider _loadIndicatorAssetProvider;
    private CancellationTokenSource _cancellationTokenSource;
    
    private bool _assetLoaded;
    private bool _isIndicate;
    private bool _isPercentIndicate;
    private bool _isClearIndicate;
    public Transform Transform => _loadIndicatorView.transform;
    public LoadIndicatorUIHandler()
    {
        _loadIndicatorAssetProvider = new LoadIndicatorAssetProvider();
        _assetLoaded = false;
        _isPercentIndicate = false;
        _isClearIndicate = false;
        _endRotationValue = new Vector3(_minRotationValue, _minRotationValue, _maxRotationValue);
        _startRotationValue = new Vector3(_minRotationValue, _minRotationValue, _minRotationValue);
    }

    public void Dispose()
    {
        StopIndicate();
        Addressables.ReleaseInstance(_loadIndicatorView.gameObject);
        _assetLoaded = false;
    }

    public void StopIndicate()
    {
        _cancellationTokenSource?.Cancel();
        _isIndicate = false;
        _loadIndicatorView.gameObject.SetActive(false);
    }

    public async UniTask Init(Transform parent)
    {
        if (_assetLoaded == false)
        {
            _loadIndicatorView = await _loadIndicatorAssetProvider.CreateIndicatorView(parent);
            _assetLoaded = true;
        }

        _loadIndicatorView.RectTransformIcon.rotation = Quaternion.Euler(_startRotationValue);
    }

    public void SetPercentIndicateMode(int lastValue)
    {
        _loadIndicatorView.LoadText.alignment = TextAlignmentOptions.Center;
        _isPercentIndicate = true;
        _loadIndicatorView.LoadText.gameObject.SetActive(true);
        TextPercentIndicate(lastValue);
    }

    public void SetClearIndicateMode()
    {
        _loadIndicatorView.LoadText.gameObject.SetActive(false);
        _isClearIndicate = true;
    }

    public void TextPercentIndicate(int value)
    {
        _loadIndicatorView.LoadText.text = $"{value}%";
    }

    public void StartIndicate()
    {
        if (_isIndicate == false)
        {
            _isIndicate = true;
            _cancellationTokenSource = new CancellationTokenSource();

            // _loadIndicatorView.transform.SetAsLastSibling();
            _loadIndicatorView.gameObject.SetActive(true);
            _loadIndicatorView.RectTransformIcon.DORotate(_endRotationValue, _duration, RotateMode.FastBeyond360).SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Restart)
                .WithCancellation(_cancellationTokenSource.Token);
            if (_isPercentIndicate == false && _isClearIndicate == false)
            {
                TextIndicate().Forget();
            }
        }
    }

    private async UniTaskVoid TextIndicate()
    {
        _isIndicate = true;
        string[] dots = new[] { $"{_loadText}{_dot}", $"{_loadText}{_dot}{_dot}", $"{_loadText}{_dot}{_dot}{_dot}"};
        int index = dots.Length - 1;
        while (_isIndicate == true)
        {
            _loadIndicatorView.LoadText.text = dots[index];
            await UniTask.Delay(TimeSpan.FromSeconds(_textIndicateDuration), cancellationToken: _cancellationTokenSource.Token);
            if (index == dots.Length -1)
            {
                index = 0;
            }
            else
            {
                index++;
            }
        }
    }
}