using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;

public class LevelLoadDataHandler
{
    public const int IndexFirstSeriaData = 0;
    private const int _indexFirstName = 0;
    private const int _delay = 1000;
    private readonly MainMenuLocalizationHandler _mainMenuLocalizationHandler;
    public readonly CharacterProviderBuildMode CharacterProviderBuildMode;
    public readonly SeriaGameStatsProviderBuild SeriaGameStatsProviderBuild;
    public readonly WardrobeSeriaDataProviderBuildMode WardrobeSeriaDataProviderBuildMode;
    public readonly GameSeriesProvider GameSeriesProvider;
    public readonly AudioClipProvider AudioClipProvider;
    public readonly BackgroundDataProvider BackgroundDataProvider;
    private readonly BackgroundContentCreator _backgroundContentCreator;
    
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    
    private readonly SwitchToNextSeriaEvent<bool> _switchToNextSeriaEvent;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;
    private int _seriesCount;
    public ReactiveCommand<bool> ContentIsLoading { get; private set; }
    public int CurrentSeriaLoadedNumber { get; private set; }
    public int CurrentLoadPercent => _loadAssetsPercentHandler.CurrentLoadPercent;

    public LevelLoadDataHandler(MainMenuLocalizationHandler mainMenuLocalizationHandler, BackgroundContentCreator backgroundContentCreator, LevelLocalizationProvider levelLocalizationProvider,
        SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent, int numberFirstSeria = 1)
    {
        _mainMenuLocalizationHandler = mainMenuLocalizationHandler;
        CurrentSeriaLoadedNumber = numberFirstSeria;
        _backgroundContentCreator = backgroundContentCreator;
        _levelLocalizationProvider = levelLocalizationProvider;
        _switchToNextSeriaEvent = switchToNextSeriaEvent;
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
        WardrobeSeriaDataProviderBuildMode.Dispose();
        CharacterProviderBuildMode.Dispose();
        GameSeriesProvider.Dispose();
        AudioClipProvider.Dispose();
        BackgroundDataProvider.Dispose();
    }
    public async UniTask LoadFirstSeriaContent()
    {
        BackgroundDataProvider.OnLoadLocationData.Subscribe(_ =>
        {
            _backgroundContentCreator.SetCurrentBackgroundData(_);
        });
        await InitLoaders();
        CheckMatchNumbersSeriaWithNumberAssets(CurrentSeriaLoadedNumber, _indexFirstName);
        _loadAssetsPercentHandler.StartCalculatePercent();

        await LoadCurrentLocalization(CurrentSeriaLoadedNumber);
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
        if (CurrentSeriaLoadedNumber <= _seriesCount)
        {
            int nextSeriaNumber = CurrentSeriaLoadedNumber;
            nextSeriaNumber++;
            // _loadAssetsPercentHandler.StartCalculatePercent();
            CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, CurrentSeriaLoadedNumber);
            await TryLoadDatas(nextSeriaNumber, CurrentSeriaLoadedNumber);
            await _backgroundContentCreator.TryCreateBackgroundContent();
            // _loadAssetsPercentHandler.StopCalculatePercent();
            CurrentSeriaLoadedNumber = nextSeriaNumber;
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
        await WardrobeSeriaDataProviderBuildMode.TryLoadData(nextSeriaIndex);
        await CharacterProviderBuildMode.TryLoadDatas(nextSeriaIndex);
        await GameSeriesProvider.TryLoadData(nextSeriaIndex);
        await AudioClipProvider.TryLoadDatas(nextSeriaIndex);
        await BackgroundDataProvider.TryLoadDatas(nextSeriaIndex);
        await SeriaGameStatsProviderBuild.TryLoadDataAndGet(nextSeriaIndex);
    }

    private void OnSwitchToNextSeria(bool key)
    {
        LoadNextSeriesContent().Forget();
    }

    private async UniTask LoadCurrentLocalization(int seriaLoadedNumber)
    {
        await _levelLocalizationProvider.TryLoadLocalization(seriaLoadedNumber, _mainMenuLocalizationHandler.CurrentLanguageName.Key);
    }
}