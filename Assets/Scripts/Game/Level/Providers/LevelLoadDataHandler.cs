
using Cysharp.Threading.Tasks;
using UniRx;

public class LevelLoadDataHandler
{
    public const int NumberFirstSeria = 1;
    public const int IndexFirstSeriaData = 0;
    private const int _indexFirstName = 0;
    private readonly BackgroundContentCreator _backgroundContentCreator;
    private readonly int _seriesCount;
    public readonly CharacterProviderBuildMode CharacterProviderBuildMode;
    public readonly WardrobeSeriaDataProviderBuildMode WardrobeSeriaDataProviderBuildMode;
    public readonly GameSeriesProvider GameSeriesProvider;
    public readonly AudioClipProvider AudioClipProvider;
    public readonly BackgroundDataProvider BackgroundDataProvider;
    private readonly LoadAssetsPercentHandler _loadAssetsPercentHandler;
    public int CurrentSeriaLoadedNumber { get; private set; }
    public int CurrentLoadPercent => _loadAssetsPercentHandler.CurrentLoadPercent;

    public LevelLoadDataHandler(BackgroundContentCreator backgroundContentCreator, int seriesCount)
    {
        _backgroundContentCreator = backgroundContentCreator;
        _seriesCount = seriesCount;
        CharacterProviderBuildMode = new CharacterProviderBuildMode();
        WardrobeSeriaDataProviderBuildMode = new WardrobeSeriaDataProviderBuildMode();
        GameSeriesProvider = new GameSeriesProvider();
        AudioClipProvider = new AudioClipProvider();
        BackgroundDataProvider = new BackgroundDataProvider();
        
        _loadAssetsPercentHandler = new LoadAssetsPercentHandler(
            CharacterProviderBuildMode.CharactersDataProviderParticipiteInLoad,
            CharacterProviderBuildMode.CustomizableCharacterDataProviderParticipiteInLoad,
            WardrobeSeriaDataProviderBuildMode,
            GameSeriesProvider,
            AudioClipProvider.AmbientAudioDataProviderParticipiteInLoad,
            AudioClipProvider.MusicAudioDataProviderParticipiteInLoad,
            BackgroundDataProvider.ArtsDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.LocationDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.AdditionalImagesDataLoadProviderParticipiteInLoad,
            BackgroundDataProvider.WardrobeBackgroundDataLoadProviderParticipiteInLoad,
            backgroundContentCreator);
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
        await TryLoadDatas(_indexFirstName);
        await _backgroundContentCreator.TryCreateBackgroundContent();
        _loadAssetsPercentHandler.StopCalculatePercent();
        CurrentSeriaLoadedNumber = NumberFirstSeria;
    }

    public async UniTaskVoid LoadNextSeriesContent()
    {
        int nextSeriaNumber = CurrentSeriaLoadedNumber;
        for (int i = 0; i < _seriesCount; ++i)
        {
            nextSeriaNumber++;
            _loadAssetsPercentHandler.StartCalculatePercent();
            CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, CurrentSeriaLoadedNumber);
            await TryLoadDatas(CurrentSeriaLoadedNumber);
            await _backgroundContentCreator.TryCreateBackgroundContent();
            _loadAssetsPercentHandler.StopCalculatePercent();
            CurrentSeriaLoadedNumber = nextSeriaNumber;
        }
    }

    private async UniTask InitLoaders()
    {
        await UniTask.WhenAll(
            WardrobeSeriaDataProviderBuildMode.Init(), 
            CharacterProviderBuildMode.Init(), 
            GameSeriesProvider.Init(), 
            AudioClipProvider.Init(), 
            BackgroundDataProvider.Init());
    }

    private void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        WardrobeSeriaDataProviderBuildMode.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        CharacterProviderBuildMode.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
        GameSeriesProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        AudioClipProvider.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
        BackgroundDataProvider.CheckMatchNumbersSeriaWithNumberAssets(nextSeriaNumber, nextSeriaNameAssetIndex);
    }

    private async UniTask TryLoadDatas(int indexName)
    {
        await WardrobeSeriaDataProviderBuildMode.TryLoadData(indexName);
        await CharacterProviderBuildMode.TryLoadDatas(indexName);
        await GameSeriesProvider.TryLoadData(indexName);
        await AudioClipProvider.TryLoadDatas(indexName);
        await BackgroundDataProvider.TryLoadDatas(indexName);
    }
}