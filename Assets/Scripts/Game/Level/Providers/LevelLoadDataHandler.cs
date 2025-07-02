using Cysharp.Threading.Tasks;
using UniRx;

public class LevelLoadDataHandler
{
    public const int NumberFirstSeria = 1;
    public const int IndexFirstSeriaData = 0;
    private const int _indexFirstName = 0;
    public readonly CharacterProviderBuildMode CharacterProviderBuildMode;
    public readonly SeriaGameStatsProviderBuild SeriaGameStatsProviderBuild;
    public readonly WardrobeSeriaDataProviderBuildMode WardrobeSeriaDataProviderBuildMode;
    public readonly GameSeriesProvider GameSeriesProvider;
    public readonly AudioClipProvider AudioClipProvider;
    public readonly BackgroundDataProvider BackgroundDataProvider;
    private readonly BackgroundContentCreator _backgroundContentCreator;
    private readonly SwitchToNextSeriaEvent<bool> _switchToNextSeriaEvent;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;
    private int _seriesCount;
    public int CurrentSeriaLoadedNumber { get; private set; }
    public int CurrentLoadPercent => _loadAssetsPercentHandler.CurrentLoadPercent;

    public LevelLoadDataHandler(BackgroundContentCreator backgroundContentCreator, SwitchToNextSeriaEvent<bool> switchToNextSeriaEvent)
    {
        _backgroundContentCreator = backgroundContentCreator;
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
            _backgroundContentCreator);
        switchToNextSeriaEvent.Subscribe(OnSwitchToNextSeria);
    }

    public async UniTask LoadFirstSeriaContent()
    {
        BackgroundDataProvider.OnLoadLocationData.Subscribe(_ =>
        {
            _backgroundContentCreator.SetCurrentBackgroundData(_);
        });
        await InitLoaders();
        CheckMatchNumbersSeriaWithNumberAssets(NumberFirstSeria, _indexFirstName);
        _loadAssetsPercentHandler.StartCalculatePercent();

        await GameSeriesProvider.TryLoadData(_indexFirstName);
        await SeriaGameStatsProviderBuild.TryLoadData(_indexFirstName);
        await BackgroundDataProvider.TryLoadDatas(_indexFirstName);
        await _backgroundContentCreator.TryCreateBackgroundContent();
        await WardrobeSeriaDataProviderBuildMode.TryLoadData(_indexFirstName);
        await CharacterProviderBuildMode.TryLoadDatas(_indexFirstName);
        await AudioClipProvider.TryLoadDatas(_indexFirstName);
        
        _loadAssetsPercentHandler.StopCalculatePercent();
        CurrentSeriaLoadedNumber = NumberFirstSeria;
        
        LoadNextSeriesContent().Forget();
    }

    private async UniTaskVoid LoadNextSeriesContent()
    {
        if (CurrentSeriaLoadedNumber <= _seriesCount)
        {
            int nextSeriaNumber = CurrentSeriaLoadedNumber;
            nextSeriaNumber++;
            // _loadAssetsPercentHandler.StartCalculatePercent();
            CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, CurrentSeriaLoadedNumber);
            await TryLoadDatas(CurrentSeriaLoadedNumber);
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

    private async UniTask TryLoadDatas(int indexName)
    {
        await WardrobeSeriaDataProviderBuildMode.TryLoadData(indexName);
        await CharacterProviderBuildMode.TryLoadDatas(indexName);
        await GameSeriesProvider.TryLoadData(indexName);
        await AudioClipProvider.TryLoadDatas(indexName);
        await BackgroundDataProvider.TryLoadDatas(indexName);
        await SeriaGameStatsProviderBuild.TryLoadData(indexName);
    }

    private void OnSwitchToNextSeria(bool key)
    {
        LoadNextSeriesContent().Forget();
    }
}