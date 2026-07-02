using Cysharp.Threading.Tasks;
using UniRx;

public class AudioClipProvider
{
    private const string _nameMusicAsset = "MusicAudioDataSeria";
    private const string _nameAmbientAsset = "AmbientAudioDataSeria";
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
        await UniTask.WhenAll(_musicAudioDataProvider.CreateNames($"{_storyName}{_nameMusicAsset}"),
            _ambientAudioDataProvider.CreateNames($"{_storyName}{_nameAmbientAsset}"));
    }
    public void Shutdown()
    {
        _musicAudioDataProvider.Shutdown();
        _ambientAudioDataProvider.Shutdown();
    }
    public void CheckMatchNumbersSeriaWithNumberAssets(int nextSeriaNumber)
    {
        _musicAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber);
        _ambientAudioDataProvider.CheckMatchNumbersSeriaWithNumberAsset(nextSeriaNumber);
    }
    public async UniTask TryLoadDatas()
    {
        await _musicAudioDataProvider.TryLoadData();
        await _ambientAudioDataProvider.TryLoadData();
    }
}