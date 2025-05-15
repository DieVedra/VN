
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class BackgroundContentCreator : IParticipiteInLoad
{
    private const float _timeDelay = 0.1f;
    private const int _maxPercent = 100;
    private const int _minPercent = 0;
    private readonly Transform _parent;
    private readonly BackgroundContentAssetProvider _backgroundContentAssetProvider;
    private List<BackgroundContent> _instantiatedBackgroundContent;
    private List<int> _percentsNumbers;
    private CancellationTokenSource _cancellationTokenSource;
    private BackgroundData _backgroundData;
    public BackgroundContent WardrobeBackground { get; private set; }
    public SpriteRenderer ArtShower { get; private set; }

    private int _contentCount;
    private int _percentCalculatedCount;
    public IReadOnlyList<BackgroundContent> InstantiatedBackgroundContent => _instantiatedBackgroundContent;
    public bool ParticipiteInLoad { get; private set;}
    public int PercentComplete { get; private set; }

    public event Action<BackgroundData> OnCreateContent;
    public BackgroundContentCreator(Transform parent)
    {
        _parent = parent;
        _backgroundContentAssetProvider = new BackgroundContentAssetProvider();
        _cancellationTokenSource = new CancellationTokenSource();
        PercentComplete = _minPercent;
        ParticipiteInLoad = true;
    }

    public void Dispose()
    {
        _backgroundContentAssetProvider.ReleaseAllCreatedObjects();
    }

    public void SetCurrentBackgroundData(BackgroundData backgroundData)
    {
        _backgroundData = backgroundData;
    }
    public async UniTask TryCreateBackgroundContent()
    {
        if (_backgroundData == null)
        {
            return;
        }
        if (WardrobeBackground == null)
        {
            _contentCount++;
        }
        _percentCalculatedCount = 0;
        if (ArtShower == null)
        {
            ArtShower = await PrefabsProvider.SpriteRendererAssetProvider.CreateSpriteRendererAsync(_parent);
            _percentCalculatedCount++;
        }
        _contentCount += _backgroundData.BackgroundContentValues.Count;


        if (_contentCount == 0)
        {
            PercentComplete = _maxPercent;
            return;
        }
        else
        {
            _percentCalculatedCount += _contentCount;
            _instantiatedBackgroundContent = new List<BackgroundContent>();
            PercentComplete = _minPercent;
        }

        PercentCalculate().Forget();

        for (int j = 0; j < _contentCount; ++j)
        {
            _instantiatedBackgroundContent.Add(await _backgroundContentAssetProvider.GetBackgroundContent(_parent));
        }
        if (WardrobeBackground == null)
        {
            WardrobeBackground = _instantiatedBackgroundContent[_instantiatedBackgroundContent.Count - 1];
            _instantiatedBackgroundContent.RemoveAt(_instantiatedBackgroundContent.Count - 1);
        }
       
        
        _cancellationTokenSource.Cancel();
        PercentComplete = _maxPercent;
        _contentCount = 0;
        OnCreateContent?.Invoke(_backgroundData);
        _backgroundData = null;
    }

    private async UniTask PercentCalculate()
    {
        _cancellationTokenSource = new CancellationTokenSource();
        int sum;
        _percentsNumbers = new List<int>(_percentCalculatedCount);
        for (int i = 0; i < _percentCalculatedCount; ++i)
        {
            _percentsNumbers.Add(_minPercent);
        }
        while (true)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(_timeDelay), cancellationToken: _cancellationTokenSource.Token);
            sum = _minPercent;
            if (_percentsNumbers.Count >= _instantiatedBackgroundContent.Count)
            {
                _percentsNumbers[_instantiatedBackgroundContent.Count] = _backgroundContentAssetProvider.GetPercentComplete();
            }

            for (int i = 0; i < _percentsNumbers.Count; ++i)
            {
                sum += _percentsNumbers[i];
            }
            
            PercentComplete = sum / _contentCount;
        }
    }
}