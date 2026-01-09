using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BackgroundContentCreator : IParticipiteInLoad
{
    private const int _maxPercent = 100;
    private const int _minPercent = 0;
    private const int _minCount = 0;
    private readonly Transform _parent;
    private readonly SpriteRendererAssetProvider _spriteRendererAssetProvider;
    private readonly BackgroundContentAssetProvider _backgroundContentAssetProvider;
    private readonly Queue<BackgroundContent> _instantiatedBackgroundContent;
    private List<int> _percentsNumbers;
    private BackgroundData _backgroundData;
    public BackgroundContent WardrobeBackground { get; private set; }
    public SpriteRenderer ArtShower { get; private set; }

    private int _createContentCount;
    private int _percentCalculatedCount;
    public bool ParticipiteInLoad { get; private set;}
    public int PercentComplete { get; private set; }

    public event Action<BackgroundData> OnCreateContent;

    public BackgroundContentCreator(Transform parent, SpriteRendererAssetProvider spriteRendererAssetProvider)
    {
        _parent = parent;
        _spriteRendererAssetProvider = spriteRendererAssetProvider;
        _backgroundContentAssetProvider = new BackgroundContentAssetProvider();
        PercentComplete = _minPercent;
        _instantiatedBackgroundContent = new Queue<BackgroundContent>();
        ParticipiteInLoad = true;
    }

    public void Shutdown()
    {
        _backgroundContentAssetProvider.Abort();
        _spriteRendererAssetProvider.Abort();
        ParticipiteInLoad = false;
    }
    public BackgroundContent TryGetInstantiatedBackgroundContent()
    {
        if (_instantiatedBackgroundContent.Count > 0)
        {
            return _instantiatedBackgroundContent.Dequeue();
        }
        else
        {
            return default;
        }
    }
    public void SetDefault()
    {
        PercentComplete = _minPercent;
    }

    public void SetCurrentBackgroundData(BackgroundData backgroundData)
    {
        _backgroundData = backgroundData;
    }
    public async UniTask TryCreateBackgroundContent()
    {
        if (ParticipiteInLoad == true)
        {
            if (_backgroundData == null)
            {
                return;
            }
            if (WardrobeBackground == null)
            {
                _createContentCount++;
            }

            _createContentCount += _backgroundData.BackgroundContentValues.Count;
            InitPercentCalculate();
            for (int j = 0; j < _createContentCount; ++j)
            {
                _instantiatedBackgroundContent.Enqueue(await _backgroundContentAssetProvider.GetBackgroundContent(_parent));
                _percentsNumbers[j] = _maxPercent;
                UpdatePercentComplete();
            }
            if (ArtShower == null)
            {
                ArtShower = await _spriteRendererAssetProvider.CreateSpriteRendererAsync(_parent);
                _percentsNumbers[_percentsNumbers.Count - 1] = _maxPercent;
                UpdatePercentComplete();
            }

            if (WardrobeBackground == null)
            {
                WardrobeBackground = _instantiatedBackgroundContent.Dequeue();
            }

            PercentComplete = _maxPercent;
            _createContentCount = _minCount;
            OnCreateContent?.Invoke(_backgroundData);
            _backgroundData = null;
        }
    }
    private void InitPercentCalculate()
    {
        _percentCalculatedCount = _minCount;
        if (ArtShower == null)
        {
            _percentCalculatedCount++;
        }
        
        if (_createContentCount == _minCount)
        {
            PercentComplete = _maxPercent;
            return;
        }
        else
        {
            _percentCalculatedCount += _createContentCount;
            PercentComplete = _minPercent;
        }
        _percentsNumbers = new List<int>(_percentCalculatedCount);
        for (int i = 0; i < _percentCalculatedCount; ++i)
        {
            _percentsNumbers.Add(_minPercent);
        }
    }

    private void UpdatePercentComplete()
    {
        int sum = _minCount;

        for (int i = 0; i < _percentsNumbers.Count; ++i)
        {
            sum += _percentsNumbers[i];
        }
        PercentComplete = sum / _percentsNumbers.Count;
    }
}