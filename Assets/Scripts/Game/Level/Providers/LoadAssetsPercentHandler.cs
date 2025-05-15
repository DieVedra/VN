
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LoadAssetsPercentHandler
{
    private const float _timeDelay = 0.1f;
    private readonly IParticipiteInLoad[] _allPercentProviders;
    private List<IParticipiteInLoad> _percentProvidersParticipiteInLoad;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isCalculating;
    public int CurrentLoadPercent { get; private set; }


    public LoadAssetsPercentHandler(params IParticipiteInLoad[] allPercentProviders)
    {
        _allPercentProviders = allPercentProviders;
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
            CurrentLoadPercent = 0;
            _isCalculating = true;
            Debug.Log($"StartCalculatePercent {CurrentLoadPercent}");

            CalculatePercent().Forget();
        }
    }

    private async UniTask CalculatePercent()
    {
        while (_isCalculating == true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_timeDelay), cancellationToken: _cancellationTokenSource.Token);
            CalculateLoadPercent();
            // Debug.Log($"CurrentLoadPercent: {CurrentLoadPercent}");
        }
    }
    public void StopCalculatePercent()
    {
        Debug.Log($"StopCalculatePercent {CurrentLoadPercent}");
        _isCalculating = false;
        _cancellationTokenSource.Cancel();
    }
    private void CalculateLoadPercent()
    {
        int sum = 0;
        for (int i = 0; i < _percentProvidersParticipiteInLoad.Count; ++i)
        {
            sum += _percentProvidersParticipiteInLoad[i].PercentComplete;
        }

        if (sum > 0)
        {
            CurrentLoadPercent = sum / _percentProvidersParticipiteInLoad.Count;
        }
        else
        {
            CurrentLoadPercent = 0;
        }
    }
}