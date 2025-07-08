using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class LoadIndicatorUIHandler
{
    private const float _minRotationValue = 0f;
    private const float _maxRotationValue = -360f;
    private const float _duration = 3f;
    private const string _dot = ".";
    private readonly Vector3 _endRotationValue;
    private readonly Vector3 _startRotationValue;
    private readonly LocalizationString _loadTextLocalization  = "Загрузка";
    private readonly LocalizationString _loadLocalizationTextLocalization  = "Загрузка языка";
    private LoadIndicatorView _loadIndicatorView;
    private LoadIndicatorAssetProvider _loadIndicatorAssetProvider;
    private CancellationTokenSource _cancellationTokenSource;
    private Transform _parent;
    private bool _assetLoaded;
    private bool _isIndicate;
    private bool _isPercentIndicate;
    private bool _isLocalizationIndicate;
    private bool _isClearIndicate;
    public Transform Transform => _loadIndicatorView.transform;
    public LoadIndicatorUIHandler()
    {
        _loadIndicatorAssetProvider = new LoadIndicatorAssetProvider();
        _assetLoaded = false;
        SkipAllModes();
        _endRotationValue = new Vector3(_minRotationValue, _minRotationValue, _maxRotationValue);
        _startRotationValue = new Vector3(_minRotationValue, _minRotationValue, _minRotationValue);
    }

    public void Dispose()
    {
        StopIndicate();
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
            _parent = parent;
            _assetLoaded = true;
        }

        _loadIndicatorView.RectTransformIcon.rotation = Quaternion.Euler(_startRotationValue);
    }

    public void SetPercentIndicateMode(int lastValue)
    {
        _loadIndicatorView.LoadText.alignment = TextAlignmentOptions.Center;
        SkipAllModes();
        _isPercentIndicate = true;
        _loadIndicatorView.LoadText.gameObject.SetActive(true);
        TextPercentIndicate(lastValue);
    }

    public void SetLocalizationIndicate()
    {
        SkipAllModes();
        _isLocalizationIndicate = true;
    }
    public void SetClearIndicateMode()
    {
        _loadIndicatorView.LoadText.gameObject.SetActive(false);
        SkipAllModes();
        _isClearIndicate = true;
    }
    public void SetTextIndicateMode()
    {
        _loadIndicatorView.LoadText.gameObject.SetActive(false);
        SkipAllModes();
    }
    public void TextPercentIndicate(int value)
    {
        _loadIndicatorView.LoadText.text = $"{value}%";
    }

    public void StartIndicate()
    {
        if (_isIndicate == false)
        {
            if (_parent.gameObject.activeSelf == false)
            {
                _parent.gameObject.SetActive(true);
            }
            _isIndicate = true;
            _cancellationTokenSource = new CancellationTokenSource();

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
        _loadIndicatorView.LoadText.text = String.Empty;
        _loadIndicatorView.LoadText.gameObject.SetActive(true);
        string[] dots = null;
        if (_isLocalizationIndicate == false)
        {
            dots = new[] { $"{_loadTextLocalization}{_dot}", $"{_loadTextLocalization}{_dot}{_dot}", $"{_loadTextLocalization}{_dot}{_dot}{_dot}"};
        }
        else
        {
            dots = new[] { $"{_loadLocalizationTextLocalization}{_dot}", $"{_loadLocalizationTextLocalization}{_dot}{_dot}", $"{_loadLocalizationTextLocalization}{_dot}{_dot}{_dot}"};
        }
        int index = dots.Length - 1;
        while (_isIndicate == true)
        {
            _loadIndicatorView.LoadText.text = dots[index];
            await UniTask.Delay(TimeSpan.FromSeconds(AnimationValuesProvider.HalfValue), cancellationToken: _cancellationTokenSource.Token);
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

    private void SkipAllModes()
    {
        _isIndicate = false;
        _isPercentIndicate = false;
        _isLocalizationIndicate = false;
        _isClearIndicate = false;
    }
}