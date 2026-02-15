using System;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class LevelLoadDataHandler
{
    public const int IndexFirstSeriaData = 0;
    private const int _indexFirstName = 0;
    private const int _numberFirstSeria = 1;
    private readonly PanelsLocalizationHandler _panelsLocalizationHandler;
    public readonly CharacterProviderBuildMode CharacterProviderBuildMode;
    public readonly SeriaGameStatsProviderBuild SeriaGameStatsProviderBuild;
    public readonly GameSeriesProvider GameSeriesProvider;
    public readonly AudioClipProvider AudioClipProvider;
    public readonly BackgroundDataProvider BackgroundDataProvider;
    public readonly PhoneProviderInBuildMode PhoneProviderInBuildMode;
    
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    private readonly CurrentSeriaLoadedNumberProperty<int> _currentSeriaLoadedNumberProperty;
    private readonly OnContentIsLoadProperty<bool> _onContentIsLoadProperty;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;
    private int _seriesCount;
    public int CurrentLoadPercent => _loadAssetsPercentHandler.CurrentLoadPercentReactiveProperty.Value;
    public LoadAssetsPercentHandler LoadAssetsPercentHandler => _loadAssetsPercentHandler;

    public LevelLoadDataHandler(PanelsLocalizationHandler panelsLocalizationHandler, PhoneMessagesCustodian phoneMessagesCustodian,
        LevelLocalizationProvider levelLocalizationProvider, PhoneSaveHandler phoneSaveHandler, CharacterProviderBuildMode characterProviderBuildMode,
        Func<UniTask> createPhoneView, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,  
        CurrentSeriaLoadedNumberProperty<int> currentSeriaLoadedNumberProperty,
        OnContentIsLoadProperty<bool> onContentIsLoadProperty)
    {
        _panelsLocalizationHandler = panelsLocalizationHandler;
        _levelLocalizationProvider = levelLocalizationProvider;
        _currentSeriaLoadedNumberProperty = currentSeriaLoadedNumberProperty;
        _onContentIsLoadProperty = onContentIsLoadProperty;
        SeriaGameStatsProviderBuild = new SeriaGameStatsProviderBuild();
        CharacterProviderBuildMode = characterProviderBuildMode;
        GameSeriesProvider = new GameSeriesProvider();
        AudioClipProvider = new AudioClipProvider();
        BackgroundDataProvider = new BackgroundDataProvider();
        PhoneProviderInBuildMode = new PhoneProviderInBuildMode(phoneMessagesCustodian, phoneSaveHandler, createPhoneView);
        _loadAssetsPercentHandler = new LoadAssetsPercentHandler(
            GameSeriesProvider,
            SeriaGameStatsProviderBuild,
            BackgroundDataProvider.ArtsDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.LocationDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.AdditionalImagesDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.WardrobeBackgroundDataLoadProviderParticipiteInLoad,
            CharacterProviderBuildMode.CharactersDataProviderParticipiteInLoad,
            CharacterProviderBuildMode.CharactersProviderParticipiteInLoad,
            AudioClipProvider.AmbientAudioDataProviderParticipiteInLoad,
            AudioClipProvider.MusicAudioDataProviderParticipiteInLoad,
            PhoneProviderInBuildMode.PhoneDataProviderParticipiteInLoad,
            PhoneProviderInBuildMode.PhoneContactsProviderParticipiteInLoad,
            _levelLocalizationProvider);
        switchToNextSeriaEvent.Subscribe(OnSwitchToNextSeria);
    }

    public void Shutdown()
    {
        _loadAssetsPercentHandler.StopCalculatePercent();
        _levelLocalizationProvider.LocalizationFileProvider.AbortLoad();
        CharacterProviderBuildMode.Shutdown();
        GameSeriesProvider.Shutdown();
        AudioClipProvider.Shutdown();
        BackgroundDataProvider.Shutdown();
        SeriaGameStatsProviderBuild.Shutdown();
        PhoneProviderInBuildMode.Shutdown();
    }
    public async UniTask LoadStartSeriaContent(StoryData storyData = null)
    {
        await InitLoaders();
        CheckMatchNumbersSeriaWithNumberAssets(_numberFirstSeria, _indexFirstName);
        _loadAssetsPercentHandler.StartCalculatePercent();
        await LoadCurrentLocalization(_numberFirstSeria);
        await GameSeriesProvider.TryLoadData(_indexFirstName);
        await SeriaGameStatsProviderBuild.TryLoadData(_indexFirstName);
        await BackgroundDataProvider.TryLoadDatas(_indexFirstName);
        await CharacterProviderBuildMode.TryLoadDatas(_indexFirstName);
        await AudioClipProvider.TryLoadDatas(_indexFirstName);
        await PhoneProviderInBuildMode.TryLoadDatas(_indexFirstName);
        if (storyData != null)
        {
            PhoneProviderInBuildMode.PhoneSaveHandler.SetPhoneInfoFromSaveData(storyData);
            for (int i = 0; i < storyData.CurrentSeriaIndex; i++)
            {
                await LoadNextSeriesContent();
            }
        }
        _currentSeriaLoadedNumberProperty.SetValue(_numberFirstSeria);
        _loadAssetsPercentHandler.StopCalculatePercent();
    }

    public async UniTask LoadNextSeriesContent()
    {
        if (_currentSeriaLoadedNumberProperty.GetValue < _seriesCount)
        {
            _onContentIsLoadProperty.SetValue(true);
            _loadAssetsPercentHandler.SetDefault();
            int nextSeriaNumber = _currentSeriaLoadedNumberProperty.GetValue;
            int nextSeriaIndex = _currentSeriaLoadedNumberProperty.GetValue;
            nextSeriaNumber++;
            await UniTask.Yield(PlayerLoopTiming.Initialization);
            CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaIndex);
            await LoadCurrentLocalization(nextSeriaNumber);
            await CharacterProviderBuildMode.TryLoadDatas(nextSeriaIndex);
            await GameSeriesProvider.TryLoadData(nextSeriaIndex);
            await AudioClipProvider.TryLoadDatas(nextSeriaIndex);
            await BackgroundDataProvider.TryLoadDatas(nextSeriaIndex);
            await SeriaGameStatsProviderBuild.TryLoadData(nextSeriaIndex);
            await PhoneProviderInBuildMode.TryLoadDatas(nextSeriaIndex);
            
            _currentSeriaLoadedNumberProperty.SetValue(nextSeriaNumber); //!!!!
            _onContentIsLoadProperty.SetValue(false);
        }
    }

    private async UniTask InitLoaders()
    {
        _seriesCount = await GameSeriesProvider.Init();
        await UniTask.WhenAll(
            CharacterProviderBuildMode.Construct(),
            AudioClipProvider.Init(), 
            BackgroundDataProvider.Init(),
            SeriaGameStatsProviderBuild.Init(),
            PhoneProviderInBuildMode.Init());
    }

    private void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        CharacterProviderBuildMode.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
        GameSeriesProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        AudioClipProvider.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);

        BackgroundDataProvider.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
        SeriaGameStatsProviderBuild.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        PhoneProviderInBuildMode.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
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
        await _levelLocalizationProvider.TryLoadLocalization(seriaLoadedNumber, _panelsLocalizationHandler.CurrentLanguageName.Key);
    }
}