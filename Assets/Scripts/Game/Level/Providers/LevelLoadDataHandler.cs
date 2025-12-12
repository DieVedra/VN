using System;
using Cysharp.Threading.Tasks;
using UniRx;

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
    private readonly BackgroundContentCreator _backgroundContentCreator;
    
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    private readonly CurrentSeriaLoadedNumberProperty<int> _currentSeriaLoadedNumberProperty;
    private readonly OnContentIsLoadProperty<bool> _onContentIsLoadProperty;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;
    private int _seriesCount;
    private int _currentSeriaLoadedNumber => _currentSeriaLoadedNumberProperty.GetValue;
    public int CurrentLoadPercent => _loadAssetsPercentHandler.CurrentLoadPercentReactiveProperty.Value;
    public LoadAssetsPercentHandler LoadAssetsPercentHandler => _loadAssetsPercentHandler;

    public LevelLoadDataHandler(PanelsLocalizationHandler panelsLocalizationHandler, PhoneMessagesCustodian phoneMessagesCustodian,  BackgroundContentCreator backgroundContentCreator,
        LevelLocalizationProvider levelLocalizationProvider, Func<UniTask> createPhoneView,
        SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent,  
        CurrentSeriaLoadedNumberProperty<int> currentSeriaLoadedNumberProperty,
        OnContentIsLoadProperty<bool> onContentIsLoadProperty)
    {
        _panelsLocalizationHandler = panelsLocalizationHandler;
        _backgroundContentCreator = backgroundContentCreator;
        _levelLocalizationProvider = levelLocalizationProvider;
        _currentSeriaLoadedNumberProperty = currentSeriaLoadedNumberProperty;
        _currentSeriaLoadedNumberProperty.SetValue(_numberFirstSeria);
        _onContentIsLoadProperty = onContentIsLoadProperty;
        SeriaGameStatsProviderBuild = new SeriaGameStatsProviderBuild();
        CharacterProviderBuildMode = new CharacterProviderBuildMode();
        GameSeriesProvider = new GameSeriesProvider();
        AudioClipProvider = new AudioClipProvider();
        BackgroundDataProvider = new BackgroundDataProvider();
        PhoneProviderInBuildMode = new PhoneProviderInBuildMode(phoneMessagesCustodian, createPhoneView);
        _loadAssetsPercentHandler = new LoadAssetsPercentHandler(
            GameSeriesProvider,
            SeriaGameStatsProviderBuild,
            BackgroundDataProvider.ArtsDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.LocationDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.AdditionalImagesDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.WardrobeBackgroundDataLoadProviderParticipiteInLoad,
            _backgroundContentCreator,
            CharacterProviderBuildMode.CharactersDataProviderParticipiteInLoad,
            CharacterProviderBuildMode.CharactersProviderParticipiteInLoad,
            AudioClipProvider.AmbientAudioDataProviderParticipiteInLoad,
            AudioClipProvider.MusicAudioDataProviderParticipiteInLoad,
            PhoneProviderInBuildMode.PhoneDataProviderParticipiteInLoad,
            PhoneProviderInBuildMode.PhoneContactsProviderParticipiteInLoad,
            _levelLocalizationProvider);
        switchToNextSeriaEvent.Subscribe(OnSwitchToNextSeria);
    }

    public void Dispose()
    {
        _loadAssetsPercentHandler.StopCalculatePercent();
        _levelLocalizationProvider.LocalizationFileProvider.AbortLoad();
        CharacterProviderBuildMode.Dispose();
        GameSeriesProvider.Dispose();
        AudioClipProvider.Dispose();
        BackgroundDataProvider.Dispose();
        SeriaGameStatsProviderBuild.Dispose();
        _backgroundContentCreator.Dispose();
        PhoneProviderInBuildMode.Dispose();
    }
    public async UniTask LoadStartSeriaContent(StoryData storyData = null)
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
        await CharacterProviderBuildMode.TryLoadDatas(_indexFirstName);
        await AudioClipProvider.TryLoadDatas(_indexFirstName);
        await PhoneProviderInBuildMode.TryLoadDatas(_indexFirstName);
        if (storyData != null)
        {
            PhoneProviderInBuildMode.TrySetSaveData(storyData.Contacts);

            for (int i = 0; i < storyData.CurrentSeriaIndex; i++)
            {
                await LoadNextSeriesContent();
            }
        }
        _loadAssetsPercentHandler.StopCalculatePercent();
    }

    public async UniTask LoadNextSeriesContent()
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
            await CharacterProviderBuildMode.TryLoadDatas(nextSeriaIndex);
            await GameSeriesProvider.TryLoadData(nextSeriaIndex);
            await AudioClipProvider.TryLoadDatas(nextSeriaIndex);
            await BackgroundDataProvider.TryLoadDatas(nextSeriaIndex);
            await SeriaGameStatsProviderBuild.TryLoadData(nextSeriaIndex);
            await _backgroundContentCreator.TryCreateBackgroundContent();

            await PhoneProviderInBuildMode.TryLoadDatas(nextSeriaIndex);
            
            _currentSeriaLoadedNumberProperty.SetValue(nextSeriaNumber);
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
            PhoneProviderInBuildMode.Init(CharacterProviderBuildMode.CustomizableCharacterIndexesCustodians));
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