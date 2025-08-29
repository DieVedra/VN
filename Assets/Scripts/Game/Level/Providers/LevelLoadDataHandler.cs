using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class LevelLoadDataHandler
{
    public const int IndexFirstSeriaData = 0;
    private const int _indexFirstName = 0;
    private readonly MainMenuLocalizationHandler _mainMenuLocalizationHandler;
    public readonly CharacterProviderBuildMode CharacterProviderBuildMode;
    public readonly SeriaGameStatsProviderBuild SeriaGameStatsProviderBuild;
    public readonly WardrobeSeriaDataProviderBuildMode WardrobeSeriaDataProviderBuildMode;
    public readonly GameSeriesProvider GameSeriesProvider;
    public readonly AudioClipProvider AudioClipProvider;
    public readonly BackgroundDataProvider BackgroundDataProvider;
    private readonly BackgroundContentCreator _backgroundContentCreator;
    
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    private readonly CurrentSeriaLoadedNumberProperty<int> _currentSeriaLoadedNumberProperty;
    private readonly OnContentIsLoadProperty<bool> _onContentIsLoadProperty;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;
    private int _seriesCount;
    private int _currentSeriaLoadedNumber => _currentSeriaLoadedNumberProperty.GetValue;
    public int CurrentLoadPercent => _loadAssetsPercentHandler.CurrentLoadPercentReactiveProperty.Value;
    public LoadAssetsPercentHandler LoadAssetsPercentHandler => _loadAssetsPercentHandler;

    public LevelLoadDataHandler(MainMenuLocalizationHandler mainMenuLocalizationHandler, BackgroundContentCreator backgroundContentCreator,
        LevelLocalizationProvider levelLocalizationProvider, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,  
        CurrentSeriaLoadedNumberProperty<int> currentSeriaLoadedNumberProperty,
        OnContentIsLoadProperty<bool> onContentIsLoadProperty, int numberFirstSeria = 1)
    {
        _mainMenuLocalizationHandler = mainMenuLocalizationHandler;
        _backgroundContentCreator = backgroundContentCreator;
        _levelLocalizationProvider = levelLocalizationProvider;
        _currentSeriaLoadedNumberProperty = currentSeriaLoadedNumberProperty;
        _currentSeriaLoadedNumberProperty.SetValue(numberFirstSeria);
        _onContentIsLoadProperty = onContentIsLoadProperty;
        SeriaGameStatsProviderBuild = new SeriaGameStatsProviderBuild();
        CharacterProviderBuildMode = new CharacterProviderBuildMode();
        WardrobeSeriaDataProviderBuildMode = new WardrobeSeriaDataProviderBuildMode();
        GameSeriesProvider = new GameSeriesProvider();
        AudioClipProvider = new AudioClipProvider();
        BackgroundDataProvider = new BackgroundDataProvider();
        _loadAssetsPercentHandler = new LoadAssetsPercentHandler(
            CharacterProviderBuildMode.CharactersDataProviderParticipiteInLoad,
            CharacterProviderBuildMode.CustomizableCharacterDataProviderParticipiteInLoad,
            WardrobeSeriaDataProviderBuildMode,
            GameSeriesProvider, SeriaGameStatsProviderBuild,
            AudioClipProvider.AmbientAudioDataProviderParticipiteInLoad,
            AudioClipProvider.MusicAudioDataProviderParticipiteInLoad,
            BackgroundDataProvider.ArtsDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.LocationDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.AdditionalImagesDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.WardrobeBackgroundDataLoadProviderParticipiteInLoad,
            _backgroundContentCreator, _levelLocalizationProvider);
        switchToNextSeriaEvent.Subscribe(OnSwitchToNextSeria);
    }

    public void Dispose()
    {
        _loadAssetsPercentHandler.StopCalculatePercent();
        _levelLocalizationProvider.LocalizationFileProvider.AbortLoad();
        WardrobeSeriaDataProviderBuildMode.Dispose();
        CharacterProviderBuildMode.Dispose();
        GameSeriesProvider.Dispose();
        AudioClipProvider.Dispose();
        BackgroundDataProvider.Dispose();
        SeriaGameStatsProviderBuild.Dispose();
        _backgroundContentCreator.Dispose();
    }
    public async UniTask LoadStartSeriaContent()
    {
        BackgroundDataProvider.OnLoadLocationData.Subscribe(_ =>
        {
            _backgroundContentCreator.SetCurrentBackgroundData(_);
        });
        
        await InitLoaders();
        CheckMatchNumbersSeriaWithNumberAssets(_currentSeriaLoadedNumber, _indexFirstName);
        _loadAssetsPercentHandler.StartCalculatePercent();
        await LoadCurrentLocalization(_currentSeriaLoadedNumber);
        await GameSeriesProvider.TryLoadData(_indexFirstName);
        await SeriaGameStatsProviderBuild.TryLoadData(_indexFirstName);
        await BackgroundDataProvider.TryLoadDatas(_indexFirstName);
        await _backgroundContentCreator.TryCreateBackgroundContent();
        await WardrobeSeriaDataProviderBuildMode.TryLoadData(_indexFirstName);
        await CharacterProviderBuildMode.TryLoadDatas(_indexFirstName);
        await AudioClipProvider.TryLoadDatas(_indexFirstName);
        _loadAssetsPercentHandler.StopCalculatePercent();
    }

    public async UniTaskVoid LoadNextSeriesContent()
    {
        if (_currentSeriaLoadedNumber < _seriesCount)
        {
            _onContentIsLoadProperty.SetValue(true);
            _loadAssetsPercentHandler.SetDefault();
            int nextSeriaNumber = _currentSeriaLoadedNumber;
            int nextSeriaIndex = _currentSeriaLoadedNumber;
            nextSeriaNumber++;
            await UniTask.Yield(PlayerLoopTiming.Initialization);
            CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaIndex);
            
            await LoadCurrentLocalization(nextSeriaNumber);
            await UniTask.NextFrame();
            Debug.Log($"1");
            await WardrobeSeriaDataProviderBuildMode.TryLoadData(nextSeriaIndex);
            await UniTask.NextFrame();
            Debug.Log($"2");
            await CharacterProviderBuildMode.TryLoadDatas(nextSeriaIndex);
            await UniTask.NextFrame();
            Debug.Log($"3");
            await GameSeriesProvider.TryLoadData(nextSeriaIndex);
            await UniTask.NextFrame();
            Debug.Log($"4");

            await AudioClipProvider.TryLoadDatas(nextSeriaIndex);
            await UniTask.NextFrame();
            Debug.Log($"5");
            await BackgroundDataProvider.TryLoadDatas(nextSeriaIndex);
            await UniTask.NextFrame();
            Debug.Log($"Delay start");
            await UniTask.Delay(TimeSpan.FromSeconds(20f));
            Debug.Log($"Delay end");
            Debug.Log($"6");
            await SeriaGameStatsProviderBuild.TryLoadDataAndGet(nextSeriaIndex);
            await UniTask.NextFrame();
            await UniTask.Delay(TimeSpan.FromSeconds(1f));

            Debug.Log($"_backgroundContentCreator 1  {_backgroundContentCreator.PercentComplete}  ");

            // await TryLoadDatas1(nextSeriaNumber, nextSeriaIndex);
            await _backgroundContentCreator.TryCreateBackgroundContent();
            
            Debug.Log($"_backgroundContentCreator 2  {_backgroundContentCreator.PercentComplete}  ");

            
            Debug.Log($"TryLoadDatas  {_loadAssetsPercentHandler.CurrentLoadPercentReactiveProperty.Value}  ");
            Debug.Log($"7");
            _currentSeriaLoadedNumberProperty.SetValue(nextSeriaNumber);
            _onContentIsLoadProperty.SetValue(false);
        }
    }

    private async UniTask InitLoaders()
    {
        _seriesCount = await GameSeriesProvider.Init();
        await UniTask.WhenAll(
            WardrobeSeriaDataProviderBuildMode.Init(), 
            CharacterProviderBuildMode.Construct(),
            AudioClipProvider.Init(), 
            BackgroundDataProvider.Init(),
            SeriaGameStatsProviderBuild.Init());
    }

    private void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        WardrobeSeriaDataProviderBuildMode.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        CharacterProviderBuildMode.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
        GameSeriesProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        AudioClipProvider.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
        BackgroundDataProvider.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
        SeriaGameStatsProviderBuild.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }

    private async UniTask TryLoadDatas(int nextSeriaNumber, int nextSeriaIndex)
    {
        await LoadCurrentLocalization(nextSeriaNumber);
        // Debug.Log($"1");
        await WardrobeSeriaDataProviderBuildMode.TryLoadData(nextSeriaIndex);
        // Debug.Log($"2");
        await CharacterProviderBuildMode.TryLoadDatas(nextSeriaIndex);
        // Debug.Log($"3");
        await GameSeriesProvider.TryLoadData(nextSeriaIndex);
        await UniTask.Delay(TimeSpan.FromSeconds(20f));

        // Debug.Log($"4");
        await AudioClipProvider.TryLoadDatas(nextSeriaIndex);
        // Debug.Log($"5");
        await BackgroundDataProvider.TryLoadDatas(nextSeriaIndex);
        // Debug.Log($"6");
        await SeriaGameStatsProviderBuild.TryLoadDataAndGet(nextSeriaIndex);
        // Debug.Log($"7");
    }
    private void OnSwitchToNextSeria(bool key)
    {
        if (_onContentIsLoadProperty.GetValue == false)
        {
            LoadNextSeriesContent().Forget();
        }
    }

    private async UniTask LoadCurrentLocalization(int seriaLoadedNumber)
    {
        await _levelLocalizationProvider.TryLoadLocalization(seriaLoadedNumber, _mainMenuLocalizationHandler.CurrentLanguageName.Key);
    }
}