using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class ContactPrintStatusHandler
{
    private const float _minTime = 0.5f;
    private const float _maxTime = 1.5f;
    private const float _minSymbol = 0f;
    private const float _maxSymbol = 150f;
    private const float _indicateDelay = 0.2f;
    private const float _delayMultiply = 2f;
    public readonly LocalizationString PrintLocalizationString = "печатает";
    private readonly IReadOnlyList<Image> _printIndicator;
    private readonly GameObject _onlineStatus;
    private readonly GameObject _printStatus;
    private readonly TextMeshProUGUI _text;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private CompositeDisposable _compositeDisposable;
    private bool _isRun;

    public ContactPrintStatusHandler(IReadOnlyList<Image> printIndicator, GameObject onlineStatus, TextMeshProUGUI text)
    {
        _printIndicator = printIndicator;
        _onlineStatus = onlineStatus;
        _printStatus = text.transform.parent.gameObject;
        _text = text;
        IndicatorOff();
        _isRun = false;
        _cancellationTokenSource = new CancellationTokenSource();
    }

    public void Shutdown()
    {
        _cancellationTokenSource?.Cancel();
    }
    private void IndicatorOff()
    {
        for (int i = 0; i < _printIndicator.Count; i++)
        {
            _printIndicator[i].gameObject.SetActive(false);
        }
    }
    public async UniTask IndicateOnPrint(SetLocalizationChangeEvent setLocalizationChangeEvent, int symbolCount, bool longerKey = false)
    {
        _compositeDisposable = setLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            _text.text = PrintLocalizationString;
        });

        bool onlineStatus = _onlineStatus.activeSelf;
        if (onlineStatus == true)
        {
            _onlineStatus.SetActive(false);
        }
        _printStatus.SetActive(true);
        _isRun = true;
        Indicate().Forget();
        float time = CalculateTime(symbolCount);
        if (longerKey == true)
        {
            time *= _delayMultiply;
        }
        await UniTask.Delay(TimeSpan.FromSeconds(time), cancellationToken: _cancellationTokenSource.Token);
        _isRun = false;
        _printStatus.SetActive(false);
        if (onlineStatus == true)
        {
            _onlineStatus.SetActive(true);
        }
        _compositeDisposable.Clear();
    }

    private async UniTask Indicate()
    {
        int index = 0;
        while (_isRun == true)
        {
            if (index < _printIndicator.Count)
            {
                Activate();
                await UniTask.Delay(TimeSpan.FromSeconds(_indicateDelay), cancellationToken: _cancellationTokenSource.Token);
            }
            else
            {
                IndicatorOff();
                index = 0;
            }
        }
        void Activate()
        {
            _printIndicator[index].gameObject.SetActive(true);
            index++;
        }
    }

    private float CalculateTime(int symbolCount)
    {
        float t = Mathf.InverseLerp(_minSymbol, _maxSymbol, symbolCount);
        return Mathf.Lerp(_minTime, _maxTime, 1 - Mathf.Pow(1 -t, 4)); // t - easeOutQuart formula
    }
}