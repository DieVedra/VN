using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;
using UnityEngine.Rendering;

public class LevelLocalizationProvider : IParticipiteInLoad
{
    private const int _deleteValue = 2;
    private const string _nameLevelLocalizationAsset = "FileLocalizationSeria";
    private readonly ILocalizationChanger _localizationChanger;

    private readonly ReactiveProperty<int> _currentSeriaIndexReactiveProperty;

    private readonly AssetExistsHandler _assetExistsHandler;

    private readonly LocalizationFileProvider _localizationFileProvider;
    private string _currentLanguageKey;

    private Dictionary<int, Dictionary<string, string>> _seriaLocalizations;
    public bool ParticipiteInLoad { get; private set; }
    public bool AssetInLoading => _localizationFileProvider.IsLoading;
    public int PercentComplete => _localizationFileProvider.GetPercentComplete();

    public LevelLocalizationProvider(ILocalizationChanger localizationChanger, ReactiveProperty<int> currentSeriaIndexReactiveProperty)
    {
        _localizationChanger = localizationChanger;
        _currentSeriaIndexReactiveProperty = currentSeriaIndexReactiveProperty;
        _localizationFileProvider = new LocalizationFileProvider();
        _assetExistsHandler = new AssetExistsHandler();
        _seriaLocalizations = new SerializedDictionary<int, Dictionary<string, string>>();
    }
    public async UniTask<bool> TryLoadLocalization(int seriaNumber, string languageKey)
    {
        if (AssetInLoading == true)
        {
            _localizationFileProvider.AbortLoad();
        }
        if (_currentLanguageKey == languageKey)
        {
            if (_seriaLocalizations.ContainsKey(seriaNumber))
            {
                ParticipiteInLoad = false;
                return ParticipiteInLoad;
            }
        }
        _currentLanguageKey = languageKey;
        var name = CreateName(seriaNumber, languageKey);
        if (await _assetExistsHandler.CheckAssetExists(name))
        {
            var currentLocalization = await _localizationFileProvider.LoadLocalizationFile(name);
            _seriaLocalizations.Add(seriaNumber, currentLocalization);
            TryDeleteUncessaryLocalization(seriaNumber);
            ParticipiteInLoad = true;
        }
        else
        {
            ParticipiteInLoad = false;
        }
        return ParticipiteInLoad;
    }

    public IReadOnlyDictionary<string, string> GetCurrentLocalization()
    {
        Debug.Log($"GetCurrentLocalization {_currentLanguageKey} {_currentSeriaIndexReactiveProperty.Value}");

        if (_seriaLocalizations.TryGetValue(CurrentSeriaNumberProvider.GetCurrentSeriaNumber(_currentSeriaIndexReactiveProperty.Value), out var value))
        {
            return value;
        }
        else
        {
            return null;
        }
    }
    public void SetDefault()
    {
        _localizationFileProvider.SetDefault();
    }
    private string CreateName(int seriaNumber, string languageKey)
    {
        return $"{languageKey}{_nameLevelLocalizationAsset}{seriaNumber}";
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
        _seriaLocalizations = new SerializedDictionary<int, Dictionary<string, string>>(); 
        await TryLoadLocalization(CurrentSeriaNumberProvider.GetCurrentSeriaNumber(_currentSeriaIndexReactiveProperty.Value),
            _localizationChanger.GetKey);
        // await TryLoadLocalization(CurrentSeriaNumberProvider.GetSecondSeriaNumber(_currentSeriaIndexReactiveProperty.Value),
        //     _localizationChanger.GetKey)/*.Forget()*/;
        TryLoadLocalization(CurrentSeriaNumberProvider.GetSecondSeriaNumber(_currentSeriaIndexReactiveProperty.Value),
            _localizationChanger.GetKey).Forget();

        foreach (var vv in _seriaLocalizations)
        {
            Debug.Log($"vv {vv.Key} {_currentLanguageKey}");
        }
    }

    private void TryDeleteUncessaryLocalization(int seriaNumber)
    {
        var seriaNumberForRemove = seriaNumber - _deleteValue;
        if (_seriaLocalizations.ContainsKey(seriaNumberForRemove))
        {
            _seriaLocalizations.Remove(seriaNumberForRemove);
        }
    }
}