using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;

public class LevelLocalizationHandler : ILevelLocalizationHandler
{
    private readonly ICurrentSeriaNodeGraphsProvider _currentSeriaNodeGraphsProvider;
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    private readonly ILocalizable _characterProviderLocalizable;
    private readonly GameStatsHandler _gameStatsHandler;
    private readonly ILocalizable _phoneUIHandler;
    private readonly ILocalizable _phoneProviderInBuildMode;
    private readonly ILocalizable _phoneMessagesCustodian;
    private readonly SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private readonly ReactiveCommand _onEndSwitchLocalization;
    private CompositeDisposable _compositeDisposable;
    private IReadOnlyDictionary<string, string> _currentLocalization;
    public ReactiveCommand OnEndSwitchLocalization => _onEndSwitchLocalization;

    public LevelLocalizationHandler(ICurrentSeriaNodeGraphsProvider currentSeriaNodeGraphsProvider,
        LevelLocalizationProvider levelLocalizationProvider, ILocalizable characterProviderLocalizable,
        GameStatsHandler gameStatsHandler, ILocalizable phoneUIHandler, ILocalizable phoneProviderInBuildMode,
        ILocalizable phoneMessagesCustodian,
        SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _currentSeriaNodeGraphsProvider = currentSeriaNodeGraphsProvider;
        _levelLocalizationProvider = levelLocalizationProvider;
        _characterProviderLocalizable = characterProviderLocalizable;
        _gameStatsHandler = gameStatsHandler;
        _phoneUIHandler = phoneUIHandler;
        _phoneProviderInBuildMode = phoneProviderInBuildMode;
        _phoneMessagesCustodian = phoneMessagesCustodian;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _compositeDisposable = new CompositeDisposable();
        _onEndSwitchLocalization = new ReactiveCommand().AddTo(_compositeDisposable);
    }

    public void Shutdown()
    {
        _compositeDisposable?.Clear();
    }
    public void TrySetLocalizationToCurrentLevelContent(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        _currentLocalization = _levelLocalizationProvider.GetCurrentLocalization();
        if (_currentLocalization != null)
        {
            SetLocalizationToSeriaTexts(seriaNodeGraphsHandler);
            SetLocalizationToStats(_gameStatsHandler);
            SetLocalizationToCharacters();
            SetLocalizationToPhoneData();
            _setLocalizationChangeEvent.Execute();
            _currentLocalization = null;
        }
    }

    private void SetLocalizationToPhoneData()
    {
        foreach (var localizationString in _phoneUIHandler.GetLocalizableContent())
        {
            SetText(localizationString);

        }

        foreach (var localizationString in _phoneProviderInBuildMode.GetLocalizableContent())
        {
            SetText(localizationString);
        }

        foreach (var localizationString in _phoneMessagesCustodian.GetLocalizableContent())
        {
            foreach (var localization in _levelLocalizationProvider.Localizations)
            {
                if (localization.Value.TryGetValue(localizationString.Key, out string value))
                {
                    localizationString.SetText(value);
                }
            }
        }
    }
    private void SetLocalizationToSeriaTexts(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        foreach (var seriaPartNodeGraph in seriaNodeGraphsHandler.SeriaPartNodeGraphs)
        {
            foreach (var node in seriaPartNodeGraph.nodes)
            {
                if (node is ILocalizable localizable)
                {
                    foreach (var localizationString in localizable.GetLocalizableContent())
                    {
                        SetText(localizationString);
                    }
                }
            }
        }
    }

    private void SetLocalizationToStats(GameStatsHandler gameStatsHandler)
    {
        foreach (var stat in gameStatsHandler.Stats)
        {
            SetText(stat.LocalizationName);
        }
    }

    private void SetLocalizationToCharacters()
    {
        foreach (var name in _characterProviderLocalizable.GetLocalizableContent())
        {
            SetText(name);
        }
    }

    private void SetText(LocalizationString localizationString)
    {
        if (localizationString.Key != null)
        {
            if (_currentLocalization.TryGetValue(localizationString.Key, out string text))
            {
                localizationString.SetText(text);
            }
        }
    }

    public bool IsLocalizationHasBeenChanged()
    {
        return _levelLocalizationProvider.IsLocalizationHasBeenChanged();
    }

    public async UniTaskVoid TrySwitchLanguageFromSettingsChange()
    {
        await _levelLocalizationProvider.TryLoadLocalizationOnSwitchLanguageFromSettings();
        TrySetLocalizationToCurrentLevelContent(_currentSeriaNodeGraphsProvider.GetCurrentSeriaNodeGraphsHandler());
        _onEndSwitchLocalization.Execute();
    }
}