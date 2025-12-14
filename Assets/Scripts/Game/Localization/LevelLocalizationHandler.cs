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
            
        }
    }
    private void SetLocalizationToSeriaTexts(SeriaNodeGraphsHandler seriaNodeGraphsHandler)
    {
        for (int i = 0; i < seriaNodeGraphsHandler.SeriaPartNodeGraphs.Count; i++)
        {
            for (int j = 0; j < seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes.Count; j++)
            {
                if (seriaNodeGraphsHandler.SeriaPartNodeGraphs[i].nodes[j] is ILocalizable localizable)
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
        for (int i = 0; i < gameStatsHandler.Stats.Count; i++)
        {
            SetText(gameStatsHandler.Stats[i].LocalizationName);
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