using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

public class LoadAssetsPercentHandler
{
    private const float _timeDelay = 0.1f;
    private readonly IParticipiteInLoad[] _allPercentProviders;
    private List<IParticipiteInLoad> _percentProvidersParticipiteInLoad;
    private CancellationTokenSource _cancellationTokenSource;
    private ReactiveProperty<int> _currentLoadPercentReactiveProperty;
    private int _sum = 0;
    private bool _isCalculating;
    
    public ReactiveProperty<int> CurrentLoadPercentReactiveProperty => _currentLoadPercentReactiveProperty;
    
    public LoadAssetsPercentHandler(params IParticipiteInLoad[] allPercentProviders)
    {
        _allPercentProviders = allPercentProviders;
        _currentLoadPercentReactiveProperty = new ReactiveProperty<int>();
    }

    public void SetDefault()
    {
        for (int i = 0; i < _allPercentProviders.Length; ++i)
        {
            _allPercentProviders[i].SetDefault();
        }
    }
    public void StartCalculatePercent()
    {
        _percentProvidersParticipiteInLoad = new List<IParticipiteInLoad>();

        for (int i = 0; i < _allPercentProviders.Length; ++i)
        {
            if (_allPercentProviders[i].ParticipiteInLoad == true)
            {
                _percentProvidersParticipiteInLoad.Add(_allPercentProviders[i]);
            }
        }
        if (_percentProvidersParticipiteInLoad.Count > 0)
        {
            _cancellationTokenSource = new CancellationTokenSource();
            _currentLoadPercentReactiveProperty.Value = 0;
            _isCalculating = true;
            CalculatePercent().Forget();
        }
    }

    private async UniTask CalculatePercent()
    {
        while (_isCalculating == true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_timeDelay), cancellationToken: _cancellationTokenSource.Token);
            CalculateLoadPercent();
        }
    }
    public void StopCalculatePercent()
    {
        _isCalculating = false;
        _cancellationTokenSource.Cancel();
        CalculateLoadPercent();
    }
    private void CalculateLoadPercent()
    {
        _sum = 0;
        for (int i = 0; i < _percentProvidersParticipiteInLoad.Count; ++i)
        {
            _sum += _percentProvidersParticipiteInLoad[i].PercentComplete;
        }
        if (_sum > 0)
        {
            _currentLoadPercentReactiveProperty.Value = _sum / _percentProvidersParticipiteInLoad.Count;
        }
        else
        {
            _currentLoadPercentReactiveProperty.Value = 0;
        }
    }
}