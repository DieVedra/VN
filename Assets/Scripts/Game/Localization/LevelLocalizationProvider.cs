using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelLocalizationProvider : IParticipiteInLoad
{
    public const string NameLevelLocalizationAsset = "FileLocalizationSeria";
    private const int _deleteValue = 2;
    private readonly string _storyName;
    private readonly ILocalizationChanger _localizationChanger;
    private readonly ReactiveProperty<int> _currentSeriaIndexReactiveProperty;
    private readonly AssetExistsHandler _assetExistsHandler;
    private readonly LocalizationFileProvider _localizationFileProvider;
    private string _currentLanguageKey;

    private List<string> _toDelete;
    private Dictionary<string, string> _localization;
    public bool ParticipiteInLoad { get; private set; }
    
    public int PercentComplete => _localizationFileProvider.GetPercentComplete();
    public LocalizationFileProvider LocalizationFileProvider => _localizationFileProvider;
    public IReadOnlyDictionary<string, string> Localization => _localization;
    public LevelLocalizationProvider(string storyName, ILocalizationChanger localizationChanger, ReactiveProperty<int> currentSeriaIndexReactiveProperty)
    {
        _storyName = storyName;
        _localizationChanger = localizationChanger;
        _currentSeriaIndexReactiveProperty = currentSeriaIndexReactiveProperty;
        _localizationFileProvider = new LocalizationFileProvider();
        _assetExistsHandler = new AssetExistsHandler();
        _localization = new Dictionary<string, string>();
    }
    public async UniTask TryLoadLocalization(int seriaNumber, string languageKey)
    {
        if (_localizationFileProvider.IsLoading == true)
        {
            _localizationFileProvider.AbortLoad();
        }
        _currentLanguageKey = languageKey;
        var name = CreateName(seriaNumber, languageKey);
        if (await _assetExistsHandler.CheckAssetExists(name))
        {
            ParticipiteInLoad = true;
            var currentLocalization = await _localizationFileProvider.LoadLocalizationFile(name);
            foreach (var pair in currentLocalization)
            {
                if (_localization.ContainsKey(pair.Key) == false)
                {
                    _localization.Add(pair.Key, pair.Value);
                }
            }
        }
        else
        {
            Debug.LogWarning($"Asset not find: '{name}'");
            ParticipiteInLoad = false;
        }
    }
    public void SetDefault()
    {
        _localizationFileProvider.SetDefault();
    }
    private string CreateName(int seriaNumber, string languageKey)
    {
        return $"{_storyName}{languageKey}{NameLevelLocalizationAsset}{seriaNumber}";
    }

    public bool IsLocalizationHasBeenChanged()
    {
        if (_localizationChanger.GetKey != _currentLanguageKey)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public async UniTask TryLoadLocalizationOnSwitchLanguageFromSettings()
    {
        _localization.Clear();
        for (int i = 0; i <= _currentSeriaIndexReactiveProperty.Value; i++)
        {
            await TryLoadLocalization(CurrentSeriaNumberProvider.GetCurrentSeriaNumber(i),
                _localizationChanger.GetKey);
        }
        TryLoadLocalization(CurrentSeriaNumberProvider.GetSecondSeriaNumber(_currentSeriaIndexReactiveProperty.Value),
            _localizationChanger.GetKey).Forget();
    }
    public void AddToDelete(string key)
    {
        _toDelete ??= new List<string>();
        _toDelete.Add(key);
    }
    public void DeleteUncessaryStrings()
    {
        foreach (var str in _toDelete)
        {
            if (_localization.ContainsKey(str))
            {
                _localization.Remove(str);
            }
        }
        _toDelete?.Clear();
    }
}