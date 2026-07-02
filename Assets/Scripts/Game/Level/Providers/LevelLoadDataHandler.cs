using System;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class LevelLoadDataHandler
{
    public const int IndexFirstSeriaData = 0;
    private const int _indexFirstName = 0;
    private const int _numberFirstSeria = 1;
    private readonly string _storyName;

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

    public LevelLoadDataHandler(string storyName, PanelsLocalizationHandler panelsLocalizationHandler, PhoneMessagesCustodian phoneMessagesCustodian,
        LevelLocalizationProvider levelLocalizationProvider, PhoneSaveHandler phoneSaveHandler, CharacterProviderBuildMode characterProviderBuildMode,
        Func<UniTask> createPhoneView, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,  
        CurrentSeriaLoadedNumberProperty<int> currentSeriaLoadedNumberProperty,
        OnContentIsLoadProperty<bool> onContentIsLoadProperty)
    {
        _panelsLocalizationHandler = panelsLocalizationHandler;
        _levelLocalizationProvider = levelLocalizationProvider;
        _currentSeriaLoadedNumberProperty = currentSeriaLoadedNumberProperty;
        _onContentIsLoadProperty = onContentIsLoadProperty;
        SeriaGameStatsProviderBuild = new SeriaGameStatsProviderBuild(storyName);
        CharacterProviderBuildMode = characterProviderBuildMode;
        GameSeriesProvider = new GameSeriesProvider(storyName);
        AudioClipProvider = new AudioClipProvider(storyName);
        BackgroundDataProvider = new BackgroundDataProvider(storyName);
        PhoneProviderInBuildMode = new PhoneProviderInBuildMode(storyName, phoneMessagesCustodian, phoneSaveHandler, createPhoneView);
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
        Debug.Log($"++++++++++++++++++++++  LoadSTARTSeriaContent");
        await InitLoaders();
        CheckMatchNumbersSeriaWithNumberAssets(_numberFirstSeria);
        _loadAssetsPercentHandler.StartCalculatePercent();
        await LoadCurrentLocalization(_numberFirstSeria);
        await GameSeriesProvider.TryLoadData();
        await SeriaGameStatsProviderBuild.TryLoadData();
        await BackgroundDataProvider.TryLoadDatas();
        await CharacterProviderBuildMode.TryLoadDatas();
        await AudioClipProvider.TryLoadDatas();
        await PhoneProviderInBuildMode.TryLoadDatas();
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
        Debug.Log($"-------------------  LoadSTARTSeriaContent");
        Debug.Log($"                              ");
        Debug.Log($"                              ");
        Debug.Log($"                              ");
        Debug.Log($"                              ");
        Debug.Log($"                              ");

    }

    public async UniTask LoadNextSeriesContent()
    {
        Debug.Log($"++++++++++++++++++++++  LoadNEXTSeriaContent");

        if (_currentSeriaLoadedNumberProperty.GetValue < _seriesCount)
        {
            _onContentIsLoadProperty.SetValue(true);
            _loadAssetsPercentHandler.SetDefault();
            int nextSeriaNumber = _currentSeriaLoadedNumberProperty.GetValue;
            int nextSeriaIndex = _currentSeriaLoadedNumberProperty.GetValue;
            nextSeriaNumber++;
            await UniTask.Yield(PlayerLoopTiming.Initialization);
            CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber);
            await LoadCurrentLocalization(nextSeriaNumber);
            await CharacterProviderBuildMode.TryLoadDatas();
            await GameSeriesProvider.TryLoadData();
            await AudioClipProvider.TryLoadDatas();
            await BackgroundDataProvider.TryLoadDatas();
            await SeriaGameStatsProviderBuild.TryLoadData();
            await PhoneProviderInBuildMode.TryLoadDatas();
            
            _currentSeriaLoadedNumberProperty.SetValue(nextSeriaNumber); //!!!!
            _onContentIsLoadProperty.SetValue(false);
        }
        Debug.Log($"----------------  LoadNEXTSeriaContent");
        Debug.Log($"                              ");
        Debug.Log($"                              ");
        Debug.Log($"                              ");
        Debug.Log($"                              ");
        Debug.Log($"                              ");
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

    private void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber)
    {
        CharacterProviderBuildMode.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber);
        GameSeriesProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber);
        AudioClipProvider.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber);
        BackgroundDataProvider.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber);
        SeriaGameStatsProviderBuild.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber);
        PhoneProviderInBuildMode.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber);
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