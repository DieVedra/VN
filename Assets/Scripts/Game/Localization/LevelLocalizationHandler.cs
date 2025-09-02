using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

public class LevelLocalizationHandler : ILevelLocalizationHandler
{
    private readonly LevelLocalizationProvider _levelLocalizationProvider;
    private readonly CharacterProviderBuildMode _characterProviderBuildMode;
    private readonly SetLocalizationChangeEvent _setLocalizationChangeEvent;
    private ReactiveCommand _onTrySwitchLocalization;
    private ReactiveCommand _onEndSwitchLocalization;
    private IReadOnlyDictionary<string, string> _currentLocalization;
    // public event Action OnTrySwitchLocalization;
    // public event Action OnEndSwitchLocalization;
    public ReactiveCommand OnTrySwitchLocalization => _onTrySwitchLocalization;
    public ReactiveCommand OnEndSwitchLocalization => _onEndSwitchLocalization;

    public LevelLocalizationHandler(LevelLocalizationProvider levelLocalizationProvider, CharacterProviderBuildMode characterProviderBuildMode,
        SetLocalizationChangeEvent setLocalizationChangeEvent)
    {
        _levelLocalizationProvider = levelLocalizationProvider;
        _characterProviderBuildMode = characterProviderBuildMode;
        _setLocalizationChangeEvent = setLocalizationChangeEvent;
        _onTrySwitchLocalization = new ReactiveCommand();
        _onEndSwitchLocalization = new ReactiveCommand();
        Debug.Log($"Test HashCode Constr {_onTrySwitchLocalization.GetHashCode()}   ");

    }

    public void TrySetCurrentLocalization(SeriaNodeGraphsHandler seriaNodeGraphsHandler, GameStatsHandler gameStatsHandler)
    {
        _currentLocalization = _levelLocalizationProvider.GetCurrentLocalization();
        Debug.Log($"TrySetCurrentLocalization  0");
        if (_currentLocalization != null)
        {
            Debug.Log($"TrySetCurrentLocalization  1");
            SetLocalizationToSeriaTexts(seriaNodeGraphsHandler);
            Debug.Log($"TrySetCurrentLocalization  2");
            SetLocalizationToStats(gameStatsHandler);
            Debug.Log($"TrySetCurrentLocalization  3");
            SetLocalizationToCharacters();
            Debug.Log($"TrySetCurrentLocalization  4");
            _setLocalizationChangeEvent.Execute();
            _currentLocalization = null;
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
        foreach (var character in _characterProviderBuildMode.GetCharacters())
        {
            SetText(character.Name);
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
        Debug.Log($"TrySwitchLanguageFromSettingsChange() 1");
        await _levelLocalizationProvider.TryLoadLocalizationOnSwitchLanguageFromSettings();
        Debug.Log($"TrySwitchLanguageFromSettingsChange() 2");
        Debug.Log($"Test HashCode {_onTrySwitchLocalization.GetHashCode()}");

        _onTrySwitchLocalization.Execute();
        _onEndSwitchLocalization.Execute();
    }
}