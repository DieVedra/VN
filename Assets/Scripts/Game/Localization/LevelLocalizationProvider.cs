
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

public class LevelLocalizationProvider : IParticipiteInLoad
{
    private const string _nameLevelLocalizationAsset = "FileLocalizationSeria";

    private readonly AssetExistsHandler _assetExistsHandler;

    private readonly LocalizationFileProvider _localizationFileProvider;

    private Dictionary<string, string> _currentLocalization;
    public bool ParticipiteInLoad { get; private set; }
    public int PercentComplete => _localizationFileProvider.GetPercentComplete();
    public IReadOnlyDictionary<string, string> CurrentLocalization => _currentLocalization;

    public LevelLocalizationProvider(ILocalizationChanger localizationChanger)
    {
        _localizationFileProvider = new LocalizationFileProvider();
        _assetExistsHandler = new AssetExistsHandler();
        
        
        CompositeDisposable compositeDisposable = new CompositeDisposable();
        localizationChanger.CurrentLanguageKeyIndex.Subscribe(_ =>
        {
            
        }).AddTo(compositeDisposable);
    }
    public async UniTask<bool> TryLoadLocalization(int seriaNumber, string languageKey)
    {
        var name = CreateName(seriaNumber, languageKey);
        if (await _assetExistsHandler.CheckAssetExists(name))
        {
            _currentLocalization = await _localizationFileProvider.LoadLocalizationFile(name);
            return true;
        }
        else
        {
            return false;
        }
    }
    private string CreateName(int seriaNumber, string languageKey)
    {
        return $"{languageKey}{_nameLevelLocalizationAsset}{seriaNumber}";
    }
}