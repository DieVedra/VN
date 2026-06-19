using Cysharp.Threading.Tasks;
using UniRx;

public class AudioClipProvider
{
    public const string NameMusicAsset = "MusicAudioDataSeria";
    public const string NameAmbientAsset = "AmbientAudioDataSeria";
    private readonly string _fullNameMusicAsset;
    private readonly string _fullNameAmbientAsset;
    private readonly string _storyName;

    private readonly DataProvider<AudioData> _musicAudioDataProvider;
    private readonly DataProvider<AudioData> _ambientAudioDataProvider;
    public IParticipiteInLoad MusicAudioDataProviderParticipiteInLoad => _musicAudioDataProvider;
    public IParticipiteInLoad AmbientAudioDataProviderParticipiteInLoad => _ambientAudioDataProvider;
    public IReactiveCommand<AudioData> OnLoadMusicAudioData => _musicAudioDataProvider.OnLoad;
    public IReactiveCommand<AudioData> OnLoadAmbientAudioData => _ambientAudioDataProvider.OnLoad;

    public AudioClipProvider(string storyName)
    {
        _storyName = storyName;
        _musicAudioDataProvider = new DataProvider<AudioData>();
        _ambientAudioDataProvider = new DataProvider<AudioData>();
    }

    public async UniTask Init()
    {
        await UniTask.WhenAll(_musicAudioDataProvider.CreateNames($"{_storyName}{NameMusicAsset}"),
            _ambientAudioDataProvider.CreateNames($"{_storyName}{NameAmbientAsset}"));
    }
    public void Shutdown()
    {
        _musicAudioDataProvider.Shutdown();
        _ambientAudioDataProvider.Shutdown();
    }
    public void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber, int nextSeriaNameAssetIndex)
    {
        _musicAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
        _ambientAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber, nextSeriaNameAssetIndex);
    }
    public async UniTask TryLoadDatas(int nextSeriaNameAssetIndex)
    {
        await _musicAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex);
        await _ambientAudioDataProvider.TryLoadData(nextSeriaNameAssetIndex);
    }
}