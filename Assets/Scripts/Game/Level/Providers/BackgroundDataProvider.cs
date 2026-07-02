using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class BackgroundDataProvider
{
    private const string _locations = "Locations";
    private const string _additionalImages = "AdditionalImages";
    private const string _arts = "Arts";
    private const string _compressed = "Compressed";
    private const string _backgroundDataSeriaNameAsset = "BackgroundDataSeria";
    private const string _wardrobeBackgroundDataNameAsset = "WardrobeBackgroundData";
    
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
            _wardrobeBackgroundDataProvider.CreateNames($"{_storyName}{_wardrobeBackgroundDataNameAsset}"));
    }
    public void Shutdown()
    {
        _locationDataProvider.Shutdown();
        _additionalImagesDataProvider.Shutdown();
        _artsDataProvider.Shutdown();
        _wardrobeBackgroundDataProvider.Shutdown();
    }
    public void CheckMatchNumbersSeriaWithNumberAssets(int seriaNumber)
    {
        _locationDataProvider.CheckMatchNumbersSeriaWithNumberAsset(seriaNumber);
        _additionalImagesDataProvider.CheckMatchNumbersSeriaWithNumberAsset(seriaNumber);
        _artsDataProvider.CheckMatchNumbersSeriaWithNumberAsset(seriaNumber);
        _wardrobeBackgroundDataProvider.CheckMatchNumbersSeriaWithNumberAsset(seriaNumber);
    }
    public async UniTask TryLoadDatas()
    {
        await _locationDataProvider.TryLoadData();
        await _additionalImagesDataProvider.TryLoadData();
        await _artsDataProvider.TryLoadData();
        await _wardrobeBackgroundDataProvider.TryLoadData();
    }
    private string GetNameLocations(bool HDMode = false)
    {
        if (HDMode == false)
        {
            return $"{_storyName}{_locations}{_compressed}{_backgroundDataSeriaNameAsset}";
        }
        else
        {
            return $"{_storyName}{_locations}{_backgroundDataSeriaNameAsset}";
        }
    }
    private string GetNameAdditionalImages(bool HDMode = false)
    {
        if (HDMode == false)
        {
            return $"{_storyName}{_additionalImages}{_compressed}{_backgroundDataSeriaNameAsset}";
        }
        else
        {
            return $"{_storyName}{_additionalImages}{_backgroundDataSeriaNameAsset}";
        }
    }
    private string GetNameArts(bool HDMode = false)
    {
        if (HDMode == false)
        {
            return $"{_storyName}{_arts}{_compressed}{_backgroundDataSeriaNameAsset}";
        }
        else
        {
            return $"{_storyName}{_arts}{_backgroundDataSeriaNameAsset}";
        }
    }
}