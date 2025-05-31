
using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class BackgroundContentCreator : IParticipiteInLoad
{
    private const int _maxPercent = 100;
    private const int _minPercent = 0;
    private const int _minCount = 0;
    private readonly Transform _parent;
    private readonly SpriteRendererAssetProvider _spriteRendererAssetProvider;
    private readonly BackgroundContentAssetProvider _backgroundContentAssetProvider;
    private List<BackgroundContent> _instantiatedBackgroundContent;
    private List<int> _percentsNumbers;
    private BackgroundData _backgroundData;
    public BackgroundContent WardrobeBackground { get; private set; }
    public SpriteRenderer ArtShower { get; private set; }

    private int _contentCount;
    private int _percentCalculatedCount;
    public IReadOnlyList<BackgroundContent> InstantiatedBackgroundContent => _instantiatedBackgroundContent;
    public bool ParticipiteInLoad { get; private set;}
    public int PercentComplete { get; private set; }

    public event Action<BackgroundData> OnCreateContent;
    public BackgroundContentCreator(Transform parent, SpriteRendererAssetProvider spriteRendererAssetProvider)
    {
        _parent = parent;
        _spriteRendererAssetProvider = spriteRendererAssetProvider;
        _backgroundContentAssetProvider = new BackgroundContentAssetProvider();
        PercentComplete = _minPercent;
        ParticipiteInLoad = true;
    }

    public void Dispose()
    {
        // for (int i = 0; i < _instantiatedBackgroundContent.Count; ++i)
        // {
        //     Addressables.ReleaseInstance(_instantiatedBackgroundContent[i].gameObject);
        // }
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
        _contentCount += _backgroundData.BackgroundContentValues.Count;
        InitPercentCalculate();
        
        for (int j = 0; j < _contentCount; ++j)
        {
            _instantiatedBackgroundContent.Add(await _backgroundContentAssetProvider.GetBackgroundContent(_parent));
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
            WardrobeBackground = _instantiatedBackgroundContent[_instantiatedBackgroundContent.Count - 1];
            _instantiatedBackgroundContent.RemoveAt(_instantiatedBackgroundContent.Count - 1);
        }

        PercentComplete = _maxPercent;
        _contentCount = _minCount;
        OnCreateContent?.Invoke(_backgroundData);
        _backgroundData = null;
    }

    private void InitPercentCalculate()
    {
        _percentCalculatedCount = _minCount;
        if (ArtShower == null)
        {
            _percentCalculatedCount++;
        }
        if (_contentCount == _minCount)
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