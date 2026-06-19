using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class BackgroundDataProvider
{
    public const string Locations = "Locations";
    public const string AdditionalImages = "AdditionalImages";
    public const string Arts = "Arts";
    public const string Compressed = "Compressed";
    public const string BackgroundDataSeriaNameAsset = "BackgroundDataSeria";
    public const string WardrobeBackgroundDataNameAsset = "WardrobeBackgroundData";
    
    private readonly string _storyName;
    
    private readonly DataProvider<BackgroundData> _locationDataProvider;
    private readonly DataProvider<BackgroundData> _additionalImagesDataProvider;
    private readonly DataProvider<BackgroundData> _artsDataProvider;
    private readonly DataProvider<BackgroundData> _wardrobeBackgroundDataProvider;
    
    public IParticipiteInLoad LocationDataLoadProviderParticipiteInLoad => _locationDataProvider;
    public IParticipiteInLoad AdditionalImagesDataLoadProviderParticipiteInLoad => _additionalImagesDataProvider;
    public IParticipiteInLoad ArtsDataLoadProviderParticipiteInLoad => _artsDataProvider;
    public IParticipiteInLoad WardrobeBackgroundDataLoadProviderParticipiteInLoad => _wardrobeBackgroundDataProvider;
    
    public IReactiveCommand<BackgroundData> OnLoadLocationData => _locationDataProvider.OnLoad;
    public IReactiveCommand<BackgroundData> OnLoadAdditionalImagesData => _additionalImagesDataProvider.OnLoad;
    public IReactiveCommand<BackgroundData> OnLoadArtsData => _artsDataProvider.OnLoad;
    public IReactiveCommand<BackgroundData> OnLoadWardrobeData => _wardrobeBackgroundDataProvider.OnLoad;


    public BackgroundDataProvider(string storyName)
    {
        _storyName = storyName;
        _locationDataProvider = new DataProvider<BackgroundData>();
        _additionalImagesDataProvider = new DataProvider<BackgroundData>();
        _artsDataProvider = new DataProvider<BackgroundData>();
        _wardrobeBackgroundDataProvider = new DataProvider<BackgroundData>();
    }

    public async UniTask Init()
    {
        await UniTask.WhenAll(
            _locationDataProvider.CreateNames(GetNameLocations()),
            _additionalImagesDataProvider.CreateNames(GetNameAdditionalImages()),
            _artsDataProvider.CreateNames(GetNameArts()),
            _wardrobeBackgroundDataProvider.CreateNames(WardrobeBackgroundDataNameAsset));
    }
    public void Shutdown()
    {
        _locationDataProvider.Shutdown();
        _additionalImagesDataProvider.Shutdown();
        _artsDataProvider.Shutdown();
        _wardrobeBackgroundDataProvider.Shutdown();
    }
    public void CheckMatchNumbersSeriaWithNumberAssets(int seriaNumber, int seriaNameAssetIndex)
    {

        _locationDataProvider.CheckMatchNumbersSeriaWithNumberAsset(seriaNumber, seriaNameAssetIndex);
        _additionalImagesDataProvider.CheckMatchNumbersSeriaWithNumberAsset(seriaNumber, seriaNameAssetIndex);
        _artsDataProvider.CheckMatchNumbersSeriaWithNumberAsset(seriaNumber, seriaNameAssetIndex);
        _wardrobeBackgroundDataProvider.CheckMatchNumbersSeriaWithNumberAsset(seriaNumber, seriaNameAssetIndex);
    }
    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        await _locationDataProvider.TryLoadData(nextSeriaNameAssetIndex);
        await _additionalImagesDataProvider.TryLoadData(nextSeriaNameAssetIndex);
        await _artsDataProvider.TryLoadData(nextSeriaNameAssetIndex);
        await _wardrobeBackgroundDataProvider.TryLoadData(nextSeriaNameAssetIndex);
    }
    private string GetNameLocations(bool HDMode = false)
    {
        if (HDMode == false)
        {
            return $"{_storyName}{Locations}{Compressed}{BackgroundDataSeriaNameAsset}";
        }
        else
        {
            return $"{_storyName}{Locations}{BackgroundDataSeriaNameAsset}";
        }
    }
    private string GetNameAdditionalImages(bool HDMode = false)
    {
        if (HDMode == false)
        {
            return $"{_storyName}{AdditionalImages}{Compressed}{BackgroundDataSeriaNameAsset}";
        }
        else
        {
            return $"{_storyName}{AdditionalImages}{BackgroundDataSeriaNameAsset}";
        }
    }
    private string GetNameArts(bool HDMode = false)
    {
        if (HDMode == false)
        {
            return $"{_storyName}{Arts}{Compressed}{BackgroundDataSeriaNameAsset}";
        }
        else
        {
            return $"{_storyName}{Arts}{BackgroundDataSeriaNameAsset}";
        }
    }
}